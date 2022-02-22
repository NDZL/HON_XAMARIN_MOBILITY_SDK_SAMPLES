using Honeywell.AIDC.Droid.OS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DeveloperUtilSample
{
    public partial class MainPage : ContentPage
    {
        public static MainPage CurrentMainPage { get; set; } = null;
        public static DeveloperUtil CurrentDeveloperUtil { get; set; } = null;

        public MainPage()
        {
            InitializeComponent();
            CurrentMainPage = this;
        }

        public static async Task<bool> GetDeveloperUtil()
        {
            bool status = true;
            try
            {
                if (null == CurrentDeveloperUtil)
                {
                    CurrentDeveloperUtil = await DeveloperUtil.GetInstance();
                }
            }
            catch (HonOSException exp)
            {
                CurrentMainPage.DisplayStatus("HonOSException: " + exp.Message);
            }

            return status;
        }

        private void ButtonClear_Clicked(object sender, EventArgs e)
        {
            EditorStatus.Text = string.Empty;
        }

        public void ReleaseDeveloperUtil()
        {
            if (null != CurrentDeveloperUtil)
            {
                CurrentDeveloperUtil.Dispose();
                CurrentDeveloperUtil = null;
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

        private void ButtonSelfDiagnostic_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (null != CurrentDeveloperUtil)
                {
                    CurrentDeveloperUtil.ActivateSelfDiagnostic();
                    CurrentMainPage.DisplayStatus("Activated self diagnostic.");
                }
            }
            catch (HonOSException exp)
            {
                CurrentMainPage.DisplayStatus("HonOSException: " + exp.Message);
            }
        }
    }
}
