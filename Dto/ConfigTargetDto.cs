using System.Diagnostics;

namespace process_tamer_win.Dto
{
    /// <summary>
    /// 対象設定
    /// </summary>
    internal class ConfigTargetDto
    {
        /// <summary>
        /// 名称 人間のための項目
        /// </summary>
        public string Name { get; set; } = "";
        /// <summary>
        /// 対象プロセスのファイル名正規表現
        /// </summary>
        public string PathRegex { get; set; } = "NOP{9999}";
        /// <summary>
        /// null以外の場合、対象の優先度に変更
        /// Idle / BelowNormal / Normal / AboveNormal / High / RealTime
        /// </summary>
        public ProcessPriorityClass? Priority { get; set; } = null;
        /// <summary>
        /// null以外の場合、指定したCPU Affinityに変更 bit mask指定 
        /// CPU1のみ=1 / CPU2のみ=2 / CPU1-2のみ=3 / CPU3のみ=4 / ...
        /// </summary>
        public nint? Affinity { get; set; } = null; 
    }
}
