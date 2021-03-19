using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace DotnetSysMon.Cpu
{
    public partial class CpuUsage
    {
        public CpuUsage()
        {
            InitializeCache();
        }

        private void InitializeCache()
        {
            GetProcesses().Count();
        }

        public IEnumerable<ProcessCpuUsage> GetProcessesDescending()
        {
            return GetProcesses().OrderByDescending(p => p.CpuUsage);
        }

        public IEnumerable<ProcessCpuUsage> GetProcesses()
        {
            var oldData = _processData;
            var newProcessData = new Dictionary<int, CpuData>();

            try
            {
                return Win32ApiProcessProvider.GetAllProcesses()
                    .Select(p => new ProcessCpuUsage(p.Info.ExeFile, (int)p.Info.th32ProcessID, Get(
                        oldData,
                        newProcessData,
                        p.Info.th32ProcessID,
                        p.TotalProcessorTime),
                        p.TotalProcessorTime));
            }
            finally
            {
                _processData = newProcessData;
            }
        }

        public double Get(Process process)
        {
            return Get(_processData, _processData, process.Id, GetTotalProcessorTime(process));
        }

        internal double Get(
            Dictionary<int, CpuData> processData,
            Dictionary<int, CpuData> newProcessData,
            int id,
            TimeSpan? cpuTime)
        {
            if (!cpuTime.HasValue) return Double.NaN;

            CpuData data;
            if (processData.TryGetValue(id, out data))
            {
                TimeSpan newCPUTime = cpuTime.Value;
                var cpuUsage = (newCPUTime - data.OldCpuTime).TotalSeconds / (Environment.ProcessorCount * DateTime.UtcNow.Subtract(data.LastMonitorTime).TotalSeconds);

                data.LastMonitorTime = DateTime.UtcNow;
                data.OldCpuTime = newCPUTime;
                newProcessData[id] = data;
                return cpuUsage;
            }

            newProcessData.Add(id, new CpuData(cpuTime.Value));
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

        private Dictionary<int, CpuData> _processData = new Dictionary<int, CpuData>();

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
