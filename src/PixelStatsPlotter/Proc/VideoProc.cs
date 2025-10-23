using OpenCvSharp;

namespace PixelStatsPlotter.Proc;

/// <summary> 视频处理器 </summary>
internal static class VideoProc
{
    public static Calc.StatResult[] Run(Config.ProcCfg cfg) {
        var path = cfg.Files[0].FullName; // ProcCfg保证非空
        using VideoCapture capture = new(path);
        if (!capture.IsOpened())
            throw new InvalidOperationException(
                $"无法打开视频文件 `{path}`");

        Console.WriteLine("处理视频...");
        int cnt = 0, total = cfg.Range.Count, start = cfg.Range.Start;
        if (start >= 0 && total > 0) {
            if (!capture.Set(VideoCaptureProperties.PosFrames, start))
                throw new InvalidOperationException(
                    $"无法设置起始帧号 `{start}`");
            while (cnt < total) {
                Console.WriteLine($"开始第 {++cnt}/{total} 帧...");
                using Mat frame = new();
                if (capture.Read(frame))
                    CollectFrame(frame);
                else {
                    Console.WriteLine($"已到结尾，只有 {cnt - 1} 帧。");
                    break;
                }
            }
        } else while (true) {
                Console.WriteLine($"开始第 {++cnt} 帧...");
                using Mat frame = new();
                if (capture.Read(frame))
                    CollectFrame(frame);
                else {
                    Console.WriteLine($"已到结尾，共 {cnt - 1} 帧。");
                    break;
                }
            }
        Console.WriteLine("处理完成！");

        return [.. cfg.StatsBufs.SelectMany(static buf => buf.Snapshot())];

        void CollectFrame(Mat frame) {
            using var roi = cfg.GetRoi(frame);
            foreach (var buf in cfg.StatsBufs)
                buf.Collect(roi);
        }
    }
}
