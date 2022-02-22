using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Honeywell.AIDC.Droid.OS;

namespace SystemConfigManagerSample
{
    public partial class MainPage : ContentPage
    {
        public static MainPage CurrentMainPage { get; set; } = null;
        public static SystemConfigManager CurrentSystemConfigManager { get; set; } = null;

        public MainPage()
        {
            InitializeComponent();
            CurrentMainPage = this;
        }

        public static async Task<bool> GetSystemConfigManager()
        {
            bool status = true;
            try
            {
                if (null == CurrentSystemConfigManager)
                {
                    CurrentSystemConfigManager = await SystemConfigManager.GetInstance();
                }
            }
            catch (HonOSException exp)
            {
                CurrentMainPage.DisplayStatus("HonOSException: " + exp.Message);
            }

            return status;
        }

        private void SwitchNITZEnabled_Toggled(object sender, ToggledEventArgs e)
        {
            try
            {
                if (null != CurrentSystemConfigManager)
                {
                    CurrentSystemConfigManager.NITZ = e.Value;
                }
            }
            catch (HonOSException exp)
            {
                CurrentMainPage.DisplayStatus("HonOSException: " + exp.Message);
            }
        }

        private void ButtonSystemProperty_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (null != CurrentSystemConfigManager)
                {
                    string key = EntrySystemProperty.Text;
                    string property = CurrentSystemConfigManager.GetSystemProperty(key);
                    CurrentMainPage.DisplayStatus("Property " + key + " = " + property);
                }
            }
            catch (HonOSException exp)
            {
                CurrentMainPage.DisplayStatus("HonOSException: " + exp.Message);
            }
        }

        private void ButtonConfigByFile_Clicked(object sender, EventArgs e)
        {
            try
            {
#if __ANDROID__
                var directory = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads);
                string fileName = EntryFileName.Text;
                if(string.IsNullOrEmpty(fileName))
                {
                    CurrentMainPage.DisplayStatus("Must enter file name.");
                }
                else
                {
                    Java.IO.File configFile = new Java.IO.File(directory, fileName);
                    if (configFile.Exists())
                    {
                        CurrentMainPage.DisplayStatus("File Path: " + configFile);
                        CurrentSystemConfigManager.ApplyConfigByFile(configFile.AbsolutePath);
                    }
                    else
                    {
                        CurrentMainPage.DisplayStatus("Incorrect file path");
                    }
                }
#endif
            }
            catch (HonOSException exp)
            {
                CurrentMainPage.DisplayStatus("HonOSException: " + exp.Message);
            }
        }

        public void ReleaseSystemConfigManager()
        {
            if (null != CurrentSystemConfigManager)
            {
                CurrentSystemConfigManager.Dispose();
                CurrentSystemConfigManager = null;
            }
        }

        private void ButtonClear_Clicked(object sender, EventArgs e)
        {
            EditorStatus.Text = string.Empty;
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
    }
}
