using OpenCvSharp;

namespace PixelStatsPlotter.Calc.StatsBufs;

/// <summary> 像素均值统计缓冲区 </summary>
/// <param name="tgtCh"> 目标通道 </param>
internal sealed class MeanBuf(Enums.ImgCh tgtCh): IStatsBuf
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
            var mean = tgtMat.Mean().Val0;
            var normMean = mean.Norm01(type.Depth);
            _buf.Add(normMean);
        }
    }

    public StatResult[] Snapshot()
        => [new($"{tgtCh}_Mean", [.. _buf])];
}
