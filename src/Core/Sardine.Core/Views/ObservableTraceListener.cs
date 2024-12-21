using System;
using System.Diagnostics;
using System.Text;

namespace Sardine.Core.Views;

public sealed class ObservableTraceListener : TraceListener
{
    private readonly StringBuilder buffer = new();

    public override void Write(string? message) => buffer.Append(message);

    [DebuggerStepThrough]
    public override void WriteLine(string? message)
    {
        _ = buffer.Append(message);

        OnTrace?.Invoke(this, new OnTraceEventArgs(buffer.ToString()));

        _ = buffer.Clear();
    }

    public event EventHandler<OnTraceEventArgs>? OnTrace;
}

public class OnTraceEventArgs(string trace) : EventArgs
{
    public string Trace { get; } = trace;
}
