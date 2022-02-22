using Honeywell.AIDC.Droid.OS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KeyboardManagerSample
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WakeupKeyPage : ContentPage
    {
        public static WakeupKeyPage CurrentWakeupKeyPage { get; set; } = null;

        public WakeupKeyPage()
        {
            InitializeComponent();
            CurrentWakeupKeyPage = this;
        }

        private void PickerKey_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //Remove the switch toggle event to only update the switch state for UI
                SwitchWakeupKeyEnable.Toggled -= SwitchWakeupKeyEnable_Toggled;

                SwitchWakeupKeyEnable.IsToggled = false;

                //check if the selected item is already in wakeup keys list
                var wakupkeyList = MainPage.CurrentKeyboardManager.WakeupKeys;
                if (wakupkeyList.Count > 0)
                {
                    var selectedValue = PickerKey.Items[PickerKey.SelectedIndex];
                    foreach (KeyInfo item in wakupkeyList)
                    {
                        if (selectedValue == item.KeyName)
                        {
                            SwitchWakeupKeyEnable.IsToggled = true;
                            break;
                        }
                    }
                }

                //Add the switch toggle event
                SwitchWakeupKeyEnable.Toggled += SwitchWakeupKeyEnable_Toggled;
            }
            catch (HonOSException exp)
            {
                DisplayStatus("HonOSException: " + exp.Message);
            }
        }

        private void SwitchWakeupKeyEnable_Toggled(object sender, ToggledEventArgs e)
        {
            try
            {
                if (null != MainPage.CurrentKeyboardManager)
                {
                    var keyIDItem = PickerKey.SelectedItem;

                    if (null == keyIDItem)
                    {
                        ShowError("Must select a key for wakeup enable or disable.");
                    }
                    else
                    {
                        KeyMap keyID = (KeyMap)Enum.Parse(typeof(KeyMap), keyIDItem.ToString());
                        MainPage.CurrentKeyboardManager.SetWakeupKeyEnabled(keyID, e.Value);
                    }
                }
            }
            catch (HonOSException exp)
            {
                DisplayStatus("HonOSException: " + exp.Message);
            }
        }

        private void ButtonWakeupKeyList_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (null != MainPage.CurrentKeyboardManager)
                {
                    var wakupkeyList = MainPage.CurrentKeyboardManager.WakeupKeys;
                    if (wakupkeyList.Count > 0)
                    {
                        DisplayStatus("=== Enabled wakeup keys List =====");
                        foreach (KeyInfo item in wakupkeyList)
                        {
                            DisplayStatus("Key name = " + item.KeyName + ",  Key ID = " + item.KeyID);
                        }
                    }
                }
            }
            catch (HonOSException exp)
            {
                DisplayStatus("HonOSException: " + exp.Message);
            }
        }

        private void ButtonClear_Clicked(object sender, EventArgs e)
        {
            CurrentWakeupKeyPage.EditorStatus.Text = string.Empty;
        }

        private async void ShowError(string errorString)
        {
            await DisplayAlert("Error", errorString, "OK");
        }

        private void DisplayStatus(string info)
        {
            if (EditorStatus.Text == string.Empty)
            {
                EditorStatus.Text = info;
            }
            else
            {
                EditorStatus.Text += Environment.NewLine + info;
            }
        }
    }
}