using System;
using System.Windows;
using System.Diagnostics;
using System.Windows.Threading;

using Windows.UI.Popups;
using Windows.Storage.Pickers;

using WinRT.InitializeWithWindow;

namespace AppxInstaller
{
    // inetcpl.cpl

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ProductSetup Setup { get; set; }

        const string ProductName = "SimpleApp";
        const string ProductVersion = "version 1.00";
        const string HelpMessage = "Select the Broadcast Explorer Project folder as your installation directory.";
        const string ContainerFolder = "BroadcastProjects";

        public MainWindow()
        {
            InitializeComponent();
        }

        public void InUiThread(Action action)
        {
            if (this.Dispatcher.CheckAccess())
                action();
            else
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, action);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Setup = new ProductSetup(ProductName, ProductVersion);
            Setup.InUiThread = this.InUiThread;

            DataContext = Setup;
        }

        private void OnDrag(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void OnMinimize(object sender, RoutedEventArgs e)
        {
            WindowState = System.Windows.WindowState.Minimized;
        }

        private void OnClose(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnInstall(object sender, RoutedEventArgs e)
        {
            Setup.StartInstall();
        }

        private void OnRepare(object sender, RoutedEventArgs e)
        {
            //Setup.StartRepair();
        }

        private void OnUninstall(object sender, RoutedEventArgs e)
        {
            Setup.StartUninstall();
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            Setup.StartCancel();
        }

        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //e.Cancel = Setup.IsRunning;
        }

        private async void OnHelp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            string dlgMessage = HelpMessage + (ContainerFolder != null ? $"\nExample -> D:\\Projects\\{ContainerFolder}" : "");
            var dlg = new MessageDialog(dlgMessage, "Instructions:");
            this.InitializeWinRTChild(dlg);
            await dlg.ShowAsync();
        }

        private async void OnSelectFolder(object sender, RoutedEventArgs e)
        {
            var folderPicker = new FolderPicker();
            this.InitializeWinRTChild(folderPicker);

            folderPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            folderPicker.FileTypeFilter.Add("*");
            var folder = await folderPicker.PickSingleFolderAsync();

            if (folder != null)
            {
                Setup.InstallDirectory = folder.Path;
            }
        }
    }
}
