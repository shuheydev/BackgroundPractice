using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace BackgroundPractice.Droid.Services
{
    [Service]
    public class LongRunningTaskService : Service
    {
        readonly string _channelId = "foreground";
        readonly string _channelName = "foreground";
        readonly string _channelDescription = "The foreground channel for notifications";
        readonly int _pendingIntentId = 1;

        NotificationManager _notificationManager;
        bool _channelInitialized = false;


        CancellationTokenSource _cts;

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            _cts = new CancellationTokenSource();

            Task.Run(() =>
            {
                try
                {
                    var counter = new TaskCounter();
                    counter.RunCounter(_cts.Token).Wait();
                }
                catch (Android.OS.OperationCanceledException)
                {

                }
                finally
                {
                    if (_cts.IsCancellationRequested)
                    {
                        var message = new CancelledMessage();
                        MainThread.BeginInvokeOnMainThread(() => MessagingCenter.Send(message, nameof(CancelledMessage)));
                    }
                }
            }, _cts.Token);

            if (!_channelInitialized)
            {
                CreateNotificationChannel();
            }

            Intent foregroundNotificationIntent = new Intent(Android.App.Application.Context, typeof(MainActivity));

            PendingIntent pendingIntent = PendingIntent.GetActivity(Android.App.Application.Context, _pendingIntentId, foregroundNotificationIntent, PendingIntentFlags.OneShot);

            NotificationCompat.Builder builder = new NotificationCompat.Builder(Android.App.Application.Context, _channelId)
                .SetContentIntent(pendingIntent)
                .SetContentTitle("hello")
                .SetContentText("world")
                .SetLargeIcon(BitmapFactory.DecodeResource(Android.App.Application.Context.Resources, Resource.Drawable.xamagonBlue))
                .SetSmallIcon(Resource.Drawable.xamagonBlue)
                .SetDefaults((int)NotificationDefaults.Sound | (int)NotificationDefaults.Vibrate);

            var notification = builder.Build();
            //_notificationManager.Notify(0, notification);

            StartForeground(1, notification);

            return StartCommandResult.NotSticky;
        }

        public override void OnDestroy()
        {
            if (_cts != null)
            {
                _cts.Token.ThrowIfCancellationRequested();

                _cts.Cancel();
            }
            base.OnDestroy();
        }

        private void CreateNotificationChannel()
        {
            _notificationManager = (NotificationManager)Android.App.Application.Context.GetSystemService(Android.App.Application.NotificationService);
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channelNameJava = new Java.Lang.String(_channelName);
                var channel = new NotificationChannel(_channelId, channelNameJava, NotificationImportance.Default)
                {
                    Description = _channelDescription,
                };
                _notificationManager.CreateNotificationChannel(channel);
            }

            _channelInitialized = true;
        }
    }
}