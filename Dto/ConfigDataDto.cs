namespace process_tamer_win.Dto
{
    /// <summary>
    /// config.yaml格納 YAMLではcamelCaseに変換
    /// </summary>
    internal class ConfigDataDto
    {
        /// <summary>
        /// スキャン間隔(ms)
        /// </summary>
        public uint Interval { get; set; } = 1000;
        /// <summary>
        /// デバッグレベル 0=通常 1=基本 2=詳細
        /// </summary>
        public int DebugLevel { get; set; } = 0;
        /// <summary>
        /// 対象の定義
        /// 対象設定 同じプロセスが複数回該当した場合、最初の設定のみ評価される。
        /// </summary>
        public List<ConfigTargetDto> Targets { get; set; } = new List<ConfigTargetDto>();

    }
}
