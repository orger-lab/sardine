using Microsoft.Win32;
using Sardine.Core.Logs;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Linq;

namespace Sardine.Core.Views.Logs.WPF
{
    /// <summary>
    /// Interaction logic for LoggerUI.xaml
    /// </summary>
    public partial class LoggerUI : UserControl
    {
        public ObservableCollection<LogMessage> LogMessages { get; } = [];

        public LoggerUI()
        {
            InitializeComponent();
            DataContext = this;
            ComboBox_LoggerLevel.ItemsSource = Enum.GetValues(typeof(LogLevel)).Cast<LogLevel>();
            ComboBox_LoggerLevel.SelectedItem = LogLevel.Debug;

            foreach (LogMessage message in Fleet.Current.Logger!.LogHistory)
                LogMessages.Add(message);

            Fleet.Current.Logger.OnNewLogMessage += Instance_OnNewLogMessage;
            Loaded += LoggerUI_Loaded;

        }

        private void LoggerUI_Loaded(object sender, RoutedEventArgs e)
        {
            Border border = (Border)VisualTreeHelper.GetChild(ListBox_LogEntries, 0);
            ScrollViewer scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
            scrollViewer.ScrollToBottom();
        }

        private void Instance_OnNewLogMessage(object? sender, OnNewLogMessageEventArgs e)
        {
            
            Dispatcher.BeginInvoke(()=>
            {
                if ((int)e.LogMessage.Level <= (int)ComboBox_LoggerLevel.SelectedItem)
                {
                    LogMessages.Add(e.LogMessage);
                    if (VisualTreeHelper.GetChildrenCount(ListBox_LogEntries) > 0)
                    {
                        Border border = (Border)VisualTreeHelper.GetChild(ListBox_LogEntries, 0);
                        ScrollViewer scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
                        if (scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight)
                            scrollViewer.ScrollToBottom();
                    }
                }
            });
        }

        private void Button_Clear_Click(object sender, RoutedEventArgs e)
        {
            LogMessages.Clear();
        }

        private void Button_SaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new()
            {
                FileName = "sardine",
                DefaultExt = ".log",
                Filter = "Log files (.log)|*.log"
            };

            bool? result = dlg.ShowDialog();

            if (result is true)
            {
                using (StreamWriter writer = new(dlg.FileName))
                {
                    foreach(LogMessage message in Fleet.Current.Logger!.LogHistory)
                        writer.WriteLine(message.ToString());
                }
            }
        }

        private void Button_Copy_Click(object sender, RoutedEventArgs e)
        {
            string outputBuilder = string.Empty;

            foreach(LogMessage message in ListBox_LogEntries.SelectedItems)
            {
                outputBuilder += message.ToString();
                outputBuilder += "\r\n";
            }

            Clipboard.SetText(outputBuilder);
        }
    }
}
