using Sardine.Core.FileManagement;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Sardine.Core.Views.WPF
{
    /// <summary>
    /// Interaction logic for FilenameDialog.xaml
    /// </summary>
    public partial class SaveSardineFileDialog : Window, INotifyPropertyChanged
    {
        private string? filename = string.Empty;

        public static string? GetPath(string callerName, FileType? fileType)
        {
            if (fileType is null)
                return null;

            SaveSardineFileDialog dialog = new(callerName);
            dialog.ShowDialog();
            string? fileName = dialog.Filename;

            string extension = string.Empty;

            if (fileType.Extension.Count > 0)
                extension = fileType.Extension[0];

            if (File.Exists(Path.Combine(Core.SardineInfo.BaseLocation, fileName ?? string.Empty, extension)))
                if (MessageBox.Show("File already exists. Overwrite?", "File Exists", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                    fileName = null;

            return fileName;
        }

        SaveSardineFileDialog(string callerName)
        {
            InitializeComponent();
            CallerName = callerName;
            DataContext = this;
        }

        public string? Filename
        {
            get => filename; set
            {
                filename = value;
                OnPropertyChanged();
            }
        }

        // FleetAggregator Aggregator { get; }
        public string CallerName { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Filename = null;
            Close();
        }

        private void TextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter && Filename is not null && Filename.Length > 0)
            {
                Close();
            }
        }
    }
}
