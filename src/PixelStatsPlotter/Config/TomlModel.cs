namespace PixelStatsPlotter.Config;

/// <summary> 用于Tomlyn解析的原始TOML模型 </summary>
/// <param name="Input"> 图像序列所在目录或视频文件路径 </param>
/// <param name="Order"> 图像顺序（视频始终按时序） </param>
/// <param name="Range"> 待测帧范围：无效则统计全部 </param>
/// <param name="Roi"> ROI坐标：无效则统计全帧 </param>
/// <param name="Stats"> 统计项目："通道_统计量"数组 </param>
/// <param name="Size"> 输出图像尺寸：无效则HD </param>
internal sealed record TomlModel(
    string Input,
    TomlModel.OrderTable Order,
    TomlModel.RangeTable Range,
    TomlModel.RoiTable Roi,
    string[] Stats,
    TomlModel.SizeTable Size)
{
    public TomlModel() : this("", new(), new(), new(), [], new()) { }

    internal sealed record OrderTable(string Key, bool Asc)
    { public OrderTable() : this("", true) { } }

    internal sealed record RangeTable(int Start, int Count)
    { public RangeTable() : this(0, 0) { } }

    internal sealed record RoiTable(int X, int Y, int W, int H)
    { public RoiTable() : this(0, 0, 0, 0) { } }

    internal sealed record SizeTable(int W, int H)
    { public SizeTable() : this(1280, 720) { } }
}
