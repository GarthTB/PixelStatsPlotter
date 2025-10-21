using OpenCvSharp;

namespace PixelStatsPlotter.Proc;

/// <summary> 视频处理器 </summary>
internal static class VideoProc
{
    public static (string Name, double[] Values)[] Run(Config.ConfigModel config) {
        var file = config.Paths.FirstOrDefault()
            ?? throw new ArgumentException(
                "未指定视频文件路径", nameof(config));
        using VideoCapture capture = new(file);
        if (!capture.IsOpened())
            throw new InvalidOperationException(
                $"无法打开视频文件 `{file}`");

        Console.WriteLine("开始处理视频...");
        using (Mat frame = new())
            if (config.Range is ( >= 0, > 0) range) {
                if (!capture.Set(VideoCaptureProperties.PosFrames, range.Start))
                    throw new InvalidOperationException(
                        $"无法设置起始帧号 `{range.Start}`");
                for (var cnt = 0; cnt < range.Count; cnt++)
                    if (capture.Read(frame))
                        CollectFrame(frame);
                    else {
                        Console.WriteLine($"第 {cnt}/{range.Count} 帧超出结尾。");
                        break;
                    }
            } else
                while (capture.Read(frame))
                    CollectFrame(frame);
        Console.WriteLine("处理完成！");

        return [.. config.StatsBufs.SelectMany(static buf => buf.Snapshot())];

        void CollectFrame(Mat frame) {
            using var roi = config.GetRoi(frame);
            foreach (var buf in config.StatsBufs)
                buf.Collect(roi);
        }
    }
}
