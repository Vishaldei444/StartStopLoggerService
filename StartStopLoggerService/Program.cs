using System;
using System.IO;
using System.ServiceProcess;
using System.Threading;

namespace StartStopLoggerService
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length == 1 && args[0] == "--time-logger")
            {
                ServiceBase.Run(new TimeLogger());
                return;
            }

            ServiceBase.Run(new StartStopLogger());
        }
    }

    internal sealed class StartStopLogger : ServiceBase
    {
        public StartStopLogger()
        {
            ServiceName = "StartStopLoggerService";
            CanStop = true;
        }

        protected override void OnStart(string[] args)
        {
            LogFile.Append("start-stop.log", "Service started");
        }

        protected override void OnStop()
        {
            LogFile.Append("start-stop.log", "Service stopped");
        }
    }

    internal sealed class TimeLogger : ServiceBase
    {
        private const int IntervalMilliseconds = 10000;
        private readonly object timerLock = new object();
        private Timer timer;

        public TimeLogger()
        {
            ServiceName = "TimeLoggerService";
            CanStop = true;
        }

        protected override void OnStart(string[] args)
        {
            LogFile.Append("time.log", "Current time");
            timer = new Timer(
                WriteCurrentTime, null, IntervalMilliseconds, IntervalMilliseconds);
        }

        protected override void OnStop()
        {
            lock (timerLock)
            {
                timer?.Dispose();
                timer = null;
            }
        }

        private void WriteCurrentTime(object state)
        {
            lock (timerLock)
            {
                if (timer != null)
                {
                    LogFile.Append("time.log", "Current time");
                }
            }
        }
    }

    internal static class LogFile
    {
        private static readonly string Folder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "LoggerServices");

        internal static void Append(string fileName, string message)
        {
            Directory.CreateDirectory(Folder);

            string path = Path.Combine(Folder, fileName);
            string entry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {message}";
            File.AppendAllText(path, entry + Environment.NewLine);
        }
    }
}