using System.Text;

namespace Sardine.Core.Logger
{
    public class LogWriter : TextWriter
    {
        public override Encoding Encoding => Encoding.Default;
        public LogLevel Level { get; }

        string stringOnBuild = string.Empty;

        public override void Write(char value)
        {
            if (value == '\n')
            {
                this.Log(new LogMessage(stringOnBuild, Level));
                stringOnBuild = string.Empty;
                return;
            }

            stringOnBuild += value;
        }

        public override void WriteLine(string? value)
        {
            this.Log(new LogMessage(value??"", Level));
        }


        internal LogWriter(LogLevel logLevel = LogLevel.Information)
        {
            Level = logLevel;
        }
    }
}