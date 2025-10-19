using OpenCvSharp;

namespace PixelStatsPlotter.Calc.StatsBufs;

/// <summary> 像素标准差缓冲区 </summary>
/// <param name="tgtCh"> 目标通道 </param>
internal sealed class StdDevBuf(Enums.ImgCh tgtCh) : IStatsBuf
{
    /// <summary> 结果序列 </summary>
    private readonly List<double> _buf = [];

    public void Collect(Mat img) {
        var type = img.Type();
        if (tgtCh == Enums.ImgCh.A
            && type.Channels != 4)
            _buf.Add(0); // 非4通道图的A值视为1，标准差为0
        else {
            using Mat _ = new(), stdDevMat = new();
            using var tgtMat = img.GetCh(tgtCh, type.Channels);
            tgtMat.MeanStdDev(_, stdDevMat);
            var stdDev = stdDevMat.At<double>(0);
            var normStdDev = stdDev.Norm01StdDev(type.Depth);
            _buf.Add(normStdDev);
        }
    }

    public (string Name, double[] Values)[] Snapshot()
        => [($"{tgtCh}_StdDev", _buf.ToArray())];
}
