using Android.Views;
using Honeywell.AIDC.Droid.OS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace KeyboardManagerSample
{
    public partial class MainPage : ContentPage
    {
        public static MainPage CurrentMainPage { get; set; } = null;
        public static KeyboardManager CurrentKeyboardManager { get; set; } = null;

        public MainPage()
        {
            InitializeComponent();
            CurrentMainPage = this;
            EditorStatus.Text = string.Empty;
        }

        public static async Task<bool> GetKeyboardManager()
        {
            bool status = true;

            try
            {
                if (null == CurrentKeyboardManager)
                {
                    CurrentKeyboardManager = await KeyboardManager.GetInstance();
                }

                if (null != CurrentKeyboardManager)
                {
                    //get support key list
                    //Gets KeyMap list for UI pickers
                    if (CurrentMainPage.PickerKey.Items.Count == 0)
                    {
                        List<KeyMap> keyMapList = Enum.GetValues(typeof(KeyMap)).Cast<KeyMap>().ToList();
                        var sortedKeyMapList = keyMapList.OrderBy(x => (string)(x.ToString())).ToList();
                        CurrentMainPage.PickerToKey.ItemsSource = sortedKeyMapList;

                        //Gets supported Key list
                        var supportKeyList = CurrentKeyboardManager.RemappableKeys;
                        //Only add supported keys to pickers
                        foreach (KeyInfo supportItem in supportKeyList)
                        {
                            if (Enum.IsDefined(typeof(KeyMap), supportItem.KeyID))
                            {
                                CurrentMainPage.PickerKey.Items.Add(supportItem.KeyName);
                            }
                        }
                    }
                }
            }
            catch (HonOSException exp)
            {
                CurrentMainPage.DisplayStatus("HonOSException: " + exp.Message);
            }

            return status;
        }

        public void OnKeyDown(Keycode keyCode, KeyEvent e)
        {
            DisplayStatus("- Pressed Key " + (Keycode)keyCode +
                          ", Key Code = " + (int)keyCode);
        }

        public void ReleaseKeyboardManager()
        {
            if (null != CurrentKeyboardManager)
            {
                CurrentKeyboardManager.Dispose();
                CurrentKeyboardManager = null;
            }
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

        private async void ShowError(string errorString)
        {
            await DisplayAlert("Error", errorString, "OK");
        }

        private void ButtonMapToKey_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (null != CurrentKeyboardManager)
                {
                    var keyIDItem = PickerKey.SelectedItem;
                    var keyCodeItem = PickerToKey.SelectedItem;

                    if ((null == keyIDItem) || (null == keyCodeItem))
                    {
                        ShowError("Must select 'Key' and 'To Key' items.");
                    }
                    else
                    {
                        KeyMap keyID = (KeyMap)Enum.Parse(typeof(KeyMap), keyIDItem.ToString());
                        KeyMap keyCode = (KeyMap)Enum.Parse(typeof(KeyMap), keyCodeItem.ToString());

                        CurrentKeyboardManager.MapToKey(keyID, keyCode);
                        DisplayStatus("- Map " + keyID + " to " + keyCode);
                    }
                }
            }
            catch (HonOSException exp)
            {
                DisplayStatus("HonOSException: " + exp.Message);
            }
        }

        private void ButtonKeyRemappedList_Clicked(object sender, EventArgs e)
        {
            try
            {
                var remappedList = CurrentKeyboardManager.RemappedKeys;
                if (remappedList.Count > 0)
                {
                    DisplayStatus("=== Key Remapped List =====");
                    foreach (KeyRemappedInfo item in remappedList)
                    {
                        DisplayStatus(string.Empty);
                        DisplayStatus("- KeyID = " + (KeyMap)item.KeyID + ", value = " + item.KeyID);

                        if (item.RemapType == KeyRemappedInfo.RemapTypeKey)
                        {
                            DisplayStatus("To KeyID = " + (KeyMap)item.KeyCode + ", value = " + item.KeyCode);
                        }
                        else if (item.RemapType == KeyRemappedInfo.RemapTypeActivity)
                        {
                            DisplayStatus("To Activity Name = " + item.ActivityName);
                        }
                    }
                }
                else
                {
                    DisplayStatus("- No key was mapped.");
                }
            }
            catch (HonOSException exp)
            {
                DisplayStatus("HonOSException: " + exp.Message);
            }
        }

        private void ButtonMapToActivity_Clicked(object sender, EventArgs e)
        {
            try
            {
                var keyIDItem = PickerKey.SelectedItem;
                string activityName = EntryActivityName.Text;

                if ((null == keyIDItem) || (activityName.Count() < 1))
                {
                    ShowError("Must select 'Key' item and enter 'To Activity' name.");
                }
                else
                {
                    KeyMap keyID = (KeyMap)Enum.Parse(typeof(KeyMap), keyIDItem.ToString());

                    CurrentKeyboardManager.MapToActivity((KeyMap)keyID, activityName);
                    DisplayStatus("- Map " + keyID + " to activity " + activityName);
                }
            }
            catch (HonOSException exp)
            {
                DisplayStatus("HonOSException: " + exp.Message);
            }
        }

        private void ButtonClearKey_Clicked(object sender, EventArgs e)
        {
            try
            {
                var keyIDItem = PickerKey.SelectedItem;

                if (null == keyIDItem)
                {
                    ShowError("- Must select 'Key' item.");
                }
                else
                {
                    KeyMap keyID = (KeyMap)Enum.Parse(typeof(KeyMap), keyIDItem.ToString());
                    CurrentKeyboardManager.ClearKeyRemap(keyID);
                    DisplayStatus("Cleared  " + keyID);
                }
            }
            catch (HonOSException exp)
            {
                DisplayStatus("HonOSException: " + exp.Message);
            }
        }

        private void ButtonClearAll_Clicked(object sender, EventArgs e)
        {
            try
            {
                EditorStatus.Text = string.Empty;

                CurrentKeyboardManager.ClearAllKeyRemaps();
                DisplayStatus("- Cleared all keys.");
            }
            catch (HonOSException exp)
            {
                DisplayStatus("HonOSException: " + exp.Message);
            }
        }

        private async void OnWakeupKey(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new WakeupKeyPage());
        }

    }
}
