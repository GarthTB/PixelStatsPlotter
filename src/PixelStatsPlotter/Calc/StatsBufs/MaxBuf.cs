using OpenCvSharp;

namespace PixelStatsPlotter.Calc.StatsBufs;

/// <summary> 像素最大值缓冲区 </summary>
/// <param name="tgtCh"> 目标通道 </param>
internal sealed class MaxBuf(Enums.ImgCh tgtCh) : IStatsBuf
{
    /// <summary> 结果序列 </summary>
    private readonly List<double> _buf = [];

    public void Collect(Mat img) {
        var type = img.Type();
        if (tgtCh == Enums.ImgCh.A
            && type.Channels != 4)
            _buf.Add(1); // 非4通道图的A值视为1
        else {
            using var tgtMat = img.GetCh(tgtCh, type.Channels);
            tgtMat.MinMaxIdx(out _, out var max);
            var normMax = max.Norm01(type.Depth);
            _buf.Add(normMax);
        }
    }

    public (string Name, double[] Values)[] Snapshot()
        => [($"{tgtCh}_Max", [.. _buf])];
}
