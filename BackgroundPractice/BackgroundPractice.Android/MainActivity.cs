using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content;
using System.Threading;
using Xamarin.Forms.Internals;
using Xamarin.Forms;
using BackgroundPractice.Droid.Services;
using Android.App.Job;
using Android.Support.Design.Widget;

namespace BackgroundPractice.Droid
{
    [Activity(Label = "BackgroundPractice", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());

            WireUpLongRunningTask();
        }


        private JobScheduler _jobScheduler;
        private readonly int _jobId = 1;
        private void WireUpLongRunningTask()
        {
            MessagingCenter.Subscribe<StartLongRunningTaskMessage>(this, nameof(StartLongRunningTaskMessage), async message =>
            {
                var javaClass = Java.Lang.Class.FromType(typeof(CountJob));
                var componentName = new ComponentName(this, javaClass);
                var jobBuilder = new JobInfo.Builder(_jobId, componentName);
                var jobInfo = jobBuilder
                    //.SetPersisted(true)
                    .SetPeriodic(1)
                    .SetRequiredNetworkType(NetworkType.None)
                    .Build();

                _jobScheduler = (JobScheduler)GetSystemService(JobSchedulerService);
                var scheduleResult = _jobScheduler.Schedule(jobInfo);

                if (JobScheduler.ResultSuccess == scheduleResult)
                {
                    var snackBar = Snackbar.Make(FindViewById(Android.Resource.Id.Content), "スケジュール成功", Snackbar.LengthShort);
                    snackBar.Show();
                }
                else
                {
                    var snackBar = Snackbar.Make(FindViewById(Android.Resource.Id.Content), "スケジュール失敗", Snackbar.LengthShort);
                    snackBar.Show();
                }
            });

            MessagingCenter.Subscribe<StopLongRunningTaskMessage>(this, nameof(StopLongRunningTaskMessage), message =>
            {
                if (_jobScheduler == null)
                    return;

                _jobScheduler.Cancel(_jobId);

                var snackBar = Snackbar.Make(FindViewById(Android.Resource.Id.Content), "スケジュール解除", Snackbar.LengthShort);
                snackBar.Show();
            });
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

    }
}