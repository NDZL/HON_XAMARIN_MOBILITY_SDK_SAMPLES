using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Honeywell.AIDC.Droid.OS;

namespace PeripheralManagerSample
{
    public partial class MainPage : ContentPage
    {
        private static Honeywell.AIDC.Droid.OS.PeripheralManager CurrentPeripheralManager = null;
        public static MainPage CurrentMainPage { get; set; } = null;

        public MainPage()
        {
            InitializeComponent();
            CurrentMainPage = this;
        }

        public static async Task<bool> GetPeripheralManager()
        {
            bool status = false;
            try
            {
                if (null == CurrentPeripheralManager)
                {
                    CurrentPeripheralManager = await Honeywell.AIDC.Droid.OS.PeripheralManager.GetInstance();
                }
                if (CurrentPeripheralManager != null)
                {
                    status = true;
                }
            }
            catch (HonOSException ex)
            {
                CurrentMainPage.DisplayStatus("Failed to create PeripheralManager object, exception: " + ex.Message);
            }

            return status;
        }

        public void ReleasePeripheralManager()
        {
            if (CurrentPeripheralManager != null)
            {
                CurrentPeripheralManager.Dispose();
                CurrentPeripheralManager = null;
            }
        }

        /// <summary>
        /// Update the UI based on whether an instance of PeripheralManager exists.
        /// </summary>
        /// <param name="peripheralMgrCreated">True indicates an instance of PeripheralManager exists.</param>
        public void UpdateUI(bool peripheralMgrCreated)
        {
            mButtonDockType.IsEnabled = peripheralMgrCreated;
            mButtonSerialInfo.IsEnabled = peripheralMgrCreated;
        }

        private void DisplayStatus(string msg)
        {
            mEditorStatus.Text += System.Environment.NewLine + msg;
        }

        private void ButtonDockType_Clicked(object sender, EventArgs e)
        {
            if (CurrentPeripheralManager != null)
            {
                try
                {
                    string msg = String.Empty;
                    string dockTypeStr = CurrentPeripheralManager.DeviceDockType;
                    switch (dockTypeStr)
                    {
                        case PeripheralManager.DockType.CN80_CN85_Client:
                            msg = "CN80/CN85 client dock";
                            break;
                        case PeripheralManager.DockType.CN80_CN85_HomeBase:
                            msg = "CN80/CN85 home base with or without ethernet";
                            break;
                        case PeripheralManager.DockType.CN80_CN85_NetBase:
                            msg = "CN80/CN85 net base dock";
                            break;
                        case PeripheralManager.DockType.CN80_CN85_Wired30Bay:
                            msg = "CN80/CN85 wired 30-bay dock";
                            break;
                        case PeripheralManager.DockType.CN80_CN85_WiredVehicle:
                            msg = "CN80/CN85 wired vehicle dock";
                            break;
                        case PeripheralManager.DockType.CN80_CN85_Wireless30Bay:
                            msg = "CN80/CN85 wireless 30-bay dock";
                            break;
                        case PeripheralManager.DockType.CN80_CN85_WirelessVehicle:
                            msg = "CN80/CN85 wireless vehicle dock";
                            break;
                        case PeripheralManager.DockType.CT60_Debug:
                            msg = "CT60 debug dock";
                            break;
                        case PeripheralManager.DockType.CT60_HomeBase:
                            msg = "CT60 home base with or without ethernet";
                            break;
                        case PeripheralManager.DockType.CT60_Printer:
                            msg = "CT60 printer dock";
                            break;
                        case PeripheralManager.DockType.CT60_ScanHandle:
                            msg = "CT60 scan handle";
                            break;
                        case PeripheralManager.DockType.CT60_Vehicle:
                            msg = "CT60 vehicle dock";
                            break;
                        case PeripheralManager.DockType.DockNotConnected:
                            msg = "Not connected to a dock";
                            break;
                        case PeripheralManager.DockType.VM1A_QM1:
                            msg = "VM1A QM1 dock";
                            break;
                        case PeripheralManager.DockType.VM1A_QM3:
                            msg = "VM1A QM3 dock";
                            break;
                        default:
                            msg = "Unknown dock type";
                            break;
                    } //endswitch

                    DisplayStatus("Dock type: " + dockTypeStr + ", " + msg);
                }
                catch (HonOSException ex)
                {
                    DisplayStatus("Failed to get the device dock type, exception: " + ex.Message);
                }
            }
            else
            {
                DisplayStatus("The PeripheralManager object has not been created.");
            }
        } //endof ButtonDockType_Clicked

        private void ButtonSerialInfo_Clicked(object sender, EventArgs e)
        {
            if (CurrentPeripheralManager != null)
            {
                try
                {
                    string[] serialInfoArray = CurrentPeripheralManager.SerialPortInfo;
                    if (serialInfoArray.Length > 0)
                    {
                        DisplayStatus("Serial ports:");
                        foreach (string s in serialInfoArray)
                        {
                            DisplayStatus(s);
                        }
                    }
                    else
                    {
                        DisplayStatus("No serial ports found.");
                    }
                }
                catch (HonOSException ex)
                {
                    DisplayStatus("Failed to get serial port info, exception: " + ex.Message);
                }
            }
            else
            {
                DisplayStatus("The PeripheralManager object has not been created.");
            }
        }

        private void ButtonClear_Clicked(object sender, EventArgs e)
        {
            mEditorStatus.Text = string.Empty;
        }
    }
}
