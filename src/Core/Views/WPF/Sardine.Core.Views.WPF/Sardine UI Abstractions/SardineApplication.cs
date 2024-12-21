using Sardine.Core.Logs;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.IO;
using Sardine.Core.Views.WPF.LayoutManagement;
using Sardine.Core.Utils.Reflection;

[assembly: CLSCompliant(true)]

namespace Sardine.Core.Views.WPF
{
    public abstract class SardineApplication<T> : Application, IDisposable where T : Fleet, new()
    {
        private readonly List<SardineWindow> sardineWindows = [];
        private readonly ObservableTraceListener observableTraceListener = new();
        private readonly object exitLock = new();
        private Thread? exitThread;
        private bool exitStatusOn;
        private bool _disposed;


        protected SardineApplication()
        {
            Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            observableTraceListener.OnTrace += Otl_OnTrace;
            PresentationTraceSources.DataBindingSource.Listeners.Add(observableTraceListener);

            Start();
        }

        public void Start()
        {
            Fleet.Start<T>();
            Fleet.Current.Log($"Base location set to {SardineInfo.BaseLocation}");

            Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            Type[] windowTypes = SardineApplication<T>.GetWindowType();

            Startup += (_, _) => ApplicationStartup(windowTypes);
        }

        public void Kill()
        {
            Fleet.Current.Dispose();
            Dispose();
            Environment.Exit(0);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
                observableTraceListener.Dispose();

            _disposed = true;
        }

        private void Otl_OnTrace(object? sender, OnTraceEventArgs e)
        {
            LogMessage? message = PresentationTraceHandler.Instance.Handle(e.Trace);
            
            if (message is not null)
                new Task(()=> Fleet.Current.Logger?.Log(message, sender)).Start();
        }

        private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            bool result = TryToHandleUnhandledExceptionFromVessel(e.Exception);

            if (result)
                e.Handled = true;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (!TryToHandleUnhandledExceptionFromVessel((Exception)e.ExceptionObject))
            {
                if (e.ExceptionObject is Exception exception)
                {
                    _ = MessageBox.Show(
                        exception.Message + exception.StackTrace,
                        "Fatal Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error,
                        MessageBoxResult.None,
                        MessageBoxOptions.ServiceNotification);
                    Dispose();
                    Environment.Exit(-1);
                }
            }
        }
        
        private void ApplicationStartup(Type[] windowTypes)
        {
            FleetLoadingWindow dialog = new();
            dialog.Closed += (_, _) => MainWindowStartup(windowTypes);
            dialog.Topmost = true;
            dialog.Show();
            dialog.Load();
        }

        private void MainWindowStartup(Type[] windowTypes)
        {
            SardineMainWindow? mainWindow = null;

            if (SardineApplicationSettings.GenerateSardineWindow)
            {
                mainWindow = new SardineMainWindow();    
                mainWindow.Closed += (_, _) => Dispatcher.Invoke(() => { ApplicationExit(); });
                sardineWindows.Add(mainWindow);
            }

            string[] windowsToOpen = Fleet.Current.SettingsProvider.FetchSettings("Application", "Layout", "Window", "Type").Select(x => x.Value).ToArray();

            foreach (Type type in windowTypes)
            {
                if (windowsToOpen.Contains(type.Name))
                {
                    SardineWindow? windowObject = (SardineWindow?)(Activator.CreateInstance(type));

                    if (windowObject is not null)
                    {
                        if (string.IsNullOrEmpty(windowObject.Title))
                            windowObject.Title = SardineInfo.FleetName;

                        windowObject.Closed += (_, _) => Dispatcher.Invoke(() => ApplicationExit());

                        sardineWindows.Add(windowObject);
                    }
                }
            }

            if (sardineWindows.Count == 0)
            {
                Environment.Exit(-1);
                return;
            }

            Current.MainWindow = sardineWindows.First();

            foreach (SardineWindow window in sardineWindows)
            {
                window.Show();
                window.Activate();
            }

            if (Fleet.Current.SettingsProvider.FetchSetting("Application","Layout","Default")?.Value is string defaultLayoutName)
            {
                if (File.Exists(Path.Combine(SardineInfo.BaseLocation,defaultLayoutName)))
                {
                    if (SardineApplicationSettings.GenerateSardineWindow && mainWindow is not null)
                    {
                        using (FileStream stream = File.OpenRead(Path.Combine(SardineInfo.BaseLocation, defaultLayoutName)))
                        {
                            VesselUIPersistenceService.Load(mainWindow, stream);
                        }
                    }
                }
            }
        }

        private void ApplicationExit()
        {
            lock (exitLock)
            {
                if (!exitStatusOn)
                {
                    exitStatusOn = true;

                    foreach(SardineWindow window in sardineWindows)
                        window.Close();

                    ExitWindow exitWindow = new();
                    exitWindow.Loaded += ExitWindow_Loaded;
                    exitWindow.Topmost = true;
                    exitThread = new Thread(new ThreadStart(Kill));

                    exitWindow.Show();
                }
            }
        }

        private void ExitWindow_Loaded(object sender, RoutedEventArgs e)
        {
            exitThread?.Start();
        }

        private static bool TryToHandleUnhandledExceptionFromVessel(Exception exception)
        {
            StackTrace trace = new(exception);

            foreach (StackFrame frame in trace.GetFrames())
            {
                Type? type = frame.GetMethod()?.DeclaringType;

                if (type is not null && Fleet.Current is not null && Fleet.Current.VesselCollection.Select(x => x.HandleType).Contains(type))
                {
                    List<Vessel> vmaList = Fleet.Current.VesselCollection.Where(x => x.HandleType == type).ToList();

                    if (vmaList.Count == 1)
                    {
                        vmaList[0].UnhandledVesselExceptionHandler(exception);
                        return true;
                    }

                }
            }

            return false;
        }
        private static Type[] GetWindowType()
        {
            return Assembly.GetEntryAssembly()!.DefinedTypes.Where((x) => x.InheritsFrom(typeof(SardineWindow))).ToArray();
        }
    }

}
