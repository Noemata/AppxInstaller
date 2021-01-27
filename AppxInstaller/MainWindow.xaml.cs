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
        const string HelpMessage = "Install Appx from:";
        const string BundleName = "SimpleApp_1.0.0.0_x64.msixbundle";
        const string CertificateName = "SimpleApp_1.0.0.0_x64.cer";

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
            Setup = new ProductSetup(ProductName, ProductVersion, BundleName, CertificateName);
            Setup.InUiThread = this.InUiThread;

            DataContext = Setup;

            Setup.InstallDirectory = AppxBundle.GetAppxFolder();
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
            string dlgMessage = HelpMessage + (Setup.InstallDirectory != null ? $"\n{Setup.InstallDirectory}" : "");
            var dlg = new MessageDialog(dlgMessage, "Instructions:");
            this.InitializeWinRTChild(dlg);
            await dlg.ShowAsync();
        }

        // MP! bug: this call generates an exception when on an elevated account ???
        private async void OnSelectFolder(object sender, RoutedEventArgs e)
        {
            var folderPicker = new FolderPicker();
            this.InitializeWinRTChild(folderPicker);

            folderPicker.SuggestedStartLocation = PickerLocationId.Unspecified;
            folderPicker.FileTypeFilter.Add("*");
            var folder = await folderPicker.PickSingleFolderAsync();

            if (folder != null)
            {
                Setup.InstallDirectory = folder.Path;
            }
        }
    }
}
