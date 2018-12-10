using DotnetSysMon.Cpu;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotnetsysmon.cpu.tests
{
    public class CpuUsageTests
    {
        [Test]
        public void Get_CurrentProcess()
        {
            CpuUsage cpu = new CpuUsage();
            Assert.IsTrue(double.IsNaN(cpu.Get(Process.GetCurrentProcess())));
            Assert.IsFalse(double.IsNaN(cpu.Get(Process.GetCurrentProcess())));
        }

        [Test]
        public void GetProcesses_CurrentProcess()
        {
            CpuUsage cpu = new CpuUsage();
            Assert.IsTrue(double.IsNaN(cpu.GetProcesses().First( x => x.Pid == Process.GetCurrentProcess().Id).CpuUsage));
            Assert.IsFalse(double.IsNaN(cpu.GetProcesses().First( x => x.Pid == Process.GetCurrentProcess().Id).CpuUsage));
        }
    }
}
