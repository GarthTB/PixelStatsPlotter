# Pixel Stats Plotter | 像素统计折线图 📈

一个图像序列或视频片段的像素值统计工具，能够对指定区域（ROI）
或整个画面中像素值的计算多种统计指标，并将结果输出为折线图。

[![用前必读 README.md](https://img.shields.io/badge/用前必读-README.md-red)](https://github.com/GarthTB/PixelStatsPlotter/blob/master/README.md)
[![开发框架 .NET 10.0](https://img.shields.io/badge/开发框架-.NET%2010.0-blueviolet)](https://dotnet.microsoft.com/zh-cn/download/dotnet/10.0)
[![最新版本 1.0.1](https://img.shields.io/badge/最新版本-1.0.1-brightgreen)](https://github.com/GarthTB/PixelStatsPlotter/releases/latest)
[![开源协议 MIT](https://img.shields.io/badge/开源协议-MIT-brown)](https://mit-license.org)

## ✨ 功能特点

- 🚀 **高效**：4K视频处理速度超30FPS
- ⚙ **自动**：通过TOML文件配置所有参数，无交互
- 🔀 **灵活**：支持多种图像/视频格式、位深、通道数
- 🎯 **精确**：支持隔离通道、指定ROI
- 📊 **直观**：所有指标归一化后输出到单张折线图
- 📦 **易用**：通过压缩包发布，无需依赖，解压即用

## 📥 安装与使用

### 系统要求

- 操作系统：Windows 10 或更高版本
- 架构：x64

> **注意：原生AOT发布，无需安装.NET运行时**

### 使用步骤

1. 下载 [最新版本包](https://github.com/GarthTB/PixelStatsPlotter/releases/latest) 并解压
2. 按需修改目录下的 `cfg.demo.toml` ，然后重命名为 `cfg.toml`
3. 运行程序 `PixelStatsPlotter.exe`
    - 推荐方式：在控制台中运行，以查看输出日志
    - 简便方式：直接运行，执行完毕后自动退出
4. 在程序目录中得到结果图 `StatsPlot.png`

## 📋 配置文件

程序的所有行为均由 `cfg.toml` 文件控制。
以下是一个示例配置及详细说明（随包附带）：

``` toml
# 示例配置：按需修改，然后保存为程序目录下的`cfg.toml`，作为输入参数

# 图像序列所在目录或视频文件路径
input = "Images"

# 图像顺序（视频始终按时序）
# 排序依据："Name", "CreationTime", "LastAccessTime", "LastWriteTime", "Size"
order = { key = "Name", asc = true }

# 待测帧范围：无效或留空则统计全部
range = { start = 0, count = 0 }

# ROI坐标：无效或留空则统计全帧
roi = { x = 0, y = 0, w = 0, h = 0 }

# 统计项目："通道_统计量"数组；每项输出为折线图的一条折线
# 通道："R", "G", "B", "A"
# 灰度图的RGB值均为其灰度值；非4通道图的A值视为1
# 统计量："Max", "Mean", "Min", "StdDev"
stats = [ "R_Mean", "G_Mean", "B_Mean" ]

# 输出图像尺寸：无效或留空则HD
size = { w = 1280, h = 720 }
```

## 🛠 技术栈

- **平台**：.NET 10.0
- **语言**：C# 14.0
- **依赖**：
    - [OpenCvSharp4](https://github.com/shimat/opencvsharp)
    - [ScottPlot](https://scottplot.net/)
    - [Tomlyn](https://github.com/xoofx/Tomlyn)

## 📜 开源信息

- **作者**：GarthTB | 天卜 <g-art-h@outlook.com>
- **许可证**：[MIT 许可证](https://mit-license.org)
    - 可以自由使用、修改和分发软件
    - 可以用于商业项目
    - 必须保留原始版权声明 `Copyright (c) 2025 GarthTB | 天卜`
- **项目地址**：https://github.com/GarthTB/PixelStatsPlotter

## 📝 更新日志

### v1.0.1 (20251024)

- 修复配置读取问题

### v1.0.0 (20251022)

- 首个发布！
