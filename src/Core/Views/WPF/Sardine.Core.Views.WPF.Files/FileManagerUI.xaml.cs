using Microsoft.Win32;
using Sardine.Core.IO.Files;
using Sardine.Core.Logs;
using Sardine.Core.Views.WPF;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

using Path = System.IO.Path;

namespace Sardine.Core.Views.Files.WPF
{
    public partial class FileManagerUI : UserControl, INotifyPropertyChanged
    {
        private IReadOnlyList<IO.Files.FileInfo> files = [];
        private IList<IO.Files.FileInfo> filteredFileList = []; private readonly SardineWindow uiManager;
        private IO.Files.FileInfo? selectedFile;
        private string filter = string.Empty;

        public IO.Files.FileInfo? SelectedFile
        {
            get => selectedFile;
            set
            {
                selectedFile = value;
                OnPropertyChanged();
            }
        }

        public IReadOnlyList<IO.Files.FileInfo> Files
        {
            get => files; private set
            {
                files = value;
                UpdateFilteredList();
            }
        }

        public string Filter
        {
            get => filter; set
            {
                filter = value;
                OnPropertyChanged();
                UpdateFilteredList();
            }
        }

        private void UpdateFilteredList()
        {
            FilteredFileList = Files.Where(x => x.Name.Contains(Filter, StringComparison.InvariantCulture)).ToList();
        }

#pragma warning disable CA2227 // Collection properties should be read only
        public IList<IO.Files.FileInfo> FilteredFileList
#pragma warning restore CA2227 // Collection properties should be read only
        {
            get => filteredFileList; set
            {
                filteredFileList = value;
                OnPropertyChanged();
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;


        

        public FileManagerUI(SardineWindow uim)
        {
            uiManager = uim;
            InitializeComponent();
            DataContext = this;
            ReloadFileList();
        }

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void ReloadFileList()
        {
            Fleet.Current.Get<FileProvider>().ReloadFileList();
            Files = Fleet.Current.Get<FileProvider>().FileList;
            foreach (var file in Files)
            {
                file.SardineFileTypeFound += (_, _) =>
                UpdateFilteredList();
            }
        }

        private void Button_Reload_Click(object sender, RoutedEventArgs e)
        {
            ReloadFileList();
        }

        private void Button_Export_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedFile?.Location is null)
                return;

            SaveFileDialog dialog = new();
            if (dialog.ShowDialog() ?? false)
                File.Copy(SelectedFile.Location, dialog.FileName);
        }
        private void Button_Import_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new()
            {
                Multiselect = false
            };

            if (dialog.ShowDialog() ?? false)
                File.Copy(dialog.FileName, Path.Combine(Core.SardineInfo.BaseLocation, Path.GetFileName(dialog.FileName)), true);
            
            ReloadFileList();
        }

        private void Button_OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", Core.SardineInfo.BaseLocation);
        }

        private void TextBlock_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed && e.ClickCount == 2)
            {
#pragma warning disable CA1031 // Do not catch general exception types
                try
                {
                    ((IO.Files.FileInfo)((TextBlock)sender).DataContext).Handle(uiManager);
                }
                catch (Exception ex)
                {
                    Fleet.Current.Log(new LogMessage($"File handling failed: {ex.Message}", LogLevel.Warning));
                }
#pragma warning restore CA1031 // Do not catch general exception types
            }
        }
    }
}
