using Microsoft.Win32;
using Sardine.Core.FileManagement;
using Sardine.Core.Logs;
using Sardine.Core.Views.WPF.UIHelpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Controls;

namespace Sardine.Core.Views.Files.WPF
{
    public sealed class FileManagerHelper : IUIHelperProvider
    {
        public string Name { get; } = "File";
        public UIHelperMetadata Metadata => new() { OrderingIndex = -1 };

        public IReadOnlyList<UIHelper> Actions { get; }
        =
        [
            new UIHelper( "Open ...", (window) =>
            {
                OpenFileDialog dialog = new OpenFileDialog() {InitialDirectory = SardineInfo.BaseLocation };
                if (dialog.ShowDialog() ?? false)
                {
                    try
                    {
                        new FileInfo(dialog.FileName).Handle(window);
                    }
                    catch (Exception ex)
                    {
                        Fleet.Current.Log(new LogMessage($"File handling failed: {ex.Message}", LogLevel.Warning));
                    }
                }
            }),
            new UIHelper("Open folder", () =>  Process.Start("explorer.exe", SardineInfo.BaseLocation)),
        ];
    }
}