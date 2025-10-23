using PixelStatsPlotter.Calc.StatsBufs;
using PixelStatsPlotter.Enums;

namespace PixelStatsPlotter.Calc;

/// <summary> 像素值统计缓冲区工厂 </summary>
internal static class StatsBufFactory
{
    /// <summary> 依据统计项目，构造对应的所有统计缓冲区 </summary>
    /// <remarks> 统计量无需预先归并 </remarks>
    public static IEnumerable<IStatsBuf> CreateFromStats(
        IEnumerable<(ImgCh TgtCh, Stat Stat)> stats)
        => (from stat in stats
            group stat by stat.TgtCh into g
            select (g.Key,
                    g.Select(static stats => stats.Stat)
                     .Aggregate(static (a, b) => a | b))) // 归并
        .Aggregate(
            new List<IStatsBuf>(),
            static (bufs, statsByCh) => {
                var (tgtCh, stats) = statsByCh;
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
                return bufs;
            });
}
