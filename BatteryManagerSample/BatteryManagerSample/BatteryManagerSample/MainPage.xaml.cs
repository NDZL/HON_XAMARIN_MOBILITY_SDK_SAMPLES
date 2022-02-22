using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Android.OS;
using Honeywell.AIDC.Droid.OS;

namespace BatteryManagerSample
{
    public partial class MainPage : ContentPage
    {
        private const string PickerItemAll = "All";
        private static Honeywell.AIDC.Droid.OS.BatteryManager CurrentBatteryManager = null;
        public static MainPage CurrentMainPage { get; set; } = null;

        public MainPage()
        {
            InitializeComponent();
            CurrentMainPage = this;
            PopulateBatteryInfoKeyPicker();
            mBatteryInfoKeyPicker.SelectedIndex = 0;
            UpdateUI(false);
        }

        public static async Task<bool> GetBatteryManager()
        {
            bool status = false;
            try
            {
                if (null == CurrentBatteryManager)
                {
                    CurrentBatteryManager = await Honeywell.AIDC.Droid.OS.BatteryManager.GetInstance();
                }
                if (CurrentBatteryManager != null)
                {
                    status = true;
                }
            }
            catch (HonOSException ex)
            {
                CurrentMainPage.DisplayStatus("Failed to create BatteryManager object, exception: " + ex.Message);
            }

            return status;
        }

        public void ReleaseBatteryManager()
        {
            if (CurrentBatteryManager != null)
            {
                CurrentBatteryManager.Dispose();
                CurrentBatteryManager = null;
            }
        }

        /// <summary>
        /// Update the UI based on whether an instance of BatteryManager exists.
        /// </summary>
        /// <param name="batteryMgrCreated"></param>
        public void UpdateUI(bool batteryMgrCreated)
        {
            mBatteryInfoKeyPicker.IsEnabled = batteryMgrCreated;
            mButtonGetBatteryInfo.IsEnabled = batteryMgrCreated;
            mButtonClear.IsEnabled = batteryMgrCreated;
        }

        private void PopulateBatteryInfoKeyPicker()
        {
            mBatteryInfoKeyPicker.Items.Add(PickerItemAll);
            mBatteryInfoKeyPicker.Items.Add(Honeywell.AIDC.Droid.OS.BatteryManager.BatteryInfoKey.AgeCapacity);
            mBatteryInfoKeyPicker.Items.Add(Honeywell.AIDC.Droid.OS.BatteryManager.BatteryInfoKey.AgeForecast);
            mBatteryInfoKeyPicker.Items.Add(Honeywell.AIDC.Droid.OS.BatteryManager.BatteryInfoKey.AgeTime);
            mBatteryInfoKeyPicker.Items.Add(Honeywell.AIDC.Droid.OS.BatteryManager.BatteryInfoKey.Authentication);
            mBatteryInfoKeyPicker.Items.Add(Honeywell.AIDC.Droid.OS.BatteryManager.BatteryInfoKey.Current);
            mBatteryInfoKeyPicker.Items.Add(Honeywell.AIDC.Droid.OS.BatteryManager.BatteryInfoKey.CycleCount);
            mBatteryInfoKeyPicker.Items.Add(Honeywell.AIDC.Droid.OS.BatteryManager.BatteryInfoKey.FullCapacity);
            mBatteryInfoKeyPicker.Items.Add(Honeywell.AIDC.Droid.OS.BatteryManager.BatteryInfoKey.FullCapacityCompensated);
            mBatteryInfoKeyPicker.Items.Add(Honeywell.AIDC.Droid.OS.BatteryManager.BatteryInfoKey.FullCapacityNotCompensated);
            mBatteryInfoKeyPicker.Items.Add(Honeywell.AIDC.Droid.OS.BatteryManager.BatteryInfoKey.MaxCurrent);
            mBatteryInfoKeyPicker.Items.Add(Honeywell.AIDC.Droid.OS.BatteryManager.BatteryInfoKey.MaxTemp);
            mBatteryInfoKeyPicker.Items.Add(Honeywell.AIDC.Droid.OS.BatteryManager.BatteryInfoKey.MaxVoltage);
            mBatteryInfoKeyPicker.Items.Add(Honeywell.AIDC.Droid.OS.BatteryManager.BatteryInfoKey.MinCurrent);
            mBatteryInfoKeyPicker.Items.Add(Honeywell.AIDC.Droid.OS.BatteryManager.BatteryInfoKey.MinTemp);
            mBatteryInfoKeyPicker.Items.Add(Honeywell.AIDC.Droid.OS.BatteryManager.BatteryInfoKey.MinVoltage);
            mBatteryInfoKeyPicker.Items.Add(Honeywell.AIDC.Droid.OS.BatteryManager.BatteryInfoKey.RemainingCapacity);
            mBatteryInfoKeyPicker.Items.Add(Honeywell.AIDC.Droid.OS.BatteryManager.BatteryInfoKey.SerialNumber);
            mBatteryInfoKeyPicker.Items.Add(Honeywell.AIDC.Droid.OS.BatteryManager.BatteryInfoKey.StateOfCharge);
            mBatteryInfoKeyPicker.Items.Add(Honeywell.AIDC.Droid.OS.BatteryManager.BatteryInfoKey.Temperature);
            mBatteryInfoKeyPicker.Items.Add(Honeywell.AIDC.Droid.OS.BatteryManager.BatteryInfoKey.TimeToEmpty);
            mBatteryInfoKeyPicker.Items.Add(Honeywell.AIDC.Droid.OS.BatteryManager.BatteryInfoKey.TimeToFull);
            mBatteryInfoKeyPicker.Items.Add(Honeywell.AIDC.Droid.OS.BatteryManager.BatteryInfoKey.Voltage);
        }

        private void ButtonClear_Clicked(object sender, EventArgs e)
        {
            mEditorStatus.Text = string.Empty;
        }

        private void ButtonGetBatteryInfo_Clicked(object sender, EventArgs e)
        {
            string selectedKey = mBatteryInfoKeyPicker.Items[mBatteryInfoKeyPicker.SelectedIndex];
            switch(selectedKey)
            {
                case PickerItemAll:
                    if (CurrentBatteryManager != null)
                    {
                        try
                        {
                            Bundle bundle = CurrentBatteryManager.GetBatteryGaugeInfo();
                            if (bundle != null)
                            {
                                foreach (string key in bundle.KeySet())
                                {
                                    string infoStr = bundle.GetString(key);
                                    DisplayBatteryInfo(key, infoStr);
                                }
                            }
                            else
                            {
                                DisplayStatus("BatteryManager.GetBatteryGaugeInfo returns null");
                            }
                        }
                        catch (HonOSException ex)
                        {
                            DisplayStatus("Failed to get all battery info, exception: " + ex.Message);
                        }
                    }
                    else
                    {
                        DisplayStatus("The BatteryManager object has not been created.");
                    }
                    break;
                default:
                    try
                    {
                        Bundle bundle = CurrentBatteryManager.GetBatteryGaugeInfo(selectedKey);
                        if (bundle != null)
                        {
                             string infoStr = bundle.GetString(selectedKey);
                             DisplayBatteryInfo(selectedKey, infoStr);
                        }
                        else
                        {
                            DisplayStatus("GetBatteryGaugeInfo for key: " + selectedKey + " returns null.");
                        }
                    }
                    catch (HonOSException ex)
                    {
                        DisplayStatus("Failed to get battery info of " + selectedKey + ", exception: " + ex.Message);
                    }
                    break;
            } //endswitch
        }

        private void DisplayStatus(string msg)
        {
            mEditorStatus.Text += System.Environment.NewLine + msg;
        }

        private void DisplayBatteryInfo(string key, string value)
        {
            String msg = String.Format("{0}key: {1} value: {2}",
                System.Environment.NewLine, key, (value != null) ? value : "null");
            mEditorStatus.Text += msg;
        }

        /// <summary>
        /// This method is invoked when the user clicks the action overflow icon
        /// (with 3 vertical dots) on the right side of the app bar and select
        /// "Battery Hot-swap" menu item.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An object containging the event data.</param>
        private async void OnBatteryHotSwap(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BatteryHotSwapPage());
        }
    }
}
