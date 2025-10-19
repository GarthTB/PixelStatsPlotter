using OpenCvSharp;
using PixelStatsPlotter.Enums;

namespace PixelStatsPlotter.Calc;

/// <summary> 图像统计工具 </summary>
internal static class StatsUtils
{
    /// <summary> 获取图像的指定通道 </summary>
    /// <param name="src"> 源图，任意通道数 </param>
    /// <param name="tgtCh"> 目标通道 </param>
    /// <param name="chCnt"> 源图通道数，避免重复访问 </param>
    /// <returns> 新Mat，仅含目标通道 </returns>
    /// <exception cref="ArgumentException"> 目标通道与源图通道数冲突 </exception>
    /// <remarks> 需提前获取源图通道数并处理非4通道图的A通道 </remarks>
    public static Mat GetCh(this Mat src, ImgCh tgtCh, int chCnt)
        => (tgtCh, chCnt) switch {
            (ImgCh.R or ImgCh.G or ImgCh.B, 1) => src, // 灰度图的RGB值均为其灰度值
            (ImgCh.R, 3 or 4) => src.ExtractChannel(2),
            (ImgCh.G, 3 or 4) => src.ExtractChannel(1),
            (ImgCh.B, 3 or 4) => src.ExtractChannel(0),
            (ImgCh.A, 4) => src.ExtractChannel(3),
            _ => throw new ArgumentException(
                $"无法获取{chCnt}通道图的{tgtCh}通道", nameof(tgtCh))
        };

    /// <summary> 依据位深度常量归一化值到0-1范围 </summary>
    /// <param name="val"> 原始值 </param>
    /// <param name="depth"> 位深度常量 </param>
    /// <returns> 归一化后的值 </returns>
    /// <exception cref="ArgumentException"> 位深度常量无效 </exception>
    public static double Norm01(this double val, int depth)
        => depth switch {
            MatType.CV_8U => val / byte.MaxValue,
            MatType.CV_8S => (val - sbyte.MinValue) / byte.MaxValue,
            MatType.CV_16U => val / ushort.MaxValue,
            MatType.CV_16S => (val - short.MinValue) / ushort.MaxValue,
            MatType.CV_32S => (val - int.MinValue) / uint.MaxValue,
            MatType.CV_32F or MatType.CV_64F => val,
            _ => throw new ArgumentException(
                $"位深度常量{depth}无效", nameof(depth))
        };

    /// <summary> 依据位深度常量归一化标准差到0-1范围 </summary>
    /// <param name="val"> 原始标准差 </param>
    /// <param name="depth"> 位深度常量 </param>
    /// <returns> 归一化后的标准差 </returns>
    /// <exception cref="ArgumentException"> 位深度常量无效 </exception>
    /// <remarks> 标准差的范围为0-极差的一半 </remarks>
    public static double Norm01StdDev(this double val, int depth)
        => depth switch {
            MatType.CV_8U or MatType.CV_8S => val / (0.5 * byte.MaxValue),
            MatType.CV_16U or MatType.CV_16S => val / (0.5 * ushort.MaxValue),
            MatType.CV_32S => val / (0.5 * uint.MaxValue),
            MatType.CV_32F or MatType.CV_64F => val,
            _ => throw new ArgumentException(
                $"位深度常量{depth}无效", nameof(depth))
        };
}
