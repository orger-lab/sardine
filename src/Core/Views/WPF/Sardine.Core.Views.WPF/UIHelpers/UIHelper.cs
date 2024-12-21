using Sardine.Core.Logs;
using System;

namespace Sardine.Core.Views.WPF.UIHelpers
{
    public class UIHelper
    {
        private Action<SardineWindow> Action { get; }
        public string Name { get; }
        public UIHelperMetadata? Metadata { get; init; }


        public UIHelper(string name, Action action)
        {            
            Name = name;
            Action = (_) => action();
        }

        public UIHelper(string name, Action<SardineWindow> action)
        {   
            Name = name;
            Action = action;
        }


        public void Run(SardineWindow argument)
        {
#pragma warning disable CA1031 // Do not catch general exception types
            try
            {
                Action(argument);
            }
            catch (Exception ex)
            {
                Fleet.Current.Logger.Log(new LogMessage($"Error running loading helper action \"{Name}\": {ex.Message}", LogLevel.Error), null);
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }
    }
}