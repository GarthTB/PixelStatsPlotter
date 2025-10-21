using PixelStatsPlotter.Config;
using PixelStatsPlotter.Output;
using PixelStatsPlotter.Proc;

try {
    Show("Pixel Stats Plotter | 像素统计折线图");
    Show("v1.0.0 (20251022)");
    Show("作者: GarthTB | 天卜 <g-art-h@outlook.com>");
    Show("仓库: https://github.com/GarthTB/PixelStatsPlotter");
    var toml = LoadToml();
    var data = Proc(toml);
    using var plot = Plotter.Render(data);
    OutputPlot(plot, toml.Size.W, toml.Size.H);
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

TomlModel LoadToml() {
    const string path = "cfg.toml";
    if (!File.Exists(path))
        throw new FileNotFoundException(
            $"找不到配置文件 `{path}`", path);
    var text = File.ReadAllText(path);
    var toml = Tomlyn.Toml.ToModel<TomlModel>(text);
    Show("配置就绪！");
    return toml;
}

static (string Name, double[] Values)[] Proc(TomlModel toml) {
    var cfg = ProcCfg.FromToml(toml);
    return cfg.Paths.Length > 1 // ProcCfg保证非空
        ? ImageProc.Run(cfg) // 内部打印提示信息
        : VideoProc.Run(cfg);
}

void OutputPlot(ScottPlot.Plot plot, int w, int h) {
    const string name = "StatsPlot";
    var path = $"{name}.png";
    for (var i = 2; File.Exists(path); i++)
        path = $"{name}_{i}.png";
    _ = w > 0 && h > 0
        ? plot.SavePng(path, w, h)
        : plot.SavePng(path, 1920, 1080);
    Show($"折线图已存至 `{path}`");
}
