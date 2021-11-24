using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace NetDownReporter.Stores
{
    public sealed class NetStatusIns
    {
        private static NetStatusIns instance = null;
        private static readonly object InstanceLock = new object();

        /// <summary>
        /// To Do
        /// Get exact Network Adapter decription 
        /// </summary>
        private readonly String Adapter = "Software Loopback Interface"; //Cisco Anyconnect Secure Mobility

        public event Action TempoNetworkStatusChanged;
        public String connection;
        public SolidColorBrush mySolidColorBrush;
        public DateTime netOutTimeStamp;
        public DateTime netInTimeStamp;
        public String netOutTimeString;
        public String netInTimeString;
        public TimeSpan duration;
        public String durationInSeconds;
        public String openingText;
        public String userName;
        public String pSID;
        public bool firstLoad;
        public string test;

        //Create a static instance of NetStatusIns class
        public static NetStatusIns GetNetStatus
        {
            get
            {
                //Check if an instance of static NetStatusIns class is already available
                if (instance == null)
                {
                    //If instance already available, create a lock to mitigate multi thread instantiation. Else return a new instance of NetStatusIns
                    lock (InstanceLock)
                    {
                        if (instance == null)
                            instance = new NetStatusIns();
                        return instance;
                    }
                }
                return instance; //Return a singleton NetStatusIns class if it has not already instantiated previously 
            }
        }

        //Private methods for NetStatusIns 
        private NetStatusIns()
        {
            //Set first load to True on the initial load of the NetStatusIns class
            firstLoad = true;

            //Create a new event handler to monitor network availability changes
            NetworkChange.NetworkAvailabilityChanged += new NetworkAvailabilityChangedEventHandler(NetworkChange_NetworkAvailabilityChanged);

            //Check if user is connected to the internet at startup & pass connection status to the viewmodel to update the view
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface Interface in interfaces)
                {
                    // Check if connected to VPN
                    if (Interface.Description.Contains(Adapter)
                      && Interface.OperationalStatus == OperationalStatus.Up)
                    {
                        netInTimeStamp = DateTime.Now;
                        netInTimeString = DateTime.Now.ToString("hh:mm:ss");
                        connection = "Connected to the VPN";
                        mySolidColorBrush = new SolidColorBrush(Colors.Green);
                        mySolidColorBrush.Freeze();
                        test = Interface.Name;
                    }
                    else
                    {
                        connection = "Not Connected to the VPN";
                        mySolidColorBrush = new SolidColorBrush(Colors.Orange);
                        mySolidColorBrush.Freeze();
                    }
                }

            }

            //If user is not connected to the internet at startup, change connection status text from view model
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                connection = "No Internet on load";
                mySolidColorBrush = new SolidColorBrush(Colors.Red);
                mySolidColorBrush.Freeze();

                //Make the firstLoad bool as false at the startup to indicate that network was out when the app started
                firstLoad = false;
            }

            //Capture user details on load
            getUsername();

        }

        // Funtionalities when network connected and disconnected
        public void NetworkChange_NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            if (e.IsAvailable)
            {
                //Get all network instances available in the system
                NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface Interface in interfaces)
                {
                    // Check if connected to the VPN
                    if (Interface.Description.Contains(Adapter)
                      && Interface.OperationalStatus == OperationalStatus.Up)
                    {
                        netInTimeStamp = DateTime.Now;
                        netInTimeString = DateTime.Now.ToString("hh:mm:ss");
                        connection = "Connected to the VPN";
                        duration = netInTimeStamp - netOutTimeStamp;

                        if (duration > TimeSpan.FromSeconds(0) & firstLoad)
                        {
                            durationInSeconds = Math.Floor(duration.TotalSeconds).ToString() + " Seconds";
                        }
                        if (duration < TimeSpan.FromSeconds(0))
                        {
                            durationInSeconds = "";
                        }
                        mySolidColorBrush = new SolidColorBrush(Colors.Green);
                        mySolidColorBrush.Freeze();

                        if(!firstLoad)
                        {
                            firstLoad = true; //Make firstLoad as false again to indicate that it is not the first time the application loads the class
                        }

                        OnTempNetworkStatusChanged(e);
                    }
                    //If not connected to the VPN but still connected to the internet
                    else
                    {
                        connection = "Not Connected to the VPN";
                        mySolidColorBrush = new SolidColorBrush(Colors.Orange);
                        mySolidColorBrush.Freeze();
                        OnTempNetworkStatusChanged(e);
                    }
                }
            }
            else
            {
                netOutTimeStamp = DateTime.Now;
                netOutTimeString = DateTime.Now.ToString("hh:mm:ss");
                connection = "Not Connected to the Internet";
                durationInSeconds = "";
                netInTimeString = "";
                mySolidColorBrush = new SolidColorBrush(Colors.Red);
                mySolidColorBrush.Freeze();
                OnTempNetworkStatusChanged(e);
            }
        }

        // Invoke temporary network change event to notify subscribers
        private void OnTempNetworkStatusChanged(EventArgs e)
        {
            TempoNetworkStatusChanged?.Invoke();
        }

        /// <summary>
        /// To Do
        /// Get user information (username & PSID)
        /// </summary>
        private void getUsername()
        {
            userName = Environment.UserName; 
            pSID = Environment.UserName;
        }
    }
}
