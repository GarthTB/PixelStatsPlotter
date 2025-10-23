using OpenCvSharp;

namespace PixelStatsPlotter.Calc.StatsBufs;

/// <summary> 像素最值统计缓冲区 </summary>
/// <param name="tgtCh"> 目标通道 </param>
internal sealed class MinMaxBuf(Enums.ImgCh tgtCh): IStatsBuf
{
    /// <summary> 结果序列 </summary>
    private readonly List<double> _minBuf = [], _maxBuf = [];

    public void Collect(Mat img) {
        var type = img.Type();
        if (tgtCh == Enums.ImgCh.A
            && type.Channels != 4) {
            _minBuf.Add(1); // 非4通道图的A值视为1
            _maxBuf.Add(1);
        } else {
            using var tgtMat = img.GetCh(tgtCh, type.Channels);
            tgtMat.MinMaxIdx(out var min, out var max);
            var normMin = min.Norm01(type.Depth);
            var normMax = max.Norm01(type.Depth);
            _minBuf.Add(normMin);
            _maxBuf.Add(normMax);
        }
    }

    public StatResult[] Snapshot()
        => [new($"{tgtCh}_Min", [.. _minBuf]),
            new($"{tgtCh}_Max", [.. _maxBuf])];
}
