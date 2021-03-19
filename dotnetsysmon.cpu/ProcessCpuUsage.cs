using System;

namespace DotnetSysMon.Cpu
{
    public class ProcessCpuUsage
    {
        public ProcessCpuUsage(string exeFilename, int pid, double cpuUsage, TimeSpan totalTime)
        {
            ExeFilename = exeFilename;
            Pid = pid;
            CpuUsage = cpuUsage;
            TotalProcessorTime = totalTime;
        }

        public string ExeFilename { get; }

        public int Pid { get; }

        public double CpuUsage { get; }

        public TimeSpan TotalProcessorTime { get; }
    }
}

