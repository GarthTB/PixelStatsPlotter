using OpenCvSharp;

namespace PixelStatsPlotter.Proc;

/// <summary> 图像序列处理器 </summary>
internal static class ImageProc
{
    public static (string Name, double[] Values)[] Run(Config.ProcConfig cfg) {
        var (start, count) = cfg.Range;
        var pathsSpan = start >= 0
            && count > 0
            && start + count <= cfg.Paths.Length
            ? cfg.Paths.AsSpan(start, count)
            : cfg.Paths.AsSpan(); // 无效则测量全部

        Console.WriteLine($"共 {pathsSpan.Length} 张图像。开始处理...");
        foreach (var path in pathsSpan) {
            using Mat img = new(path, ImreadModes.Unchanged);
            using var roi = cfg.GetRoi(img);
            foreach (var buf in cfg.StatsBufs)
                buf.Collect(roi);
        }
        Console.WriteLine("处理完成！");

        return [.. cfg.StatsBufs.SelectMany(static buf => buf.Snapshot())];
    }
}
