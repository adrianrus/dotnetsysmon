using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using DotnetSysMon.Cpu;
using System;
using System.Diagnostics;
using System.Linq;

namespace DotnetSysMon.Cpu.Benchmark
{
    internal partial class Program
    {
        private static void Main(string[] args)
        {
            BenchmarkRunner.Run<CpuBench>();
        }
    }

    [MemoryDiagnoser]
    public class CpuBench
    {
        public static CpuUsage _cpuUsage = new CpuUsage();

        [Benchmark]
        public Tuple<Process, double> Get()
        {
            return _cpuUsage.GetProcessesDescending().First();
        }
    }
}
