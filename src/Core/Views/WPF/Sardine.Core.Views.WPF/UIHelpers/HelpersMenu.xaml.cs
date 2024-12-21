using Sardine.Core.Views.WPF.UIHelpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Sardine.Core.Views.WPF
{
    /// <summary>
    /// Interaction logic for HelpersMenu.xaml
    /// </summary>
    public partial class HelpersMenu : UserControl
    {

        public SardineWindow? SardineWindow { get; set; }

        public HelpersMenu()
        {
            InitializeComponent();
            Loaded += (_, _) => GenerateMenuItems();
            Fleet.Current.FleetLoaded += (_, _) => GenerateMenuItems();
        }

        private void GenerateMenuItems()
        {
            Dictionary<string, MiiValuePair> menuItems = [];

            bool showHelpers = true;
            string? showHelpersValue = Fleet.Current.SettingsProvider.FetchSetting("HelpersMenu", "ShowHelpers")?.Value;
            if (showHelpersValue is not null)
                showHelpers = Convert.ToBoolean(showHelpersValue, CultureInfo.InvariantCulture);

            string[] helpersToShow = Fleet.Current.SettingsProvider.FetchSettings("Application", "HelpersMenu", "Helper", "Name").Select(x => x.Value).ToArray();

            if (showHelpers)
            {
                foreach (IUIHelperProvider helper in Fleet.Current.Get<UIHelperProvidersDictionary>().Values)
                {
                    if (helpersToShow.Length > 0 && !helpersToShow.Contains(helper.Name) && helper.Name != "Fleet")
                        continue;


                    
                    if (!menuItems.TryGetValue(helper.Name, out MiiValuePair? value))
                    {
                        value = new MiiValuePair(new MenuItem(){ Header = helper.Name}, int.MaxValue);
                        menuItems[helper.Name] = value;
                    }

                    MakeHelperMenu(value, helper);

                }

                foreach (MenuItem item in menuItems.Values.OrderBy((x) => x.Index).Where(x => x.Item.Items.Count > 0).Select((x) => x.Item))
                    Menu_FleetManager.Items.Add(item);
            }
        }

        class MiiValuePair(MenuItem item, int index)
        {
            internal MenuItem Item { get; } = item;
            internal int Index { get; set; } = index;
        }

        private void MakeHelperMenu(MiiValuePair miiValuePair, IUIHelperProvider helper)
        {
            //(MenuItem Item, int Index) miiValuePair = (menuItemIn.Item,menuItemIn.Index);

            if (miiValuePair.Item.Items.Count > 0 && (helper.Metadata?.ShowSeparator ?? true))
            {
                Separator separator = new();
                miiValuePair.Item.Items.Add(separator);
            }

            SetHelperProviderMenuMetadata(helper, miiValuePair);

            if (helper.Actions is not null)
            {
                foreach (UIHelper action in helper.Actions)
                {
                    AddActionToHelperMenuItem(action, miiValuePair);
                }
            }
        }

        private void AddActionToHelperMenuItem(UIHelper action, MiiValuePair miiValuePair)
        {
            if (action.Name == "null")
            {
                Separator separator = new();
                miiValuePair.Item.Items.Add(separator);
                return;
            }

            MenuItem subMenuItem = new() { Header = action.Name, Tag = action };
            if (action.Metadata is not null)
            {
                subMenuItem.ToolTip = action.Metadata.Description;
                subMenuItem.Icon = action.Metadata.IconSource;
            }

            subMenuItem.Click += MenuItem_GeneralButton_Click;
            miiValuePair.Item.Items.Add(subMenuItem);
        }

        private static void SetHelperProviderMenuMetadata(IUIHelperProvider helperProvider, MiiValuePair miiValuePair)
        {
            if ((helperProvider.Metadata?.OrderingIndex ?? int.MaxValue) < miiValuePair.Index)
            {
                miiValuePair.Item.ToolTip = helperProvider.Metadata?.Description;
                miiValuePair.Item.Icon = helperProvider.Metadata?.IconSource;
                miiValuePair.Index = helperProvider.Metadata?.OrderingIndex ?? int.MaxValue;
            }
        }

        private void MenuItem_GeneralButton_Click(object sender, RoutedEventArgs e)
        {
            Action<SardineWindow> action = ((UIHelper)((MenuItem)sender).Tag).Run;

            SardineWindow?.InvokeInNewDispatcher((_) => action(SardineWindow), handlerName: ((UIHelper)((MenuItem)sender).Tag).Name);
        }
    }
}
