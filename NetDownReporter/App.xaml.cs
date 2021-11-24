using Microsoft.Toolkit.Uwp.Notifications;
using NetDownReporter.Stores;
using NetDownReporter.ViewModels;
using System;
using System.Drawing;
using System.Windows;
using Forms = System.Windows.Forms;

namespace NetDownReporter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly Forms.NotifyIcon _notifyIcon;

        //Instantiate NetStatusIns class
        private readonly NetStatusIns NetStatusInstance;

        public App()
        {
            _notifyIcon = new Forms.NotifyIcon();

            //Instantiate the NetStatusIns singleton class
            NetStatusInstance = NetStatusIns.GetNetStatus;            

        }

        protected override void OnStartup(StartupEventArgs e)
        {
            //Instatiate Navigation Store
            NavigationStore _navigationStore = new NavigationStore();

            //Set current viewmodel to the home viewmodel
            _navigationStore.CurrentViewModel = new HomeViewModel(_navigationStore, _notifyIcon);

            MainWindow = new MainWindow()
            {
                DataContext = new MainViewModel(_navigationStore)
            };
            
            //Set up NotifyIcon
            _notifyIcon.Icon = new System.Drawing.Icon("Resources/icon.ico");
            _notifyIcon.Text = "Net Down Reporter";
            _notifyIcon.Click += NotifyIcon_Click;

            _notifyIcon.ContextMenuStrip = new Forms.ContextMenuStrip();
            _notifyIcon.ContextMenuStrip.Items.Add("Status", Image.FromFile("Resources/icon.ico"));
            _notifyIcon.BalloonTipClicked += _notifyIcon_BalloonTipClicked;

            _notifyIcon.Visible = true;

            MainWindow.Show();
            base.OnStartup(e);
        }

        //Set up notifyIcon baloon click events
        private void _notifyIcon_BalloonTipClicked(object sender, EventArgs e)
        {
            MainWindow.WindowState = WindowState.Normal;
            MainWindow.Activate();
        }

        //Set up notifyIcon click events
        private void NotifyIcon_Click(object sender, EventArgs e)
        {
            MainWindow.WindowState = WindowState.Normal;
            MainWindow.Activate(); 
        }

        //Dispose NotifyIcon on app exit
        protected override void OnExit(ExitEventArgs e)
        {
            _notifyIcon.Dispose();
            base.OnExit(e);
        }
    }
}
