using Microsoft.Toolkit.Uwp.Notifications;
using NetDownReporter.Commands;
using NetDownReporter.Stores;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using Forms = System.Windows.Forms;

namespace NetDownReporter.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        //Instantiate NetStatusIns class
        private readonly NetStatusIns NetStatusInstance;

        public ICommand notiCommand { get; }

        private readonly Forms.NotifyIcon _notifyIcon;

        private readonly NotifyIcon tnotifyIcon;

        //Default UI Elements
        public string Version { get; }

        //Reference & Update UI elements
        public string openingText => NetStatusInstance.openingText;
        public string xNetStatus => NetStatusInstance.connection;
        public string xNetIn => NetStatusInstance.netInTimeString;
        public string xNetOut => NetStatusInstance.netOutTimeString;
        public string xNetOutDuration => NetStatusInstance.durationInSeconds;
        public string xUserName => NetStatusInstance.userName;
        public string xPSID => NetStatusInstance.pSID;
        public SolidColorBrush FillColor => NetStatusInstance.mySolidColorBrush;
        public bool firstLoad => NetStatusInstance.firstLoad;

        public string testText => NetStatusInstance.test;

        public HomeViewModel(NavigationStore navigationStore, NotifyIcon notifyIcon)
        {
            //Instantiate the NetStatusIns singleton class
            NetStatusInstance = NetStatusIns.GetNetStatus;

            //Subscribe to TempoNetworkStatusChanged Action
            NetStatusInstance.TempoNetworkStatusChanged += NetStatus_TempNetworkStatusChanged; 

            //_notifyIcon = new Forms.NotifyIcon();

            tnotifyIcon = notifyIcon;

            //Set version
            Version = "v1.0.0";

            //Monitor toast events on the home view
            MonitorToastEvents();
        }

        //observe the subscribed event in NetStatusIns and update UI elements INotifyPropertyChanged 
        private void NetStatus_TempNetworkStatusChanged()
        {
            //Set label text for Network Status 
            OnPropertyChanged(nameof(xNetStatus)); 
            OnPropertyChanged(nameof(testText));

            //If connected to the VPN
            if (xNetStatus == "Connected to the VPN")
            {
                OnPropertyChanged(nameof(xNetIn));
                OnPropertyChanged(nameof(FillColor));
                OnPropertyChanged(nameof(xNetOutDuration));
                OnPropertyChanged(nameof(xNetOut)); 

                //Send outage report 
                //SendOutageReport();
                OutageReportSentSuccesstNotice();
            }

            //If not connected to the VPN
            if (xNetStatus == "Not Connected to the VPN")
            {
                OnPropertyChanged(nameof(xNetIn));
                OnPropertyChanged(nameof(FillColor));
                OnPropertyChanged(nameof(xNetOutDuration));
                OnPropertyChanged(nameof(xNetOut));
            }

            //If not connected to the Internet
            if (xNetStatus == "Not Connected to the Internet")
            {
                OnPropertyChanged(nameof(xNetIn));
                OnPropertyChanged(nameof(FillColor));
                OnPropertyChanged(nameof(xNetOutDuration));
                OnPropertyChanged(nameof(xNetOut));
            }
        }

        private void SendOutageReport()
        {
            //To do
            //Add logic to udate the WFH issue tracker Share Point site through API
        }

        private void OutageReportSentSuccesstNotice()
        {
            tnotifyIcon.ShowBalloonTip(3000, "Network Outage Detected!", "I have submitted an incident in the WFH Issue Tracker for the " + xNetOutDuration + " outage.", ToolTipIcon.Info);
            //new ToastContentBuilder()
            //.AddArgument("action", "viewConversation")
            //.AddArgument("conversationId", 9813)
            //.AddText("Andrew sent you a picture")
            //.AddText("Check this out, The Enchantments in Washington!")
            //.Show();
        }

        private void OutageReportSentFailedNotice()
        {
            _notifyIcon.ShowBalloonTip(3000, "Network Outage Detected", "I will notify the WFH Issue Tracker", ToolTipIcon.Info);
        }

        private void MonitorToastEvents()
        {
            // Listen to notification activation
            ToastNotificationManagerCompat.OnActivated += toastArgs =>
            {
                // Obtain the arguments from the notification
                ToastArguments args = ToastArguments.Parse(toastArgs.Argument);

                // Need to dispatch to UI thread if performing UI operations
                System.Windows.Application.Current.Dispatcher.Invoke(delegate
                {
                    // TODO: Show the corresponding content
                    System.Windows.MessageBox.Show("Toast activated. Args: " + toastArgs.Argument);
                });
            };
        }

    }
}
