using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using DotnetSysMon.Cpu;
using System;
using System.Diagnostics;
using System.Linq;
using static DotnetSysMon.Cpu.CpuUsage;

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
        public ProcessCpuUsage Get()
        {
            return _cpuUsage.GetProcessesDescending().First();
        }
    }
}
