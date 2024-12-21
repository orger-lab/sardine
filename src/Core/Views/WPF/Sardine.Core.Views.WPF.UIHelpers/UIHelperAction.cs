using Sardine.Core.Logs;

namespace Sardine.Core.Helpers
{
    public class UIHelperAction
    {
        public string Name { get; }
        Action<object?> Action { get; }
        public object? Metadata { get; init; }
        public Type? ExpectedArgumentType { get; init; }


        public UIHelperAction(string name, Action action)
        {
            Name = name;
            Action = (_) => action();
        }

        public UIHelperAction(string name, Action<SardineWindow> action)
        {
            Name = name;
            Action = action;
        }


        public void Run(object? argument = null)
        {
#pragma warning disable CA1031 // Do not catch general exception types
            try
            {
                Action(argument);
            }
            catch (Exception ex)
            {
                Fleet.Current.Logger.Log(new LogMessage($"Error running loading helper action \"{Name}\": {ex.Message}", LogLevel.Error), this);
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }
    }
}