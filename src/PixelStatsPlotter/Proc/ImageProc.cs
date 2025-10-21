using OpenCvSharp;

namespace PixelStatsPlotter.Proc;

/// <summary> 图像序列处理器 </summary>
internal static class ImageProc
{
    public static (string Name, double[] Values)[] Run(Config.ConfigModel config) {
        var (start, count) = config.Range;
        var pathsSpan = (start, count) is ( >= 0, > 0)
            && start + count <= config.Paths.Length
            ? config.Paths.AsSpan(start, count)
            : config.Paths.AsSpan(); // 无效则测量全部

        Console.WriteLine($"共 {pathsSpan.Length} 张图像。开始处理...");
        foreach (var path in pathsSpan) {
            using Mat mat = new(path, ImreadModes.Unchanged);
            using var roi = config.GetRoi(mat);
            foreach (var buf in config.StatsBufs)
                buf.Collect(roi);
        }
        Console.WriteLine("处理完成！");

        return [.. config.StatsBufs.SelectMany(buf => buf.Snapshot())];
    }
}
