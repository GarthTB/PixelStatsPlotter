using OpenCvSharp;
using PixelStatsPlotter.Calc;
using PixelStatsPlotter.Enums;

namespace PixelStatsPlotter.Config;

/// <summary> 由TomlModel提取并验证的处理配置 </summary>
/// <param name="Files"> 图像序列或视频文件信息 </param>
/// <param name="Range"> 待测帧范围：无效则统计全部 </param>
/// <param name="GetRoi"> ROI的提取方法 </param>
/// <param name="StatsBufs"> 所需的所有统计缓冲区 </param>
internal sealed record ProcCfg(
    FileInfo[] Files,
    (int Start, int Count) Range,
    Func<Mat, Mat> GetRoi,
    IStatsBuf[] StatsBufs)
{
    /// <summary> 从TomlModel提取并验证处理配置 </summary>
    public static ProcCfg FromToml(TomlModel toml) {
        var files = toml.Input switch {
            var dir when Directory.Exists(dir)
                && Directory.GetFiles(dir) is { Length: > 0 } paths
                => OrderFiles(paths, toml.Order.Key, toml.Order.Asc),
            var file when File.Exists(file) => [new FileInfo(file)],
            var s => throw new ArgumentException(
                $"输入路径 `{s}` 无效", nameof(toml))
        };
        var (start, count) = toml.Range; // 用时再结合实际长度检查

        Func<Mat, Mat> getRoi = toml.Roi is ( >= 0, >= 0, > 0, > 0) roi
            ? mat => new(mat, new Rect(roi.X, roi.Y, roi.W, roi.H))
            : static mat => mat; // 无效则统计全帧

        var stats = toml.Stats.Select(static s
            => s.Split('_') is { Length: 2 } parts
            && Enum.TryParse(parts[0], true, out ImgCh tgtCh)
            && Enum.TryParse(parts[1], true, out Stat stat)
            ? (tgtCh, stat)
            : throw new ArgumentException(
                $"统计项目 `{s}` 无效", nameof(toml)));
        var bufs = StatsBufFactory.CreateFromStats(stats);

        return new(files, (start, count), getRoi, [.. bufs]);

        static FileInfo[] OrderFiles(string[] paths, string key, bool asc) {
            if (!Enum.TryParse(key, true, out OrdKey ordKey))
                throw new ArgumentException(
                    $"图像排序依据 `{key}` 无效", nameof(key));
            Func<FileInfo, object> selector = ordKey switch {
                OrdKey.Name => static fi => fi.Name,
                OrdKey.CreationTime => static fi => fi.CreationTimeUtc,
                OrdKey.LastAccessTime => static fi => fi.LastAccessTimeUtc,
                OrdKey.LastWriteTime => static fi => fi.LastWriteTimeUtc,
                OrdKey.Size => static fi => fi.Length,
                _ => throw new ArgumentException(
                    $"图像排序依据 `{key}` 无效", nameof(key))
            };
            var infos = paths.Select(static f => new FileInfo(f));
            var ordered = asc
                ? infos.OrderBy(selector)
                : infos.OrderByDescending(selector);
            return [.. ordered];
        }
    }

    /// <summary> 图像排序依据 </summary>
    private enum OrdKey
    { Name, CreationTime, LastAccessTime, LastWriteTime, Size }
}
