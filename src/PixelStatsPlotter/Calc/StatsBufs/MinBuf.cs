using OpenCvSharp;

namespace PixelStatsPlotter.Calc.StatsBufs;

/// <summary> 像素最小值缓冲区 </summary>
/// <param name="tgtCh"> 目标通道 </param>
internal sealed class MinBuf(Enums.ImgCh tgtCh) : IStatsBuf
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
            tgtMat.MinMaxIdx(out var min, out _);
            var normMin = min.Norm01(type.Depth);
            _buf.Add(normMin);
        }
    }

    public (string Name, double[] Values)[] Snapshot()
        => [($"{tgtCh}_Min", _buf.ToArray())];
}
