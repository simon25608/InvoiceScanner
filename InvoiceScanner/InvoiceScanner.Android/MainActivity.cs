using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Tesseract;
using Tesseract.Droid;
using Plugin.CurrentActivity;
using Xamarin.Forms;
using TinyIoC;
using XLabs.Ioc.TinyIOC;
using XLabs.Ioc;
using XLabs.Platform.Device;

namespace InvoiceScanner.Droid
{
    [Activity(Label = "InvoiceScanner", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;



            base.OnCreate(savedInstanceState);
            CrossCurrentActivity.Current.Init(this, savedInstanceState);

            var container = TinyIoCContainer.Current;
            container.Register<IDevice>(AndroidDevice.CurrentDevice);
            container.Register<ITesseractApi>((cont, parameters) =>
            {
                return new TesseractApi(ApplicationContext, AssetsDeployment.OncePerInitialization);
            });

            Resolver.SetResolver(new TinyResolver(container));

            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}