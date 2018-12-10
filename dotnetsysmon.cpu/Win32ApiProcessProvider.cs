using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ComType = System.Runtime.InteropServices.ComTypes;
using PInvoke;

namespace DotnetSysMon.Cpu
{
    internal static class Win32ApiProcessProvider
    {
        public static IEnumerable<ProcessThreadTimes> GetAllProcesses()
        {
            var snapshot = Kernel32.CreateToolhelp32Snapshot(Kernel32.CreateToolhelp32SnapshotFlags.TH32CS_SNAPPROCESS, 0);
            if (snapshot.IsInvalid) yield break;

            using (snapshot)
            {
                var processes = Kernel32.Process32Enumerate(snapshot);
                foreach (var processInfo in processes)
                {
                    using (var currentProcess = Kernel32.OpenProcess(Kernel32.ProcessAccess.PROCESS_QUERY_LIMITED_INFORMATION, false, processInfo.th32ProcessID))
                    {
                        ProcessThreadTimes pt = new ProcessThreadTimes();
                        pt.Info = processInfo;

                        if (GetProcessTimes(currentProcess,
                            out pt.create,
                            out pt.exit,
                            out pt.kernel,
                            out pt.user))
                        {
                            yield return pt;
                        }
                    }
                }
            }
        }

        [DllImport("kernel32.dll")]
        public static extern bool GetProcessTimes(Kernel32.SafeObjectHandle handle, out long creation, out long exit, out long kernel, out long user);
    }

    internal class ProcessThreadTimes {
        internal long create;
        internal long exit;
        internal long kernel;
        internal long user;

        public DateTime StartTime {
            get {
                return DateTime.FromFileTime(create);
            }
        }

        public DateTime ExitTime {
            get {
                return DateTime.FromFileTime(exit);
            }
        }

        public TimeSpan PrivilegedProcessorTime {
            get {
                return new TimeSpan(kernel);
            }
        }

        public TimeSpan UserProcessorTime {
            get {
                return new TimeSpan(user);
            }
        }

        public TimeSpan TotalProcessorTime {
            get {
                return new TimeSpan(user + kernel);
            }
        }

        public Kernel32.PROCESSENTRY32 Info { get; internal set; }
    }
}