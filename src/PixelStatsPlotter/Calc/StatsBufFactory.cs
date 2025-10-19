using PixelStatsPlotter.Enums;

namespace PixelStatsPlotter.Calc;

/// <summary> IStatsBuf实现类的工厂 </summary>
internal static class StatsBufFactory
{
    /// <summary> 根据通道和统计量创建IStatsBuf实现类 </summary>
    public static IStatsBuf Create(ImgCh ch, Stat stat)
        => stat switch {
        };
}
