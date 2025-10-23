using ScottPlot;

namespace PixelStatsPlotter.Output;

/// <summary> 折线图绘制器 </summary>
internal static class Plotter
{
    /// <summary> 由统计结果生成折线图 </summary>
    /// <returns> 配置好的Plot对象，用完需释放 </returns>
    public static Plot Render(Calc.StatResult[] data) {
        if (data.Length == 0
            || data[0].Values.Length == 0)
            throw new ArgumentException(
                "统计结果缺失", nameof(data));

        Plot plot = new();
        var xs = data[0].Values.Select(static (_, i) => i).ToArray();
        foreach (var (name, values) in data)
            plot.Add.Scatter(xs, values).LegendText = name;
        _ = plot.ShowLegend();
        plot.Axes.SetLimits(xs[0], xs[^1], 0, 1);
        plot.XLabel("Frame Index");
        plot.YLabel("Stats Value");

        return plot;
    }
}
