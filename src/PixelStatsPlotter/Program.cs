using PixelStatsPlotter.Config;
using PixelStatsPlotter.Output;
using PixelStatsPlotter.Proc;

try {
    Show("Pixel Stats Plotter | 像素统计折线图");
    Show("v1.0.0 (20251022)");
    Show("作者: GarthTB | 天卜 <g-art-h@outlook.com>");
    Show("仓库: https://github.com/GarthTB/PixelStatsPlotter");

    var toml = LoadToml();
    var cfg = ProcCfg.FromToml(toml);
    Show("配置就绪！");

    var data = cfg.Paths.Length > 1 // ProcCfg保证非空
        ? ImageProc.Run(cfg)
        : VideoProc.Run(cfg); // 内部打印提示信息

    using var plot = Plotter.Render(data);
    var path = GetOutputPath();
    var info = toml.Size is ( > 0, > 0) size
        ? plot.SavePng(path, size.W, size.H)
        : plot.SavePng(path, 1280, 720);
    Show($"折线图已存至：{info.Path}");
    info.LaunchFile();
} catch (Exception ex) {
    Console.ForegroundColor = ConsoleColor.Red;
    Show("运行出错，中断！");
    Show($"错误信息：{ex.Message}");
    Show("堆栈跟踪：");
    Show(ex.StackTrace);
    Console.ResetColor();
} finally { Show("程序结束！"); }

static void Show(string? msg)
    => Console.WriteLine(msg);

static TomlModel LoadToml() {
    const string path = "cfg.toml";
    if (!File.Exists(path))
        throw new FileNotFoundException(
            $"找不到配置文件 `{path}`", path);
    var text = File.ReadAllText(path);
    return Tomlyn.Toml.ToModel<TomlModel>(text);
}

static string GetOutputPath() {
    const string name = "StatsPlot";
    var path = $"{name}.png";
    for (var i = 2; File.Exists(path); i++)
        path = $"{name}_{i}.png";
    return path;
}
