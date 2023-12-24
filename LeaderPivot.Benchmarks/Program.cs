namespace LeaderPivot.Benchmarks;
using BenchmarkDotNet;
using BenchmarkDotNet.Running;

internal class Program
{
    static void Main(string[] args)
    {
        BenchmarkRunner.Run<Benchmarks>();
    }
}
