using Sardine.Core.Views.WPF.UIHelpers;
using System.Diagnostics;
using System.Windows;

namespace Sardine.Core.Views.Files.WPF
{
    public sealed class FileManagerHelper : IUIHelperProvider
    {
        public string Name { get; } = "File";
        public UIHelperMetadata Metadata => new() { OrderingIndex = -1 };

        public IReadOnlyList<UIHelper> Actions { get; }
        =
        [
            new UIHelper( "Open manager",(window) => new Window()
                                                        {
                                                            Content = new FileManagerUI(window),
                                                            ResizeMode=ResizeMode.NoResize,
                                                            SizeToContent=SizeToContent.WidthAndHeight,
                                                            Title="File Manager"
                                                        }.ShowDialog()),
            new UIHelper("Open folder", () =>  Process.Start("explorer.exe", SardineInfo.BaseLocation)),
        ];
    }
}
