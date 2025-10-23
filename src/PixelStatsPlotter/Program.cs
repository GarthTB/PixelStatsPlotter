using PixelStatsPlotter.Config;
using PixelStatsPlotter.Output;
using PixelStatsPlotter.Proc;

try {
    Show("Pixel Stats Plotter | 像素统计折线图");
    Show("v1.0.1 (20251024)");
    Show("作者: GarthTB | 天卜 <g-art-h@outlook.com>");
    Show("仓库: https://github.com/GarthTB/PixelStatsPlotter");

    var toml = LoadToml();
    var cfg = CfgModel.FromToml(toml);
    Show("配置就绪！");

    var data = cfg.Files.Length > 1 // CfgModel保证非空
        ? ImageProc.Run(cfg)
        : VideoProc.Run(cfg); // 内部打印进度

    Show("绘制折线图...");
    using var plot = Plotter.Render(data);
    var path = GetOutputPath();
    var info = plot.SavePng(path, cfg.PlotSize.W, cfg.PlotSize.H);
    Show($"折线图已存至：{info.Path}");
    info.LaunchFile();
} catch (Exception ex) {
    Console.ForegroundColor = ConsoleColor.Red;
    Show("运行出错，中断！");
    Show("错误信息：");
    Show(ex.Message);
    Show("堆栈跟踪：");
    Show(ex.StackTrace);
    Console.ResetColor();
} finally { Show("程序结束！"); }

static void Show(string? msg)
    => Console.WriteLine(msg);

static Tomlyn.Model.TomlTable LoadToml() {
    const string PATH = "cfg.toml";
    if (!File.Exists(PATH))
        throw new FileNotFoundException(
            $"找不到配置文件 `{PATH}`", PATH);
    var text = File.ReadAllText(PATH);
    return Tomlyn.Toml.ToModel(text);
}

static string GetOutputPath() {
    const string NAME = "StatsPlot";
    var path = $"{NAME}.png";
    for (var i = 2; File.Exists(path); i++)
        path = $"{NAME}_{i}.png";
    return path;
}
