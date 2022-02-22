using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content.Res;

namespace SystemConfigManagerSample.Droid
{
    [Activity(Label = "SystemConfigManagerSample", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
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
        }

        protected override async void OnResume()
        {
            base.OnResume();

            //Gets the instance of SystemConfigManager
            bool status = await MainPage.GetSystemConfigManager();
        }

        protected override void OnDestroy()
        {
            MainPage.CurrentMainPage.ReleaseSystemConfigManager();
            MainPage.CurrentMainPage = null;

            base.OnDestroy();
        }
    }
}

