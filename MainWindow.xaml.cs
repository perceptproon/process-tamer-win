using process_tamer_win.Dto;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace process_tamer_win
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Logic? logic = null;
        private readonly List<string> logLines = new List<string>();
        private readonly Mutex mutex;

        public MainWindow()
        {
            bool createdNew;
            mutex = new Mutex(true, AppConfig.APP_MUTEX_KEY, out createdNew);
            if (!createdNew)
            {
                MessageBox.Show("すでに起動しています。起動を中止します。", AppConfig.APP_TITLE, MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(255);
            }

            InitializeComponent();
            this.Title = AppConfig.APP_TITLE;

            _ = StartAsync();
        }

        private async Task StartAsync()
        {
            // YAMLファイルをロード
            try
            {
                var configFilePath = GetConfigFilePath();
                var yaml = await File.ReadAllTextAsync(configFilePath, Encoding.UTF8);
                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .Build();
                
                var config = deserializer.Deserialize<ConfigDataDto>(yaml);

                if (config.DebugLevel > 0)
                {
                    this.Visibility = Visibility.Visible;
                }

                if (config.DebugLevel > 0)
                {
                    WriteLog($"start");
                    WriteLog($"configFile={configFilePath}");
                    var ser = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
                    WriteLog($"config={Environment.NewLine}{ser.Serialize(config)}");
                }

                // つつましく生きていく
                Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.Idle;

                logic = new Logic(config, (s) => WriteLog(s));
                logic.Start();
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"fail: ex={ex}");
                MessageBox.Show($"起動に失敗: {ex.Message}", AppConfig.APP_TITLE, MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(255);
            }
            
        }

        /// <summary>
        /// 適切な設定ファイルパス返却
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        private string GetConfigFilePath ()
        {
            var configFilePath = AppConfig.APP_CONFIG_FILE_NAME;
            if (File.Exists(configFilePath))
            {
                return configFilePath;
            }

            configFilePath = AppConfig.APP_DEFAULT_CONFIG_FILE_NAME;
            if (File.Exists(configFilePath))
            {
                return configFilePath;
            }

            throw new ApplicationException($"設定ファイルが見つかりません path1={AppConfig.APP_CONFIG_FILE_NAME} path2={AppConfig.APP_DEFAULT_CONFIG_FILE_NAME}");
        }

        /// <summary>
        /// ログ書き込み
        /// </summary>
        /// <param name="_line"></param>
        private void WriteLog(string _line)
        {
            var line = $"{DateTime.Now} {_line}";
            Trace.WriteLine(line);
            logLines.Add(line);
            if (logLines.Count > AppConfig.APP_MAX_LOG_LINES)
            {
                logLines.RemoveAt(0);
            }

            Dispatcher.Invoke(() =>
            {
                logbox.Text = string.Join(Environment.NewLine, logLines);
                logbox.ScrollToEnd();
            });
            
        }

        /// <summary>
        /// メニュー->終了ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_File_Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// シャットダウン時に適切な停止処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            logic?.Dispose();
            logic = null;
            mutex.ReleaseMutex();
            WriteLog("shutdown");
        }
    }
}