using OpenCvSharp;
using PixelStatsPlotter.Calc;
using PixelStatsPlotter.Enums;

namespace PixelStatsPlotter.Config;

/// <summary> 由TomlModel提取并验证的处理配置 </summary>
/// <param name="Paths"> 图像序列路径或视频文件路径 </param>
/// <param name="Range"> 待测帧范围：无效则测量全部 </param>
/// <param name="GetRoi"> ROI的提取方法 </param>
/// <param name="StatsBufs"> 统计所需的所有IStatsBuf </param>
internal sealed record ProcConfig(
    string[] Paths,
    (int Start, int Count) Range,
    Func<Mat, Mat> GetRoi,
    IStatsBuf[] StatsBufs)
{
    /// <summary> 从TomlModel构造ConfigModel </summary>
    public static ProcConfig FromToml(TomlModel toml) {
        var paths = toml.Input switch {
            var dir when Directory.Exists(dir)
                && Directory.GetFiles(dir) is { Length: > 0 } files
                => OrderFiles(files, toml.Order.Key, toml.Order.Asc),
            var file when File.Exists(file) => [file],
            var s => throw new ArgumentException(
                $"输入路径 `{s}` 无效", nameof(toml))
        };
        var (start, count) = toml.Range; // 用时再结合实际长度检查

        Func<Mat, Mat> getRoi = toml.Roi is ( >= 0, >= 0, > 0, > 0) roi
            ? mat => new(mat, new Rect(roi.X, roi.Y, roi.W, roi.H))
            : static mat => mat; // 无效则测量全帧

        var metrics = toml.Metrics.Select(static metric
            => metric.Split('_') is { Length: 2 } parts
            && Enum.TryParse(parts[0], true, out ImgCh tgtCh)
            && Enum.TryParse(parts[1], true, out Stat stat)
            ? (tgtCh, stat)
            : throw new ArgumentException(
                $"统计项目 `{metric}` 无效", nameof(toml)));
        var bufs = StatsBufFactory.CreateFromMetrics(metrics);

        return new(paths, (start, count), getRoi, [.. bufs]);

        static string[] OrderFiles(string[] files, string key, bool asc) {
            if (!Enum.TryParse(key, true, out OrdKey ordKey))
                throw new ArgumentException(
                    $"图像排序依据 `{key}` 无效", nameof(key));
            Func<FileInfo, object> selector = ordKey switch {
                OrdKey.Name => static fi => fi.Name,
                OrdKey.CreationTime => static fi => fi.CreationTime,
                OrdKey.LastAccessTime => static fi => fi.LastAccessTime,
                OrdKey.LastWriteTime => static fi => fi.LastWriteTime,
                OrdKey.Size => static fi => fi.Length,
                _ => throw new ArgumentException(
                    $"图像排序依据 `{key}` 无效", nameof(key))
            };
            var fis = files.Select(static f => new FileInfo(f));
            var ordered = asc
                ? fis.OrderBy(selector)
                : fis.OrderByDescending(selector);
            return [.. ordered.Select(static f => f.FullName)];
        }
    }

    /// <summary> 图像排序依据 </summary>
    private enum OrdKey
    { Name, CreationTime, LastAccessTime, LastWriteTime, Size }
}
