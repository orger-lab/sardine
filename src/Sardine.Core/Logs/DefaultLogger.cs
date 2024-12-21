using Sardine.Core.Utils.Files;
using Sardine.Core.Utils.Reflection;
using System.Diagnostics;

namespace Sardine.Core.Logs
{
    public sealed class DefaultLogger : ILogger, IDisposable
    {
        private const string LOG_FOLDER_NAME = "logs";
        private const string DEFAULT_TYPE_NAME = "SARDINE";
        private const string LOG_EXTENSION = ".log";

        private StreamWriter? writer;
        private DefaultLogWriter? logWriter;
        private TextWriter? oldConsoleStream;
        private bool disposedValue;
        

        public IReadOnlyList<LogMessage> LogHistory => Logs.AsReadOnly();
        public bool CaptureConsole { get; }
        public bool MatchTypesForConsoleLogs { get; }
        public int Capacity { get; set; } = 1000;
        private List<LogMessage> Logs { get; } = [];
        private Fleet? Aggregator { get; set; }
        private string LogDirectory { get; set; } = string.Empty;
        private string? LogFileName { get; set; }
        private List<Type> LoggingSources { get; set; } = [];

        private StreamWriter Writer
        {
            get
            {
                if (writer is null)
                {
                    string writerFilename = PathOperations.AddExtensionIfMissing(LogFileName ?? GetNextDefaultSardineFilename(), LOG_EXTENSION);
                    writer = new StreamWriter(Path.Combine(LogDirectory, writerFilename));
                    Log(new LogMessage($"Logging into file {writerFilename}", LogLevel.Information), this);
                }

                return writer;
            }
        }


        public event EventHandler<OnNewLogMessageEventArgs>? OnNewLogMessage;


        public DefaultLogger(bool captureConsole = true, bool matchTypesForConsoleLogs = true)
        {
            CaptureConsole = captureConsole;
            MatchTypesForConsoleLogs = matchTypesForConsoleLogs;
        }


        public void ClearHistory() => Logs.Clear();
        public void AddLoggingSource(Type type) => LoggingSources.Add(type);
        public void Log(LogMessage message, object? source = null) => Log(message, GetCallerSardineInstance(source));

        public void StartLogger(Fleet aggregator)
        {
            ArgumentNullException.ThrowIfNull(aggregator);

            Aggregator = aggregator;

            LogDirectory = Path.Combine(SardineInfo.BaseLocation, LOG_FOLDER_NAME);

            foreach (Type t in Aggregator.VesselCollection.Where(x => x.CaptureLogs).Select(x => x.HandleType))
                AddLoggingSource(t);

            if (CaptureConsole)
                StartConsoleCapture();
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private void StartConsoleCapture()
        {
            logWriter = new DefaultLogWriter(this, LogLevel.Debug);
            oldConsoleStream = Console.Out;
            Console.SetOut(logWriter);
        }

        private void Log(LogMessage? message, string? sourceName)
        {
            if (message is null)
                return;

            if (sourceName is not null)
                message.Source = sourceName;

            Logs.Add(message);

            while (Logs.Count > Capacity)
                Logs.RemoveAt(0);

            OnNewLogMessage?.Invoke(this, new OnNewLogMessageEventArgs(message));
            Writer.WriteLine(message.ToString());
            Writer.Flush();
            Trace.WriteLine(message.ToString());
        }

        private string GetCallerSardineInstance(object? caller)
        {
            if (caller is null)
                return DEFAULT_TYPE_NAME;

            if (caller is string outString)
                return outString;

            Type type = caller.GetType();

            if (type.FindRelatedTypeInList(LoggingSources, out Type? outType))
                return outType.Name;

            
            if (MatchTypesForConsoleLogs && type == typeof(DefaultLogWriter))
            {
                foreach (Type t in new StackTrace().GetFrames().Select(x => x.GetMethod()!.DeclaringType!))
                {
                    if (t.FindRelatedTypeInList(LoggingSources, out Type? outType2))
                        return outType2.Name;
                }
            }

            if (Aggregator?.RegisteredServiceTypes.Contains(type) ?? false)
                return type.Name;

            return DEFAULT_TYPE_NAME;
        }

        private string GetNextDefaultSardineFilename()
        {
            if (!Directory.Exists(LogDirectory))
                Directory.CreateDirectory(LogDirectory);

            return $"sardine{Directory.GetFiles(LogDirectory, $"sardine*{LOG_EXTENSION}").Length:000000}{LOG_EXTENSION}";
        }

        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (oldConsoleStream is not null)
                        Console.SetOut(oldConsoleStream);

                    writer?.Dispose();
                    logWriter?.Dispose();
                    logWriter = null;
                }
                disposedValue = true;
            }
        }
    }
}