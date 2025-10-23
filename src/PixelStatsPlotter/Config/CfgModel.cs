using OpenCvSharp;
using Tomlyn.Model;

namespace PixelStatsPlotter.Config;

/// <summary> 由cfg.toml提取并验证的处理配置 </summary>
/// <param name="Files"> 图像序列或视频文件信息 </param>
/// <param name="Range"> 待测帧范围：无效则统计全部 </param>
/// <param name="GetRoi"> ROI的提取方法 </param>
/// <param name="StatsBufs"> 所需的所有统计缓冲区 </param>
/// <param name="PlotSize"> 输出图像尺寸 </param>
internal sealed record CfgModel(
    FileInfo[] Files,
    (int Start, int Count) Range,
    Func<Mat, Mat> GetRoi,
    Calc.IStatsBuf[] StatsBufs,
    (int W, int H) PlotSize)
{
    /// <summary> 从TomlModel提取并验证处理配置 </summary>
    public static CfgModel FromToml(TomlTable toml) {
        if (toml["input"] is not string input)
            throw new ArgumentException(
                "输入路径缺失", nameof(toml));
        var files = input switch {
            var dir when Directory.Exists(dir)
                && Directory.GetFiles(dir) is { Length: > 0 } paths
                => toml["order"] is TomlTable orderTable
                && orderTable["key"] is string sKey
                && Enum.TryParse(sKey, true, out OrdKey eKey)
                && orderTable["asc"] is bool asc
                ? OrderFiles(paths, eKey, asc)
                : throw new ArgumentException(
                    "图像顺序缺失或无效", nameof(toml)),
            var file when File.Exists(file) => [new FileInfo(file)],
            var s => throw new ArgumentException(
                $"输入路径 `{s}` 无效", nameof(toml))
        };
        if (toml["range"] is not TomlTable rangeTable
            || rangeTable["start"] is not int start
            || rangeTable["count"] is not int count)
            (start, count) = (0, 0); // 用时再结合实际长度检查

        Func<Mat, Mat> getRoi = toml["roi"] is TomlTable roiTable
            && roiTable["x"] is int roiX and >= 0
            && roiTable["y"] is int roiY and >= 0
            && roiTable["w"] is int roiW and > 0
            && roiTable["h"] is int roiH and > 0
            ? mat => new(mat, new Rect(roiX, roiY, roiW, roiH))
            : static mat => mat; // 无效则统计全帧

        var stats = toml["stats"] is TomlArray statsArr
            ? statsArr.Select(static o
                => o is string s
                && s.Split('_') is { Length: 2 } parts
                && Enum.TryParse(parts[0], true, out Enums.ImgCh tgtCh)
                && Enum.TryParse(parts[1], true, out Enums.Stat stat)
                ? (tgtCh, stat)
                : throw new ArgumentException(
                    $"统计项目 `{o}` 无效", nameof(toml)))
            : throw new ArgumentException(
                "统计项目缺失", nameof(toml));
        var bufs = Calc.StatsBufFactory.CreateFromStats(stats);

        if (toml["size"] is not TomlTable sizeTable
            || sizeTable["w"] is not int plotW
            || sizeTable["h"] is not int plotH)
            (plotW, plotH) = (1280, 720); // 无效则HD

        return new(files, (start, count), getRoi, [.. bufs], (plotW, plotH));

        static FileInfo[] OrderFiles(string[] paths, OrdKey key, bool asc) {
            Func<FileInfo, object> selector = key switch {
                OrdKey.Name => static fi => fi.Name,
                OrdKey.CreationTime => static fi => fi.CreationTimeUtc,
                OrdKey.LastAccessTime => static fi => fi.LastAccessTimeUtc,
                OrdKey.LastWriteTime => static fi => fi.LastWriteTimeUtc,
                OrdKey.Size => static fi => fi.Length,
                _ => throw new ArgumentException(
                    $"图像排序依据 `{key}` 无效", nameof(key))
            };
            var infos = paths.Select(static f => new FileInfo(f));
            return [.. asc
                ? infos.OrderBy(selector)
                : infos.OrderByDescending(selector)];
        }
    }

    /// <summary> 图像排序依据 </summary>
    private enum OrdKey
    { Name, CreationTime, LastAccessTime, LastWriteTime, Size }
}
