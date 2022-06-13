using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using AndroidX.AppCompat.App;
using AndroidX.Preference;
using System;

namespace com.companyname.SplashScreen2.Fragments
{
    public class SettingsFragment : PreferenceFragmentCompat
    {
        private ISharedPreferences sharedPreferences;
        private ColorThemeListPreference colorThemeListPreference;
        private SystemThemeListPreference systemThemeListPreference;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            HasOptionsMenu = true;
        }
        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            base.OnCreateOptionsMenu(menu, inflater);
            menu.Clear();
        }

        #region OnCreatePreferences
        public override void OnCreatePreferences(Bundle savedInstanceState, string rootKey)
        {
            sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(Activity);

            SetPreferencesFromResource(Resource.Xml.preferences, rootKey);
            
            if (PreferenceScreen.FindPreference("darkTheme") is CheckBoxPreference checkboxDarkThemePreference)
            {
                if (Build.VERSION.SdkInt < BuildVersionCodes.Q)
                    checkboxDarkThemePreference.PreferenceChange += CheckboxDarkThemePreference_PreferenceChange;
                else
                    checkboxDarkThemePreference.Enabled = false;
            }

            if (PreferenceScreen.FindPreference("colorThemeValue") is ColorThemeListPreference colorThemeListPreference)
            {
                colorThemeListPreference.Init();
                colorThemeListPreference.PreferenceChange += ColorThemeListPreference_PreferenceChange;
            }

            if (PreferenceScreen.FindPreference("systemThemeValue") is SystemThemeListPreference systemThemeListPreference)
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.Q)
                {
                    systemThemeListPreference.Init();
                    systemThemeListPreference.PreferenceChange += SystemThemeListPreference_PreferenceChange;
                }
                else
                    systemThemeListPreference.Enabled = false;
            }
        }
        #endregion

        #region CheckboxDarkThemePreference_PreferenceChange
        private void CheckboxDarkThemePreference_PreferenceChange(object sender, Preference.PreferenceChangeEventArgs e)
        {
            bool requestedNightMode = (bool)e.NewValue;
            ISharedPreferencesEditor editor = sharedPreferences.Edit();
            editor.PutBoolean("darkTheme", requestedNightMode).Apply();
            editor.Commit();
            // This is only available to devices running less than Android 10.
            if (requestedNightMode)
                SetDefaultNightMode(requestedNightMode);
        }
        #endregion

        #region ColorThemeListPreference_PreferenceChange
        private void ColorThemeListPreference_PreferenceChange(object sender, Preference.PreferenceChangeEventArgs e)
        {
            colorThemeListPreference = e.Preference as ColorThemeListPreference;

            ISharedPreferencesEditor editor = colorThemeListPreference.SharedPreferences.Edit();
            editor.PutString("colorThemeValue", e.NewValue.ToString()).Apply();

            int index = Convert.ToInt16(e.NewValue.ToString());
            string colorThemeValue = colorThemeListPreference.GetEntries()[index - 1];
            colorThemeListPreference.Summary = (index != -1) ? colorThemeValue : colorThemeListPreference.DefaultThemeValue;

            // Must now force the theme to change - see BaseActivity. It's OnCreate checks the sharedPreferences, get the string currentTheme and passes that value to SetAppTheme(currentTheme)
            // which checks to see if it has changed and if so calls SetTheme which the correct Resource.Style.Theme_Name)
            Activity.Recreate();
        }
        #endregion

        #region SystemThemeListPreference_PreferenceChange
        private void SystemThemeListPreference_PreferenceChange(object sender, Preference.PreferenceChangeEventArgs e)
        {
            systemThemeListPreference = e.Preference as SystemThemeListPreference;

            ISharedPreferencesEditor editor = systemThemeListPreference.SharedPreferences.Edit();
            editor.PutString("systemThemeValue", e.NewValue.ToString()).Apply();

            int index = Convert.ToInt16(e.NewValue.ToString());
            string systemThemeValue = systemThemeListPreference.GetEntries()[index - 1];
            systemThemeListPreference.Summary = (index != -1) ? systemThemeValue : colorThemeListPreference.DefaultThemeValue;

            // Only available on devices running Android 10+
            UiNightMode uiNightMode = (UiNightMode)index-1;
            SetDefaultNightMode(uiNightMode);
        }
        #endregion

        #region SetDefaultNightMode
        private void SetDefaultNightMode(UiNightMode uiNightMode)
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
        #endregion

        #region SetDefaultNightMode
        private void SetDefaultNightMode(bool requestedNightMode)
        {
            AppCompatDelegate.DefaultNightMode = requestedNightMode ? AppCompatDelegate.ModeNightYes : AppCompatDelegate.ModeNightNo;
        }
        #endregion
    }
}