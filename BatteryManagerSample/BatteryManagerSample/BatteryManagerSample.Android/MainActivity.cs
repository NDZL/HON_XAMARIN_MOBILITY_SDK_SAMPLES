using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content;
using Android.Content.Res;

namespace BatteryManagerSample.Droid
{
    public interface BatteryHotSwapListener
    {
        void OnBatterySwapStartedEvent();
        void OnBatterySwapCompletedEvent(bool state);
    }

    [Activity(Label = "BatteryManagerSample", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, BatteryHotSwapListener
    {
        public const string BatterySwapStarted = "com.honeywell.batteryswap.STARTED";
        public const string BatterySwapCompleted = "com.honeywell.batteryswap.COMPLETED";

        private BatteryHotSwapReceiver mBatteryHotSwapReceiver = null;

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
            LoadApplication(new BatteryManagerSample.App());

            // Register a receiver for hot-swap intent broadcasts.
            mBatteryHotSwapReceiver = new BatteryHotSwapReceiver(this);
            IntentFilter intentFilter = new IntentFilter();
            intentFilter.AddAction(BatterySwapStarted);
            intentFilter.AddAction(BatterySwapCompleted);
            RegisterReceiver(mBatteryHotSwapReceiver, intentFilter);
        }

        protected override async void OnResume()
        {
            base.OnResume();

            // Get an instance of BatteryManager.
            bool status = await MainPage.GetBatteryManager();
            MainPage.CurrentMainPage.UpdateUI(status);
        }

        protected override void OnDestroy()
        {
            UnregisterReceiver(mBatteryHotSwapReceiver);

            MainPage.CurrentMainPage.ReleaseBatteryManager();
            MainPage.CurrentMainPage = null;
            BatteryHotSwapPage.CurrentBatteryHotSwapPage = null;

            base.OnDestroy();
        }

        public void OnBatterySwapStartedEvent()
        {
            if (BatteryHotSwapPage.CurrentBatteryHotSwapPage != null)
            {
                BatteryHotSwapPage.CurrentBatteryHotSwapPage.DisplayStatus("Battery swap started.");
            }
        }

        public void OnBatterySwapCompletedEvent(bool isUncontrolled)
        {
            if (BatteryHotSwapPage.CurrentBatteryHotSwapPage != null)
            {
                if (isUncontrolled)
                {
                    BatteryHotSwapPage.CurrentBatteryHotSwapPage.DisplayStatus("Battery swap completed with uncontrolled.");
                }
                else
                {
                    BatteryHotSwapPage.CurrentBatteryHotSwapPage.DisplayStatus("Battery swap completed with controlled.");
                }
            }
        }

        public class BatteryHotSwapReceiver : BroadcastReceiver
        {
            public BatteryHotSwapListener mBatteryHotSwapListener = null;

            public BatteryHotSwapReceiver(BatteryHotSwapListener aListener)
            {
                mBatteryHotSwapListener = aListener;
            }

            public override void OnReceive(Context context, Intent intent)
            {
                if (intent.Action == MainActivity.BatterySwapStarted)
                {
                    if (mBatteryHotSwapListener != null)
                    {
                        mBatteryHotSwapListener.OnBatterySwapStartedEvent();
                    }
                }
                else if (intent.Action == MainActivity.BatterySwapCompleted)
                {
                    if (mBatteryHotSwapListener != null)
                    {
                        if (intent.GetBooleanExtra("EXTRA_UNCONTROLLED", true))
                        {
                            mBatteryHotSwapListener.OnBatterySwapCompletedEvent(true);
                        }
                        else
                        {
                            mBatteryHotSwapListener.OnBatterySwapCompletedEvent(false);
                        }
                    }
                }
            }
        }
    }
}

