using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BackgroundPractice
{
    public partial class App : Application
    {
        private readonly BackgroundPage _backgroundPage;

        public App()
        {
            InitializeComponent();

            _backgroundPage = new BackgroundPage();
            //MainPage = _backgroundPage;

            MainPage = new LongRunningPage();
        }

        protected override void OnStart()
        {
            LoadPersistedValues();
        }

        protected override void OnSleep()
        {
            Application.Current.Properties["SleepDate"] = DateTimeOffset.Now.ToString("O");
            Application.Current.Properties["FirstName"] = _backgroundPage.FirstName;
        }

        protected override void OnResume()
        {
            LoadPersistedValues();
        }

        private void LoadPersistedValues()
        {
            if (Application.Current.Properties.ContainsKey("SleepDate"))
            {
                var value = (string)Application.Current.Properties["SleepDate"];
                DateTimeOffset sleepDate;
                if(DateTimeOffset.TryParse(value, out sleepDate))
                {
                    _backgroundPage.SleepDate = sleepDate;
                }
            }

            if(Application.Current.Properties.ContainsKey("FirstName"))
            {
                var firstName = (string)Application.Current.Properties["FirstName"];
                _backgroundPage.FirstName = firstName;
            }
        }
    }
}
