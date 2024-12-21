namespace Sardine.Core.Logs
{
    public interface ISardineTraceHandler
    {
        LogMessage? Handle(string trace);
    }
}
