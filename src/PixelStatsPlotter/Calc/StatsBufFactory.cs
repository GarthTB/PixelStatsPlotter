using PixelStatsPlotter.Calc.StatsBufs;
using PixelStatsPlotter.Enums;

namespace PixelStatsPlotter.Calc;

/// <summary> 像素值统计结果缓冲区工厂 </summary>
internal static class StatsBufFactory
{
    /// <summary> 依据通道和统计量，构造统计所需的所有IStatsBuf </summary>
    /// <remarks> 统计量无需预先归并 </remarks>
    public static IEnumerable<IStatsBuf> CreateFromMetrics(
        IEnumerable<(ImgCh TgtCh, Stat Stat)> metrics) {
        var statsPerCh =
            from metric in metrics
            group metric by metric.TgtCh into g
            select (g.Key,
                    g.Select(static x => x.Stat)
                     .Aggregate(static (a, b) => a | b));

        List<IStatsBuf> bufs = [];
        foreach (var (tgtCh, stats) in statsPerCh) {
            if (stats.HasFlag(Stat.MeanStdDev))
                bufs.Add(new MeanStdDevBuf(tgtCh));
            else {
                if (stats.HasFlag(Stat.Mean))
                    bufs.Add(new MeanBuf(tgtCh));
                if (stats.HasFlag(Stat.StdDev))
                    bufs.Add(new StdDevBuf(tgtCh));
            }
            if (stats.HasFlag(Stat.MinMax))
                bufs.Add(new MinMaxBuf(tgtCh));
            else {
                if (stats.HasFlag(Stat.Max))
                    bufs.Add(new MaxBuf(tgtCh));
                if (stats.HasFlag(Stat.Min))
                    bufs.Add(new MinBuf(tgtCh));
            }
        }
        return bufs;
    }
}
