using Sardine.Core.Logs;
using System.Text.RegularExpressions;

namespace Sardine.Core.Views.WPF
{
    public class PresentationTraceHandler : ISardineTraceHandler
    {
        readonly static Regex regexBinding = new("(?<=BindingExpression:Path=).*?(?=;)");
        readonly static Regex regexDataItem = new("(?<=DataItem='VesselViewModel_).*?(?=')");
        readonly static Regex exceptionTrace = new("(?<=Exception:')[^']*(?=')");

        public static PresentationTraceHandler Instance { get; } = new();

        private PresentationTraceHandler() { }

        public LogMessage? Handle(string trace)
        {
            Match dataItem = regexDataItem.Match(trace);
            Match binding = regexBinding.Match(trace);
            Match exception = exceptionTrace.Match(trace);


            if (binding.Success)
            {
                if (dataItem.Success)
                {
                    return new LogMessage($"Binding error in Vessel UI [{dataItem.Value}.{binding.Value}]{(exception.Success?$" - {exception.Value}":"")}", LogLevel.Notice);
                }
                return new LogMessage($"Binding error in SARDINE UI [{binding.Value}] - {(exception.Success?exception.Value:trace)}", LogLevel.Warning);
            }

            return new LogMessage($"UI Error {(dataItem.Success ? $"[{dataItem.Value}]" : "")} - {(exception.Success ? exception.Value : trace)}", LogLevel.Error);
        }
    }
}
