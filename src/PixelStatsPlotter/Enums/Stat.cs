namespace PixelStatsPlotter.Enums;

/// <summary> 统计量 </summary>
[Flags] // 用于归并
internal enum Stat
{
    Max = 1,
    Mean = 2,
    Min = 4,
    StdDev = 8,
    MinMax = Min | Max,
    MeanStdDev = Mean | StdDev
}
