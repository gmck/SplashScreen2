using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using AndroidX.AppCompat.App;
using AndroidX.Preference;
using System;

namespace com.companyname.SplashScreen2
{
    [Application]
    public  class SplashScreen2 : Application
    {
        // Didn't help, so now could probably be excluded
        #region Ctor
        public SplashScreen2(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {

        }
        #endregion

        #region OnCreate
        public override void OnCreate()
        {
            base.OnCreate();

            // I now don't think this is required
            ISharedPreferences sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(this);
            UiNightMode uiNightMode = (UiNightMode)Convert.ToInt16(sharedPreferences.GetString("systemThemeValue", "1")) - 1;

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Q)
            {
                switch (uiNightMode)
                {
                    case UiNightMode.Yes:
                        AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightYes;
                        break;
                    case UiNightMode.No:
                        AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightNo;
                        break;
                    case UiNightMode.Auto:
                    case UiNightMode.Custom:
                        AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightFollowSystem;
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion
    }
}