using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content.Res;
using Android.Content;

namespace DeviceManagerSample.Droid
{
    [Activity(Label = "DeviceManagerSample", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public const string HallSensorStateChange = "com.honeywell.intent.action.HALL_SENSOR_STATE_CHANGE";
        private HallSensorStateChangeReciever mHallSensorStateChangeReciever = null;
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            //lock application orientation
            ScreenLayout screenSize = Application.Context.ApplicationContext.Resources.Configuration.ScreenLayout & ScreenLayout.SizeMask;
            switch (screenSize)
            {
                case ScreenLayout.SizeXlarge:
                case ScreenLayout.SizeLarge:
                    this.RequestedOrientation = ScreenOrientation.Landscape;
                    break;
                default:
                    this.RequestedOrientation = ScreenOrientation.Portrait;
                    break;
            }

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());
            // Register a receiver for HALL sensor state change intent broadcasts.
            mHallSensorStateChangeReciever = new HallSensorStateChangeReciever();
            IntentFilter intentFilter = new IntentFilter();
            intentFilter.AddAction(HallSensorStateChange);
            RegisterReceiver(mHallSensorStateChangeReciever, intentFilter);
        }
        protected override async void OnResume()
        {
            base.OnResume();

            //Gets the instance of DeviceManager
            bool status = await MainPage.GetDeviceManager();
        }
        protected override void OnDestroy()
        {
            MainPage.CurrentMainPage.ReleaseDeviceManager();
            MainPage.CurrentMainPage = null;
            DeviceExtrasPage.CurrentDeviceExtrasPage = null;
            base.OnDestroy();
        }
        public void OnHallSensorStateChangeEvent()
        {
            if (DeviceExtrasPage.CurrentDeviceExtrasPage != null)
            {
                DeviceExtrasPage.CurrentDeviceExtrasPage.DisplayStatus("HALL Sensor Status Changed.");
            }
        }
        public class HallSensorStateChangeReciever : BroadcastReceiver
        {
            public override void OnReceive(Context context, Intent intent)
            {
                if (intent.Action == MainActivity.HallSensorStateChange)
                {
                    string sensorId = intent.GetStringExtra("hallSensorId");
                    int sensorState = intent.GetIntExtra("hallSensorState", 0);
                    if (DeviceExtrasPage.CurrentDeviceExtrasPage != null)
                    {
                        DeviceExtrasPage.CurrentDeviceExtrasPage.DisplayStatus("HallSensorId = " + sensorId);
                        DeviceExtrasPage.CurrentDeviceExtrasPage.DisplayStatus("HallSensorState = " + sensorState);
                    }
                }
            }
        }
    }
}

