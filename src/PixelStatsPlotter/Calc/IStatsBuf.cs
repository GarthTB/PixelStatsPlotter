namespace PixelStatsPlotter.Calc;

/// <summary> 图像统计结果缓冲区 </summary>
internal interface IStatsBuf
{
    /// <summary> 统计图像并追加暂存结果 </summary>
    /// <param name="img"> 待统计图像 </param>
    void Collect(OpenCvSharp.Mat img);

    /// <summary> 获取所有统计结果的快照 </summary>
    /// <returns> (指标名, 结果序列)数组 </returns>
    (string Name, double[] Values)[] Snapshot();
}
