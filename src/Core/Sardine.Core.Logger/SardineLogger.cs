using Sardine.Core.Fleet;

using System.Diagnostics;

namespace Sardine.Core.Logger
{

    public static class SardineLoggerExtensions
    {
        public static void Log(this object caller, LogMessage message)
        {
            SardineLogger.Instance.LogPrivate(message, SardineLogger.GetCallerSardineInstance(caller));
        }

        public static void Log(this object caller, string message)
        {
            SardineLogger.Instance.LogPrivate(new LogMessage(message), SardineLogger.GetCallerSardineInstance(caller));
        }

        public static void Log(this object caller, string message, LogLevel level)
        {
            SardineLogger.Instance.LogPrivate(new LogMessage(message, level), SardineLogger.GetCallerSardineInstance(caller));
        }

        public static void SetAsLoggingSource(this IFleetAggregator aggregator, bool captureConsole = true)
        {
            SardineLogger.Aggregator = aggregator;
            SardineLogger.CaptureOrigin = aggregator.GetVessels().Where(x => x.CaptureLogs).Select(x => x.HandleType).ToList();
            if (captureConsole)
                Console.SetOut(SardineLogger.GetLogWriter(LogLevel.Debug));
        }

        public static void AddLoggingSource(this IFleetAggregator aggregator, Type type)
        {
            SardineLogger.CaptureOrigin.Add(type);
        }
    }


public class SardineLogger
    {
        public static SardineLogger Instance { get; } = new SardineLogger();

        const string LOG_FOLDER_NAME = "logs";

        internal static IFleetAggregator? Aggregator { get; set; } = null;
        
        private SardineLogger() { }

        public List<LogMessage> Logs { get; } = new();
        public string LogDirectory { get; protected set; } = Path.Combine(SARDINE.BaseLocation, LOG_FOLDER_NAME);
        public string? LogFileName { get; protected set; }

        static StreamWriter? writer;

        public event EventHandler<OnNewLogMessageEventArgs>? OnNewLogMessage;

        internal void LogPrivate(LogMessage message, string sourceName)
        {
            writer ??= new StreamWriter(Path.Combine(LogDirectory, FixExtension(LogFileName ?? GetNextSardineFilename())));

            message.Source = sourceName;

            Logs.Add(message);
            OnNewLogMessage?.Invoke(this, new OnNewLogMessageEventArgs(message));
            writer.WriteLine(message.ToString());
            writer.Flush();
        }

        internal static List<Type> CaptureOrigin { get; set; } = new List<Type>();
        internal static string GetCallerSardineInstance(object caller)
        {
            Type type = caller.GetType();


            if (CaptureOrigin.Contains(type))
                return Aggregator?.GetVesselName(caller) ?? "SARDINE";

            if (type == typeof(LogWriter))
            {
                foreach(Type t in new StackTrace().GetFrames().Select(x=>x.GetMethod()!.DeclaringType!))
                {
                    if (CaptureOrigin.Contains(t))
                    {
                        return t.Name;
                    }
                }
            }

            return "SARDINE";
        }


        static string FixExtension(string v) => Path.HasExtension(v) ? v : Path.ChangeExtension(v, ".log");
        string GetNextSardineFilename()
        {
            if (!Directory.Exists(LogDirectory))
                Directory.CreateDirectory(LogDirectory);

            return $"sardine{Directory.GetFiles(LogDirectory, "sardine*.log").Length:000000}.log";
        }

        public static LogWriter GetLogWriter(LogLevel logLevel)
        {
            return new LogWriter(logLevel);
        }

        public static void Clear() => Instance.Logs.Clear();
    }
}