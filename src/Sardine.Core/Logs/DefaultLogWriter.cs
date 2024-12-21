using System.Text;

namespace Sardine.Core.Logs
{
    public sealed class DefaultLogWriter : TextWriter
    {
        private string stringOnBuild = string.Empty;


        private ILogger Logger { get; }
        public override Encoding Encoding => Encoding.Default;
        public LogLevel Level { get; }


        internal DefaultLogWriter(ILogger logger, LogLevel logLevel = LogLevel.Information)
        {
            Logger = logger;
            Level = logLevel;
        }


        public override void WriteLine(string? value) => Logger.Log(new LogMessage(value ?? "", Level), this);

        public override void Write(char value)
        {
            if (value == '\n')
            {
                Logger.Log(new LogMessage(stringOnBuild, Level), this);
                stringOnBuild = string.Empty;
                return;
            }

            stringOnBuild += value;
        }
    }
}