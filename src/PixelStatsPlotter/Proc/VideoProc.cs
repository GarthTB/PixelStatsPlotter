using OpenCvSharp;

namespace PixelStatsPlotter.Proc;

/// <summary> 视频处理器 </summary>
internal static class VideoProc
{
    public static (string Name, double[] Values)[] Run(Config.ProcCfg cfg) {
        using VideoCapture capture = new(cfg.Paths[0]); // ProcCfg保证非空
        if (!capture.IsOpened())
            throw new InvalidOperationException(
                $"无法打开视频文件 `{cfg.Paths[0]}`");

        Console.WriteLine("开始处理视频...");
        using (Mat frame = new())
            if (cfg.Range is ( >= 0, > 0) range) {
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

        return [.. cfg.StatsBufs.SelectMany(static buf => buf.Snapshot())];

        void CollectFrame(Mat frame) {
            using var roi = cfg.GetRoi(frame);
            foreach (var buf in cfg.StatsBufs)
                buf.Collect(roi);
        }
    }
}
