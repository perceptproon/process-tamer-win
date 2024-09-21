using process_tamer_win.Dto;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace process_tamer_win
{
    internal class Logic : IDisposable
    {
        private Timer? timer = null;
        private bool disposedValue;

        private readonly ConfigDataDto config;
        
        private readonly Action<string> logAction;

        public static readonly int DEBUGLEVEL_NORMAL = 1;
        public static readonly int DEBUGLEVEL_VERBOSE = 2;

        public Logic(ConfigDataDto config, Action<string> logAction)
        {
            this.config = config;
            this.logAction = logAction;
        }

        public void Start()
        {
            if (config.Interval < 1)
            {
                throw new ArgumentException("interval should be positive value!!");
            }
            if (config.Targets == null)
            {
                throw new ArgumentException("targets should not be null!!");
            }
            foreach (var t in config.Targets)
            {
                if (string.IsNullOrEmpty(t.Name))
                {
                    throw new ArgumentException("targets.name should not be null!!");
                }
                if (string.IsNullOrEmpty(t.PathRegex))
                {
                    throw new ArgumentException("targets.pathRegex should not be null!!");
                }
            }
            timer = new Timer(TimerCallback, null, 100, config.Interval);
        }

        public void TimerCallback(Object? stateInfo)
        {
            try
            {
                if (config.DebugLevel >= DEBUGLEVEL_NORMAL)
                {
                    logAction?.Invoke($"check start");
                }

                var procs = new List<(Process process, string name, string path)>();

                if (true)
                {
                    // プロセス一覧取得
                    var gotProcs = Process.GetProcesses();
                    foreach (var p in gotProcs)
                    {
                        string? name = "";
                        string? path = "";
                        try
                        {
                            name = p.ProcessName;
                            path = p.MainModule?.FileName;

                            if (name == null || path == null)
                            {
                                continue;
                            }

                            procs.Add((p, name ?? "", path ?? ""));

                            if (config.DebugLevel >= DEBUGLEVEL_VERBOSE)
                            {
                                logAction?.Invoke($"found process: name={name} path={path}");
                            }

                        }
                        catch (Exception)
                        {
                            // システムプロセス等は取得できないので、ここでエラーを無視する
                            // Trace.WriteLine($"cannnot retrieve a process info: name={name} path={path} err={e_pl.Message}");
                        }
                    }
                }

                if (procs.Any())
                {
                    // 条件と合致するプロセスについて、適切な操作を行う
                    foreach (var p in procs)
                    {
                        CheckAndTameProcess(p.process, p.name, p.path);
                    }
                }
            }
            catch (Exception ex)
            {
                logAction?.Invoke($"エラー発生: {ex.Message} {ex.StackTrace}");
            }
            
        }

        /// <summary>
        /// 設定に合致するプロセスかどうか確認し、そうであれば適切な操作を行う
        /// </summary>
        /// <param name="p"></param>
        /// <param name="name"></param>
        /// <param name="path"></param>
        private void CheckAndTameProcess(Process process, string name, string path)
        {
            foreach (var c in config.Targets)
            {
                if (Regex.IsMatch(path, c.PathRegex))
                {
                    if (config.DebugLevel >= DEBUGLEVEL_NORMAL)
                    {
                        logAction?.Invoke($"match: name={name} path={path} config={c.Name}");
                    }
                    TameProcess(process, name, path, c.Priority, c.Affinity);
                    break;
                }
            }
            
        }
        
        /// <summary>
        /// 指定されたプロセス設定を適応する
        /// </summary>
        /// <param name="process"></param>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <param name="priority">nullの場合設定しない</param>
        /// <param name="affinity">nullの場合設定しない</param>
        private void TameProcess(Process process, string name, string path, ProcessPriorityClass? priority, nint? affinity)
        {
            bool needTame = false;
            if (priority.HasValue && (priority != process.PriorityClass))
            {
                needTame = true;
            }
            if (affinity.HasValue && (affinity != process.ProcessorAffinity))
            {
                needTame = true;
            }

            if (!needTame)
            {
                return;
            }

            if (priority.HasValue)
            {
                process.PriorityClass = priority.Value;
            }
            if (affinity.HasValue)
            {
                process.ProcessorAffinity = affinity.Value;
            }
            
            if (config.DebugLevel >= DEBUGLEVEL_NORMAL)
            {
                logAction?.Invoke($"update process: name={name} path={path} priority={process.PriorityClass} affinity={process.ProcessorAffinity}");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    timer?.Dispose();
                    timer = null;
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }   
}
