namespace PixelStatsPlotter.Calc;

/// <summary> 统计结果：名称和值数组 </summary>
internal readonly record struct StatResult(
    string Name, double[] Values);
