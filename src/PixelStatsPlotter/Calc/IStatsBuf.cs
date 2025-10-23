namespace PixelStatsPlotter.Calc;

/// <summary> 像素值统计缓冲区：统计并暂存结果 </summary>
internal interface IStatsBuf
{
    /// <summary> 统计像素值并追加暂存结果 </summary>
    void Collect(OpenCvSharp.Mat img);

    /// <summary> 获取所有统计结果的快照 </summary>
    StatResult[] Snapshot();
}
