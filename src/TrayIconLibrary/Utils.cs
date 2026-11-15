using System;
using System.Diagnostics;
using System.Drawing;

namespace TrayIconLibrary
{
    public class Utils
    {
        public static String ColorToString(Color c)
        {
            return "#" + c.A.ToString("X2") + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }

        public static Color ColorFromString(string str)
        {
            var mediaColor = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(str);
            Color result = Color.FromArgb(mediaColor.A, mediaColor.R, mediaColor.G, mediaColor.B);
            return result;
        }

        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public static long UtcNowMilliseconds()
        {
            return (long)(DateTime.UtcNow - UnixEpoch).TotalMilliseconds;
        }

        public static void StartProgram(string path, string args = "")
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.UseShellExecute = true;
                startInfo.FileName = ResolveSystemProgram(path);
                startInfo.Arguments = args;
                Process.Start(startInfo);
            }
            catch (Exception)
            {
                // e.g. the UAC prompt was cancelled: do nothing
            }
        }

        // A 32-bit program on 64-bit Windows is silently redirected to the 32-bit
        // copies of system tools in SysWOW64. The 32-bit Task Manager saves its
        // layout in a format the normal one can't read (and vice versa), so its
        // window keeps resetting to the compact view. "Sysnative" reaches the
        // real 64-bit System32 folder instead.
        private static string ResolveSystemProgram(string fileName)
        {
            if (Environment.Is64BitOperatingSystem && !Environment.Is64BitProcess
                && fileName.IndexOfAny(new[] { '\\', '/' }) < 0)
            {
                string windir = Environment.GetEnvironmentVariable("windir") ?? @"C:\Windows";
                string native = System.IO.Path.Combine(windir, "Sysnative", fileName);
                if (System.IO.File.Exists(native))
                {
                    return native;
                }
            }
            return fileName;
        }

        public static int CpuFreqMhz(int numIterations = 10000)
        {
            // https://stackoverflow.com/a/31889271
            // returns wrong results under debugger
            // for max accuracy numIterations = int.MaxValue
            long frequency, start, stop;
            double multiplier = 1000 * 1000 * 1000;//nano
            if (WinApi.QueryPerformanceFrequency(out frequency) == false)
                return -1;

            Process.GetCurrentProcess().ProcessorAffinity = new IntPtr(1);
            const int known_instructions_per_loop = 1;

            // decreased num of iteration to decrease calculation time with less accuracy
            int iterations = numIterations;
            int g = 0;

            WinApi.QueryPerformanceCounter(out start);
            for (int i = 0; i < iterations; i++)
            {
                g++;
                g++;
                g++;
                g++;
            }
            WinApi.QueryPerformanceCounter(out stop);

            //normal ticks differs from the WMI data, i.e 3125, when WMI 3201, and CPUZ 3199
            var normal_ticks_per_second = frequency * 1000;
            var ticks = (double)(stop - start);
            var time = (ticks * multiplier) / frequency;
            var loops_per_sec = iterations / (time / multiplier);
            var instructions_per_loop = normal_ticks_per_second / loops_per_sec;

            var ratio = (instructions_per_loop / known_instructions_per_loop);
            var actual_freq = normal_ticks_per_second / ratio;

            if (actual_freq < 0)
            {
                return -1;
            }
            Console.WriteLine((int)(actual_freq / 1000000));

            return (int)(actual_freq / 1000000);
        }
    }
}
