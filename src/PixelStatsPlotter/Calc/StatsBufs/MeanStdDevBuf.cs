using OpenCvSharp;

namespace PixelStatsPlotter.Calc.StatsBufs;

/// <summary> 像素均值与标准差缓冲区 </summary>
/// <param name="tgtCh"> 目标通道 </param>
internal sealed class MeanStdDevBuf(Enums.ImgCh tgtCh) : IStatsBuf
{
    /// <summary> 结果序列 </summary>
    private readonly List<double> _meanBuf = [], _stdDevBuf = [];

    public void Collect(Mat img) {
        var type = img.Type();
        if (tgtCh == Enums.ImgCh.A
            && type.Channels != 4) {
            _meanBuf.Add(1); // 非4通道图的A值视为1：标准差为0
            _stdDevBuf.Add(0);
        } else {
            using Mat meanMat = new(), stdDevMat = new();
            using var tgtMat = img.GetCh(tgtCh, type.Channels);
            tgtMat.MeanStdDev(meanMat, stdDevMat);
            var mean = meanMat.At<double>(0);
            var stdDev = stdDevMat.At<double>(0);
            var normMean = mean.Norm01(type.Depth);
            var normStdDev = stdDev.Norm01StdDev(type.Depth);
            _meanBuf.Add(normMean);
            _stdDevBuf.Add(normStdDev);
        }
    }

    public (string Name, double[] Values)[] Snapshot()
        => [($"{tgtCh}_Mean", [.. _meanBuf]),
            ($"{tgtCh}_StdDev", [.. _stdDevBuf])];
}
