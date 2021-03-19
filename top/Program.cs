using DotnetSysMon.Cpu;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace top
{
    class Program
    {

        static void Main(string[] args)
        {
            var top = Console.CursorTop;
            const int ProcessCount = 10;

            CpuUsage cpu = new CpuUsage();
            Thread.Sleep(500);
            var items = cpu.GetProcessesDescending().ToList();

            PrintTop(items, ProcessCount);

            while (true)
            {
                if (!Sleep(3000, items))
                    return;
                ConsoleOut("");
                Console.SetCursorPosition(0, top);
                items = cpu.GetProcessesDescending().Take(ProcessCount).ToList();
                PrintTop(items, ProcessCount);
            }
        }

        private static bool Sleep(int delay, List<ProcessCpuUsage> items)
        {
            int count = delay / 500;
            for (int i = 0; i < count; i++)
            {
                Thread.Sleep(500);
                if (ShouldQuit(items))
                    return false;
            }

            return true;
        }

        private static bool ShouldQuit(List<ProcessCpuUsage> processes)
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                switch (key.Key)
                {
                    case ConsoleKey.Q: return Quit();
                    case ConsoleKey.K: return KillTopCpu(processes);
                    case ConsoleKey.P: return Pause(processes);

                    default:
                        break;
                }
            }
            return false;
        }

        private static bool Quit()
        {
            ConsoleOut("Quiting ...");
            return true;

        }


        private static bool Pause(List<ProcessCpuUsage> processes)
        {

            ConsoleOut("Pause");
            while (true)
            {
                var (resume, exit) = ShouldResume(processes);
                if (resume || exit)
                    return exit;

                Thread.Sleep(500);
            }

        }



        private static (bool resume, bool exit) ShouldResume(List<ProcessCpuUsage> processes)
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                switch (key.Key)
                {
                    case ConsoleKey.Q: return (true, true);
                    case ConsoleKey.K: KillTopCpu(processes); return (false, false);
                    case ConsoleKey.P:
                        ConsoleOut("Resume");
                        return (true, false);

                    default:
                        break;
                }
            }
            return (false, false);
        }

        private static bool KillTopCpu(List<ProcessCpuUsage> processes)
        {
            var process = processes.First();
            try
            {
                Process.GetProcessById(process.Pid).Kill();
                ConsoleOut($"Stopped process {process.ExeFilename} pid: {process.Pid} ...");

            }
            catch (Exception ex)
            {
                ConsoleOut(ex.Message);
            }
            processes.RemoveAt(0);

            return false;

        }

        private static void ConsoleOut(string str)
        {
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write($"{GetString(str.PadRight(80), 80)}");
            Console.SetCursorPosition(str.Length, Console.CursorTop);

        }

        private static void PrintTop(IEnumerable<ProcessCpuUsage> items, int count)
        {
            ConsoleHeader($"{"Process Filename",-50} {"PID",-10} {"%CPU",-10:P} {"Time+",-10}");

            foreach (var item in items.Take(count))
            {
                Console.WriteLine($"{GetString(item.ExeFilename, 50),-50} {item.Pid,-10} {item.CpuUsage,-10:P} {item.TotalProcessorTime.ToString("hh\\:mm\\:ss")}");
            }

            ConsoleHeader("Menu: Quit(Esc/Q) | Pause/Resume (P) | Kill Top CPU Process (K)".PadRight(83));
        }

        private static void ConsoleHeader(string header)
        {
            var oldBg = Console.BackgroundColor;
            var oldFgColor = Console.ForegroundColor;

            try
            {
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.ForegroundColor = ConsoleColor.Black;

                Console.WriteLine(header);
            }
            finally
            {
                Console.BackgroundColor = oldBg;
                Console.ForegroundColor = oldFgColor;
            }
        }

        private static string GetString(string str, int maxlen)
        {
            return str.Substring(0, Math.Min(maxlen, str.Length));
        }
    }
}
