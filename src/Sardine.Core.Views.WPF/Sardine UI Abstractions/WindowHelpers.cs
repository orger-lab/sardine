using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;

namespace Sardine.Core.Views.WPF
{
    internal static class WindowHelpers
    {

        public static bool IsForeground(Window window, Dispatcher? dispatcher = null)
        {

            IntPtr? windowHandle = null;
            if (dispatcher is null)
                dispatcher = Application.Current.Dispatcher;

            dispatcher.Invoke(() =>
            {
                windowHandle = new WindowInteropHelper(window).Handle;
            });
            IntPtr foregroundWindow = NativeMethods.GetForegroundWindow();
            return windowHandle == foregroundWindow;
        }

        //https://stackoverflow.com/questions/454792/what-is-the-best-way-to-determine-if-a-window-is-actually-visible-in-wpf
        internal static bool IsWindowVisible(Window window)
        {
            WindowInteropHelper win = new(window);
            int x = (int)(window.Left + (window.Width / 2));
            int y = (int)(window.Top + (window.Height / 2));
            System.Drawing.Point p = new System.Drawing.Point(x, y);
            return (win.Handle == NativeMethods.WindowFromPoint(p));
        }
    }
}