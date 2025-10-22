using BenchmarkDotNet.Attributes;

namespace Benchmarks.Misc;

/// <summary> 控制台打印方法 </summary>
[MemoryDiagnoser] // 分析内存
public class PrintLine
{
    [Params(4, 8)]
    public int IterExp { get; set; }

    [Benchmark(Baseline = true)]
    public void WriteLine() {
        for (var i = 0; i < 1 << IterExp; i++) {
            Console.WriteLine($"第 {i} 个");
            Thread.Sleep(10); // 模拟图像处理
        }
    }

    [Benchmark]
    public void WriteNew() {
        for (var i = 0; i < 1 << IterExp; i++) {
            Console.Write($"第 {i} 个\n");
            Thread.Sleep(10); // 模拟图像处理
        }
    }

    [Benchmark]
    public void WriteReturn() {
        for (var i = 0; i < 1 << IterExp; i++) {
            Console.Write($"\r第 {i} 个");
            Thread.Sleep(10); // 模拟图像处理
        }
    }
}
