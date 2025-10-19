namespace PixelStatsPlotter.Calculating;

/// <summary> 统计图像并暂存结果的接口 </summary>
internal interface IStatsBuffer
{
    /// <summary> 统计单张图像并暂存结果 </summary>
    /// <param name="img"> 待测图像 </param>
    void Collect(OpenCvSharp.Mat img);

    /// <summary> 获取所有统计结果 </summary>
    /// <returns> (指标名, 所有结果值)数组 </returns>
    (string Name, double[] Values)[] GetResults();
}
