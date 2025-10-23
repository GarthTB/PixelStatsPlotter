using OpenCvSharp;

namespace PixelStatsPlotter.Proc;

/// <summary> 图像序列处理器 </summary>
internal static class ImageProc
{
    public static Calc.StatResult[] Run(Config.CfgModel cfg) {
        var (start, count) = cfg.Range;
        var files = start >= 0
            && count > 0
            && start + count <= cfg.Files.Length
            ? cfg.Files.AsSpan(start, count)
            : cfg.Files.AsSpan(); // 无效则统计全部

        int cur = 0, total = files.Length;
        Console.WriteLine($"处理共 {total} 张图像...");
        foreach (var file in files) {
            Console.WriteLine($"开始第 {++cur}/{total} 张：{file.Name}");
            using Mat img = new(file.FullName, ImreadModes.Unchanged);
            using var roi = cfg.GetRoi(img);
            foreach (var buf in cfg.StatsBufs)
                buf.Collect(roi);
        }
        Console.WriteLine("处理完成！");

        return [.. cfg.StatsBufs.SelectMany(static buf => buf.Snapshot())];
    }
}
