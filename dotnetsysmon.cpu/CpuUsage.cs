using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace DotnetSysMon.Cpu
{
    public class CpuUsage
    {
        public IEnumerable<Tuple<Process, double>> GetProcessesDescending()
        {
            var newProcessData = new Dictionary<Tuple<DateTime, int>, CpuData>();

            try
            {
                return Process.GetProcesses()
                    .Select(p => Tuple.Create(p, Get(_processData, newProcessData, p)))
                    .OrderByDescending(x => x.Item2)
                    .ToList();
            }
            finally
            {
                _processData = newProcessData;
            }
        }

        public double Get(Process process)
        {
            return Get(_processData, _processData, process);
        }

        internal double Get(
            Dictionary<Tuple<DateTime, int>, CpuData> processData,
            Dictionary<Tuple<DateTime, int>, CpuData> newProcessData,
            Process process)
        {
            var cpu = GetTotalProcessorTime(process);
            if (!cpu.HasValue) return Double.NaN;

            var key = Tuple.Create(process.StartTime, process.Id);
            CpuData data;
            if (processData.TryGetValue(key, out data))
            {
                TimeSpan newCPUTime = cpu.Value;
                var cpuUsage = (newCPUTime - data.OldCpuTime).TotalSeconds / (Environment.ProcessorCount * DateTime.UtcNow.Subtract(data.LastMonitorTime).TotalSeconds);

                data.LastMonitorTime = DateTime.UtcNow;
                data.OldCpuTime = newCPUTime;
                newProcessData[key] = data;
                return cpuUsage;
            }

            newProcessData.Add(key, new CpuData(cpu.Value));
            return Double.NaN;
        }

        internal class CpuData
        {
            public CpuData(TimeSpan initialProcessTime)
            {
                OldCpuTime = initialProcessTime;
                LastMonitorTime = DateTime.UtcNow;
            }

            public TimeSpan OldCpuTime { get; set; }
            public DateTime LastMonitorTime { get; set; }
        }

        private Dictionary<Tuple<DateTime, int>, CpuData> _processData = new Dictionary<Tuple<DateTime, int>, CpuData>();

        private static TimeSpan? GetTotalProcessorTime(Process process)
        {
            try
            {
                return process.TotalProcessorTime;
            }
            catch (InvalidOperationException) // process exited
            {
                return null;
            }
            catch (Win32Exception ex) when (ex.NativeErrorCode == 5) // Access denied!
            {
                return null;
            }
        }
    }
}
