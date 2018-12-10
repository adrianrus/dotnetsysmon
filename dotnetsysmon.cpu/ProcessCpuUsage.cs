namespace DotnetSysMon.Cpu
{
    public class ProcessCpuUsage
    {
        public ProcessCpuUsage(string exeFilename, int pid, double cpuUsage)
        {
            ExeFilename = exeFilename;
            Pid = pid;
            CpuUsage = cpuUsage;
        }

        public string ExeFilename { get; }

        public int Pid { get; }

        public double CpuUsage { get; }
    }
}

