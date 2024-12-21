using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using Sardine.Core.Logs;
using System.Reflection;
using Sardine.Core.Utils.Reflection;

namespace Sardine.Core.Views.WPF
{
    public abstract class SardineWindow : Window
    {
        public ConcurrentDictionary<Dispatcher, object?> Dispatchers { get; } = new();


        protected SardineWindow()
        {
            DataContext = this;

            Title = Core.SardineInfo.FleetName;
            Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString((string)SardineApplicationSettings.SardineWindowBackground));

            Loaded += SardineWindow_Loaded;
            Unloaded += (_, _) => { foreach ((Dispatcher dispatcher, _) in Dispatchers) dispatcher.BeginInvokeShutdown(DispatcherPriority.Send); };
            Activated += SardineWindow_Activated;
        }


        protected virtual void SardineLoadedAction() { }

        public void InvokeInNewDispatcher(Action<Dispatcher> action, DispatcherUnhandledExceptionEventHandler? unhandledExceptionHandler = null, string? handlerName = null)
        {
            InvokeInNewDispatcher<object?>((d) => { action(d); return null; }, unhandledExceptionHandler, handlerName);
        }


        public void InvokeInNewDispatcher<T>(Func<Dispatcher, T> action, DispatcherUnhandledExceptionEventHandler? unhandledExceptionHandler = null, string? handlerName = null)
        {
            //Console.WriteLine($"Current task queue: {TaskScheduler.Current.GetScheduledTasksForDebugger().Length}");

            Dispatcher? myDispatcher = null;
            ManualResetEvent dispatcherReadyEvent = new(false);

            Thread t = new(new ThreadStart(() =>
            {
                myDispatcher = Dispatcher.CurrentDispatcher;
                myDispatcher.UnhandledException += unhandledExceptionHandler
                                                   ?? ((sender, exp)
                                                   => {
                                                       exp.Handled = true;
                                                       Fleet.Current.Logger.Log(new LogMessage($"{handlerName ?? "Dispatcher"} exception: {exp.Exception.InnerException?.Message}", LogLevel.Warning), myDispatcher);
                                                       myDispatcher.InvokeShutdown();
                                                   });

                myDispatcher.ShutdownFinished += (sender, _) => Dispatchers.Remove((Dispatcher)sender!, out object? value);

                dispatcherReadyEvent.Set();
                Dispatcher.Run();
            }));

            t.SetApartmentState(ApartmentState.STA);
            t.Start();

            dispatcherReadyEvent.WaitOne();
            dispatcherReadyEvent.Dispose();

            new Task(() =>
            {
                T? result = (T?)myDispatcher?.Invoke(action, myDispatcher);

                if (myDispatcher is null)
                    return;

                if (result is null)
                {
                    myDispatcher.BeginInvokeShutdown(DispatcherPriority.Normal);
                    return;
                }

                Dispatchers.TryAdd(myDispatcher, result);
            }).Start();
        }

        public void CloseAllVesselUIs()
        {
            foreach (KeyValuePair<Dispatcher, object?> kvp in Dispatchers)
            {
                if (kvp.Value is VesselUIWindow window)
                    kvp.Key.Invoke(() => window.Close());
            }
        }

        public void OpenVesselUI(Vessel vessel, Type? uiType = null, (double left, double top)? position = null, bool locked = false)
        {
            InvokeInNewDispatcher((myDispatcher) =>
            {
                if (!VesselUIProvider.CanProvideUI(vessel))
                    return null;

                VesselUIWindow window = new()
                {
                    Title = vessel.DisplayName,
                    Locked = locked,
                };

                window.UIContainer.UIType = uiType;
                window.Vessel = vessel;

                if (position is not null)
                {
                    window.Top = position.Value.top;
                    window.Left = position.Value.left;
                }

                Application.Current.Dispatcher.BeginInvoke(
                    () =>
                    {
                        Unloaded += (_, _) => OnApplicationClosingDispatcherInvocation(myDispatcher, window);
                    });

                window.Closing += (_, _) =>
                {
                    Application.Current.Dispatcher.BeginInvoke(() =>
                    {
                        Unloaded -= (_, _) => OnApplicationClosingDispatcherInvocation(myDispatcher, window);
                    }
                    );
                    myDispatcher.BeginInvokeShutdown(DispatcherPriority.Normal);

                };

                window.Show();
                return window;
            }
            , (_, e) => { e.Handled = true; vessel.UnhandledVesselExceptionHandler(e.Exception); });
        }

        private void SardineWindow_Activated(object? sender, EventArgs e)
        {
            foreach (KeyValuePair<Dispatcher, object?> kvp in Dispatchers)
            {
                if (kvp.Value is VesselUIWindow window)
                    kvp.Key.BeginInvoke(() => { if (!WindowHelpers.IsWindowVisible(window)) window.Activate(); });
            }
        }

        private void SardineWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(SardineLoadedAction);
        }

        private static DispatcherOperation OnApplicationClosingDispatcherInvocation(Dispatcher myDispatcher, VesselUIWindow window) => myDispatcher.BeginInvoke(() => window.Close());
    }
}
