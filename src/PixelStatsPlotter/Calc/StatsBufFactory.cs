using PixelStatsPlotter.Calc.StatsBufs;
using PixelStatsPlotter.Enums;

namespace PixelStatsPlotter.Calc;

/// <summary> 像素值统计结果缓冲区工厂 </summary>
internal static class StatsBufFactory
{
    /// <summary> 依据通道和统计量创建缓冲区 </summary>
    /// <param name="statsPerCh"> 按通道分组的统计量 </param>
    /// <returns> 已初始化的缓冲区实例数组 </returns>
    /// <remarks> 每个通道的所有统计量需提前用位运算合并 </remarks>
    public static IStatsBuf[] Create((ImgCh TgtCh, Stat Stats)[] statsPerCh) {
        List<IStatsBuf> bufs = [];
        foreach (var (tgtCh, stats) in statsPerCh) {
            if (stats.HasFlag(Stat.MeanStdDev)) {
                bufs.Add(new MeanStdDevBuf(tgtCh));
            } else {
                if (stats.HasFlag(Stat.Mean))
                    bufs.Add(new MeanBuf(tgtCh));
                if (stats.HasFlag(Stat.StdDev))
                    bufs.Add(new StdDevBuf(tgtCh));
            }
            if (stats.HasFlag(Stat.MinMax)) {
                bufs.Add(new MinMaxBuf(tgtCh));
            } else {
                if (stats.HasFlag(Stat.Max))
                    bufs.Add(new MaxBuf(tgtCh));
                if (stats.HasFlag(Stat.Min))
                    bufs.Add(new MinBuf(tgtCh));
            }
        }
        return [.. bufs];
    }
}
