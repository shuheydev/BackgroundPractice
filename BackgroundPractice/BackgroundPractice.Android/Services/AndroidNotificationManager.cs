using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Xamarin.Forms.Platform.Android;

[assembly:Xamarin.Forms.Dependency(typeof(BackgroundPractice.Droid.Services.AndroidNotificationManager))]
namespace BackgroundPractice.Droid.Services
{
    public class AndroidNotificationManager : INotificationManager
    {
        readonly string _channelId = "BackgroundPracticeNotificationChannel";
        readonly string _channelName = "BackgroundPracticeNotificationChannel";
        readonly string _channelDescription = "The default channel for notifications(not for foregroundservice)";
        readonly int _pendingIntentId = 111;

        public readonly string TitleKey = "title";
        public readonly string MessageKey = "message";

        bool _channelInitialized = false;
        int _messageId = -1;

        NotificationManager _notificationManager;

        public event EventHandler NotificationReceived;

        public void Initialize()
        {
            CreateNotificationChannel();
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

        public void ReceiveNotification(string title, string message)
        {
            var args = new NotificationEventArgs()
            {
                Title = title,
                Message = message,
            };
            NotificationReceived?.Invoke(null, args);
        }

        public int ScheduleNotification(string title, string message)
        {
            if(!_channelInitialized)
            {
                CreateNotificationChannel();
            }

            _messageId++;

            Intent intent = new Intent(Android.App.Application.Context, typeof(MainActivity));
            intent.PutExtra(TitleKey, title);
            intent.PutExtra(MessageKey, message);

            PendingIntent pendingIntent = PendingIntent.GetActivity(Android.App.Application.Context, _pendingIntentId, intent, PendingIntentFlags.OneShot);

            NotificationCompat.Builder builder = new NotificationCompat.Builder(Android.App.Application.Context, _channelId)
                .SetContentIntent(pendingIntent)
                .SetContentTitle(title)
                .SetContentText(message)
                .SetLargeIcon(BitmapFactory.DecodeResource(Android.App.Application.Context.Resources, Resource.Drawable.xamagonBlue))
                .SetSmallIcon(Resource.Drawable.xamagonBlue)
                .SetAutoCancel(true)
                .SetDefaults((int)NotificationDefaults.Sound|(int)NotificationDefaults.Vibrate);

            var notification = builder.Build();
            _notificationManager.Notify(_messageId, notification);

            return _messageId;
        }
    }
}