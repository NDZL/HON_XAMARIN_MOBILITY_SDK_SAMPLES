using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Honeywell.AIDC.Droid.OS;

namespace DeviceManagerSample
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DeviceExtrasPage : ContentPage
    {
        public static DeviceExtrasPage CurrentDeviceExtrasPage { get; set; } = null;
        public DeviceExtrasPage()
        {
            InitializeComponent();
            CurrentDeviceExtrasPage = this;
        }

        public async void ButtonSetLED_Clicked(object Sender, EventArgs e)
        {
            if (null != MainPage.CurrentDeviceManager)
            {
                try
                {
                    string color = mColorEntry.Text;
                    string period = mPeriodEntry.Text;
                    string duty = mDutyEntry.Text;
                    DisplayStatus("Color Entered = " + color);
                    DisplayStatus("Period Entered = " + period);
                    DisplayStatus("Duty Entered = " + duty);
                    MainPage.CurrentDeviceManager.SetRGBLedNotification(Convert.ToInt32(color, 16), Convert.ToInt32(period), Convert.ToInt32(duty));
                }
                catch (HonOSException exp)
                {
                    DisplayStatus("SetRGBLedNotification HonOSException: " + exp.Message);
                }
                catch (FormatException exp)
                {
                    DisplayStatus("SetRGBLedNotification FormatException: " + exp.Message);
                    await DisplayAlert("Error", "Some input field contains non-numeric or invalid numeric value.", "OK");
                }
                catch (OverflowException exp)
                {
                    DisplayStatus("SetRGBLedNotification OverflowException: " + exp.Message);
                    await DisplayAlert("Error", "Some input field contains non-numeric or invalid numeric value.", "OK");
                }
            }
        }

        public void ButtonClear_Clicked(object Sender, EventArgs e)
        {
            mLEDStatusInfo.Text = string.Empty;
            mColorEntry.Text = string.Empty;
            mPeriodEntry.Text = string.Empty;
            mDutyEntry.Text = string.Empty;

            if (null != MainPage.CurrentDeviceManager)
            {
                try
                {
                    // Turn off the LED light.
                    MainPage.CurrentDeviceManager.SetRGBLedNotification(0, 2, 0);
                }
                catch (HonOSException exp)
                {
                    DisplayStatus("Failed to turn of the LED light, HonOSException: " + exp.Message);
                }
            }
        }

        public void DisplayStatus(string info)
        {
            if (mLEDStatusInfo.Text == string.Empty)
            {
                mLEDStatusInfo.Text = info;
            }
            else
            {
                mLEDStatusInfo.Text += Environment.NewLine + info;
            }
        }
    }
}