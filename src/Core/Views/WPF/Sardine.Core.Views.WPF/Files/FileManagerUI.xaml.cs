using Microsoft.Win32;
using Sardine.Core.FileManagement;
using Sardine.Core.Logs;
using Sardine.Core.Views.WPF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using FileInfo = Sardine.Core.FileManagement.FileInfo;
using Path = System.IO.Path;

namespace Sardine.Core.Views.Files.WPF
{
    public partial class FileManagerUI : UserControl, INotifyPropertyChanged
    {
        private IReadOnlyList<FileInfo> files = [];
        private IList<FileInfo> filteredFileList = []; private readonly SardineWindow uiManager;
        private FileInfo? selectedFile;
        private string filter = string.Empty;

        public FileInfo? SelectedFile
        {
            get => selectedFile;
            set
            {
                selectedFile = value;
                OnPropertyChanged();
            }
        }

        public IReadOnlyList<FileInfo> Files
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
        public IList<FileInfo> FilteredFileList
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
                    ((FileInfo)((TextBlock)sender).DataContext).Handle(uiManager);
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
