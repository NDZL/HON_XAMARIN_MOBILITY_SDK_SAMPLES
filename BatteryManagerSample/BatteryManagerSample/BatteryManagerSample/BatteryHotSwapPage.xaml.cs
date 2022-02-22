using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BatteryManagerSample
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BatteryHotSwapPage : ContentPage
    {
        public static BatteryHotSwapPage CurrentBatteryHotSwapPage { get; set; } = null;

        public BatteryHotSwapPage ()
        {
            InitializeComponent ();

            CurrentBatteryHotSwapPage = this;
        }

        private void ButtonClear_Clicked(object sender, EventArgs e)
        {
            mBatteryHotSwapInfo.Text = string.Empty;
        }

        public void DisplayStatus(string info)
        {
            if (mBatteryHotSwapInfo.Text == string.Empty)
            {
                mBatteryHotSwapInfo.Text = info;
            }
            else
            {
                mBatteryHotSwapInfo.Text += Environment.NewLine + info;
            }
        }
    }
}