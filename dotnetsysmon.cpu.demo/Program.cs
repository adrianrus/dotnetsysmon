using DotnetSysMon.Cpu;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace DotnetSysMon.Cpu.Demo
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            int cpuUsageSample = 5;
            CpuUsage cpu = new CpuUsage();
            PrintTop3(cpu);
            Observable.Interval(TimeSpan.FromSeconds(cpuUsageSample)).Subscribe(_ => PrintTop3(cpu));

            Console.WriteLine("Processor count: {0}", Environment.ProcessorCount);
            Console.WriteLine("Sample interval {0}", cpuUsageSample);
            Console.WriteLine("Press any key to increase load.");
            while (true)
            {
                Processor();
                Console.ReadKey();
            }
        }

        private static void PrintTop3(CpuUsage cpu)
        {
            var items = cpu.GetProcessesDescending().Take(3);

            foreach (var item in items)
            {
                Console.WriteLine("  {0} {1:P}", item.ExeFilename, item.CpuUsage);
            }
            Console.WriteLine();
        }

        private static int _threadCount = 1;

        private static void Processor()
        {
            Console.WriteLine("Starting thread: {0}....", _threadCount);
            _threadCount++;
            Task.Run(() =>
            {
                long i = 0;
                while (true)
                {
                    i++;
                };
            });
        }
    }
}
