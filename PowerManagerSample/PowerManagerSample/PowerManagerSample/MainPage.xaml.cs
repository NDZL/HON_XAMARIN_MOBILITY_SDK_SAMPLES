using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Honeywell.AIDC.Droid.OS;

namespace PowerManagerSample
{
	public partial class MainPage : ContentPage
	{
        private static Honeywell.AIDC.Droid.OS.PowerManager CurrentPowerManager = null;
        public static MainPage CurrentMainPage { get; set; } = null;

        public MainPage()
        {
            InitializeComponent();
            CurrentMainPage = this;
        }

        public static async Task<bool> GetPowerManager()
        {
            bool status = false;
            try
            {
                if (null == CurrentPowerManager)
                {
                    CurrentPowerManager = await Honeywell.AIDC.Droid.OS.PowerManager.GetInstance();
                }
                if (CurrentPowerManager != null)
                {
                    status = true;
                }
            }
            catch (HonOSException ex)
            {
                CurrentMainPage.DisplayStatus("Failed to create PowerManager object, exception: " + ex.Message);
            }

            return status;
        }

        public void ReleasePowerManager()
        {
            if (CurrentPowerManager != null)
            {
                CurrentPowerManager.Dispose();
                CurrentPowerManager = null;
            }
        }

        /// <summary>
        /// Update the UI based on whether an instance of PowerManager exists.
        /// </summary>
        /// <param name="powerMgrCreated">True indicates an instance of PowerManager exists.</param>
        public void UpdateUI(bool powerMgrCreated)
        {
            mButtonSuspend.IsEnabled = powerMgrCreated;
            mButtonReboot.IsEnabled = powerMgrCreated;
        }

        private void DisplayStatus(string msg)
        {
            mEditorStatus.Text += System.Environment.NewLine + msg;
        }

        private async void ButtonSuspend_Clicked(object sender, EventArgs e)
        {
            if (CurrentPowerManager != null)
            {
                try
                {
                    bool isYes = await DisplayAlert("Question", "Suspend device now?", "Yes", "No");
                    if (isYes)
                    {
                        CurrentPowerManager.Suspend();
                    }
                }
                catch (HonOSException ex)
                {
                    DisplayStatus("Failed to suspend the device, exception: " + ex.Message);
                    if (ex.HResult == (int)HonOSException.ErrorCode.NoPermission)
                    {
                        DisplayStatus("Please add this sample app package name to the provision whitelist.");
                    }
                }
            }
            else
            {
                DisplayStatus("The PowerManager object has not been created.");
            }
        }

        private async void ButtonReboot_Clicked(object sender, EventArgs e)
        {
            if (CurrentPowerManager != null)
            {
                try
                {
                    bool isYes = await DisplayAlert("Question", "Reboot device now?", "Yes", "No");
                    if (isYes)
                    {
                        CurrentPowerManager.Reboot();
                    }
                }
                catch (HonOSException ex)
                {
                    DisplayStatus("Failed to reboot the device, exception: " + ex.Message);
                    if (ex.HResult == (int)HonOSException.ErrorCode.NoPermission)
                    {
                        DisplayStatus("Please add this sample app package name to the provision whitelist.");
                    }
                }
            }
            else
            {
                DisplayStatus("The PowerManager object has not been created.");
            }
        }

        private void ButtonClear_Clicked(object sender, EventArgs e)
        {
            mEditorStatus.Text = string.Empty;
        }
    }
}
