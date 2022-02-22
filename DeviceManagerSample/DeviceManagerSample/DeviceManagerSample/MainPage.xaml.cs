using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Honeywell.AIDC.Droid.OS;
using System.Threading;
using Android.OS;

namespace DeviceManagerSample
{
    public partial class MainPage : ContentPage
    {
        public static DeviceManager CurrentDeviceManager { get; set; } = null;
        public static MainPage CurrentMainPage { get; set; } = null;

        private SynchronizationContext mUIContext = SynchronizationContext.Current;
        public MainPage()
        {
            InitializeComponent();
            CurrentMainPage = this;
        }

        private void EnableKeyInputSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            try
            {
                if (null != CurrentDeviceManager)
                {
                    CurrentDeviceManager.KeyInputEnabled = e.Value;
                    DisplayStatus("Key Input Enabled = " + CurrentDeviceManager.KeyInputEnabled.ToString());
                }
            }
            catch (HonOSException exp)
            {
                DisplayStatus("HonOSException: " + exp.Message);
            }
        }

        private void EnableTouchInputSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            try
            {
                if (null != CurrentDeviceManager)
                {
                    CurrentDeviceManager.TouchInputEnabled = e.Value;

                    bool isEnable = CurrentDeviceManager.TouchInputEnabled;

                    //Enable touch input in 5 seconds
                    //after it was disabled.
                    //It is used for testing purpose.
                    if (false == isEnable)
                    {
                        Task.Run(() =>
                        {
                            //disable status on UI thread
                            mUIContext.Post(_ =>
                            {
                                DisplayStatus("Will enable touch input in 5 seconds.");
                            }, null);

                            Thread.Sleep(5000);

                            //enable touch input with UI thread
                            mUIContext.Post(_ => {
                                mEnableTouchInputSwitch.IsToggled = true;
                            }, null);
                        });
                    }

                    DisplayStatus("Touch Input Enabled = " + CurrentDeviceManager.TouchInputEnabled.ToString());
                }
            }
            catch (HonOSException extExp)
            {
                DisplayStatus("HonOSException: " + extExp.Message);
            }
            catch (Exception exp)
            {
                DisplayStatus("Exception: " + exp.Message);
            }
        }

        private void AssetNumberButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (null != CurrentDeviceManager)
                {
                    CurrentDeviceManager.AssetNumber = mAssetNumberEntry.Text;
                    DisplayStatus("Asset number = " + CurrentDeviceManager.AssetNumber);
                }
            }
            catch (HonOSException exp)
            {
                DisplayStatus("HonOSException: " + exp.Message);
            }
        }

        private void HallSensorButton_Clicked(object sender, EventArgs e)
        {
            if (null != CurrentDeviceManager)
            {
                int selectedIndex = mHallSensorPicker.SelectedIndex;
                if (selectedIndex < 0)
                {
                    ShowError("Must select a hall sensor id.");
                    return;
                }

                string selectedId = mHallSensorPicker.Items[selectedIndex];
                GetHallSensorInfo(selectedId);
            }
        }

        private void mTouchProfileButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (null != CurrentDeviceManager)
                {
                    int selectedIndex = mTouchProfilePicker.SelectedIndex;
                    if (selectedIndex < 0)
                    {
                        ShowError("Must select a touch profile.");
                        return;
                    }

                    var touchProfile = mTouchProfilePicker.SelectedItem;
                    CurrentDeviceManager.ActiveTouchProfile = touchProfile.ToString();
                    DisplayStatus("ActiveTouchProfile = " + CurrentDeviceManager.ActiveTouchProfile);
                }
            }
            catch (HonOSException exp)
            {
                DisplayStatus("HonOSException: " + exp.Message);
            }
        }

        private async void WipeDeviceButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (null != CurrentDeviceManager)
                {
                    int selectedIndex = mWipeDevicePicker.SelectedIndex;
                    if (selectedIndex < 0)
                    {
                        ShowError("Must select a wipe type.");
                        return;
                    }

                    var type = mWipeDevicePicker.SelectedItem;
                    bool isYes = await DisplayAlert("Question", "Wipe device data now?", "Yes", "No");
                    if (isYes)
                    {
                        CurrentDeviceManager.WipeDevice((int)type);
                    }
                }
            }
            catch (HonOSException exp)
            {
                DisplayStatus("HonOSException: " + exp.Message);
            }
        }

        private void DeviceInfoButton_Clicked(object sender, EventArgs e)
        {
            if (null != CurrentDeviceManager)
            {
                try
                {
                    DisplayStatus("Asset Number = " + CurrentDeviceManager.AssetNumber);
                }
                catch (HonOSException exp)
                {
                    DisplayStatus("AssetNumber HonOSException: " + exp.Message);
                }

                try
                {
                    DisplayStatus("IMEI = " + CurrentDeviceManager.IMEI);
                }
                catch (HonOSException exp)
                {
                    DisplayStatus("IMEI HonOSException: " + exp.Message);
                }

                try
                {
                    DisplayStatus("Serial Number = " + CurrentDeviceManager.SerialNumber);
                }
                catch (HonOSException exp)
                {
                    DisplayStatus("SerialNumber HonOSException: " + exp.Message);
                }

                try
                {
                    DisplayStatus("Internal Temperature = " + CurrentDeviceManager.InternalTemperature);
                }
                catch (HonOSException exp)
                {
                    DisplayStatus("InternalTemperature HonOSException: " + exp.Message);
                }

                try
                {
                    DisplayStatus("Key Input Enabled = " + CurrentDeviceManager.KeyInputEnabled);
                }
                catch (HonOSException exp)
                {
                    DisplayStatus("KeyInputEnabled HonOSException: " + exp.Message);
                }

                try
                {
                    DisplayStatus("Touch Input Enabled = " + CurrentDeviceManager.TouchInputEnabled);
                }
                catch (HonOSException exp)
                {
                    DisplayStatus("TouchInputEnabled HonOSException: " + exp.Message);
                }

                try
                {
                    //Display bluetooth address
                    Bundle bundle = CurrentDeviceManager.GetBluetoothAddresses();
                    if (null != bundle)
                    {
                        foreach (string key in bundle.KeySet())
                        {
                            string addressInfo = bundle.GetString(key);
                            DisplayStatus(key + " = " + addressInfo);
                        }
                    }
                }
                catch (HonOSException exp)
                {
                    DisplayStatus("BluetoothAdress HonOSException: " + exp.Message);
                }

                try
                {
                    Bundle bundle = null;
                    //Display WiFi Mac Address
                    bundle = CurrentDeviceManager.GetWiFiMacAddresses();
                    if (null != bundle)
                    {
                        foreach (string key in bundle.KeySet())
                        {
                            string addressInfo = bundle.GetString(key);
                            DisplayStatus(key + " = " + addressInfo);
                        }
                    }
                }
                catch (HonOSException exp)
                {
                    DisplayStatus("WiFiMacAddress HonOSException: " + exp.Message);
                }

                try
                {
                    DisplayStatus("Active touch profile = " + CurrentDeviceManager.ActiveTouchProfile);
                }
                catch (HonOSException exp)
                {
                    DisplayStatus("ActiveTouchProfile HonOSException: " + exp.Message);
                }
                GetHallSensorInfo("All");
            }
        }

        private void ClearButton_Clicked(object sender, EventArgs e)
        {
            mInfoEditor.Text = string.Empty;
            mAssetNumberEntry.Text = string.Empty;
            mWipeDevicePicker.SelectedItem = string.Empty;
        }


        public static async Task<bool> GetDeviceManager()
        {
            bool status = true;

            try
            {
                if (null == CurrentDeviceManager)
                {
                    CurrentDeviceManager = await DeviceManager.GetInstance();
                    //add wipe type to picker
                    CurrentMainPage.mWipeDevicePicker.ItemsSource = Enum.GetValues(typeof(DeviceManager.WipeType)).Cast<DeviceManager.WipeType>().ToList();

                    //add hall sensor id to picker
                    CurrentMainPage.mHallSensorPicker.ItemsSource = new List<string>() {"All",
                                                                                        DeviceManager.HallSensorId.DockConnectedHallSensor,
                                                                                        DeviceManager.HallSensorId.ScannerTriggerHallSensor,
                                                                                        DeviceManager.HallSensorId.SimHallSensor };

                    //add touch profile to picker
                    string[] profilesArray = CurrentDeviceManager.TouchProfiles;
                    foreach (string item in profilesArray)
                    {
                        CurrentMainPage.mTouchProfilePicker.Items.Add(item);
                    }
                }

                if (null != CurrentDeviceManager)
                {
                    CurrentMainPage.mEnableKeyInputSwitch.IsToggled = CurrentDeviceManager.KeyInputEnabled;
                    CurrentMainPage.mEnableTouchInputSwitch.IsToggled = CurrentDeviceManager.TouchInputEnabled;
                }
            }
            catch (HonOSException exp)
            {
                CurrentMainPage.DisplayStatus("HonOSException: " + exp.Message);
            }

            return status;
        }

        public void ReleaseDeviceManager()
        {
            if (null != CurrentDeviceManager)
            {
                CurrentDeviceManager.Dispose();
                CurrentDeviceManager = null;
            }
        }

        private void GetHallSensorInfo(string hallSensorId)
        {
            try
            {
                if (null != CurrentDeviceManager)
                {
                    Bundle retBundle = null;
                    if ("All" == hallSensorId)
                    {
                        retBundle = CurrentDeviceManager.GetHallSensorStatus();
                        if (null != retBundle)
                        {
                            foreach (string id in retBundle.KeySet())
                            {
                                bool status = retBundle.GetBoolean(id);
                                DisplayStatus(id + " = " + status.ToString());
                            }
                        }
                    }
                    else
                    {
                        retBundle = CurrentDeviceManager.GetHallSensorStatus(hallSensorId);
                        bool status = retBundle.GetBoolean(hallSensorId);
                        DisplayStatus(hallSensorId + " = " + status.ToString());
                    }
                }
            }
            catch (HonOSException exp)
            {
                DisplayStatus("HonOSException: " + exp.Message);
            }
        }

        private void DisplayStatus(string info)
        {
            if (mInfoEditor.Text == string.Empty)
            {
                mInfoEditor.Text = info;
            }
            else
            {
                mInfoEditor.Text += System.Environment.NewLine + info;
            }
        }

        private async void ShowError(string errorString)
        {
            await DisplayAlert("Error", errorString, "OK");
        }
        /// <summary>
        /// This method is invoked when the user clicks the action overflow icon
        /// (with 3 vertical dots) on the right side of the app bar and select
        /// "Other Features" menu item.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An object containging the event data.</param>
        private async void OnOtherDeviceFeatures(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new DeviceExtrasPage());
        }

    }
}
