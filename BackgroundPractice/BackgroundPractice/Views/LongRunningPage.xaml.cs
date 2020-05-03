using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BackgroundPractice
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LongRunningPage : ContentPage
    {
        public LongRunningPage()
        {
            InitializeComponent();

            startLongRunningTask.Clicked += (s, e) =>
            {
                var message = new StartLongRunningTaskMessage();
                MessagingCenter.Send(message, nameof(StartLongRunningTaskMessage));
            };

            stopLongRunningTask.Clicked += (s, e) =>
            {
                var message = new StopLongRunningTaskMessage();
                MessagingCenter.Send(message, nameof(StopLongRunningTaskMessage));
            };

            notificationTest.Clicked += (s, e) =>
            {
                _notificationNumber++;
                string title = $"Local Notification #{_notificationNumber}";
                string message = $"You have now receive {_notificationNumber} nogirications";
                _notificationManager.ScheduleNotification(title, message);
            };

            HandleReceiveMessages();

            InitializeNotificationManager();
        }

        private void HandleReceiveMessages()
        {
            MessagingCenter.Subscribe<TickedMessage>(this, nameof(TickedMessage), message =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    ticker.Text = message.Message;
                });
            });

            MessagingCenter.Subscribe<CancelledMessage>(this, nameof(CancelledMessage), message =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    ticker.Text = "Cancelled";
                });
            });
        }


        #region Notification
        INotificationManager _notificationManager;
        int _notificationNumber = 0;

        private void InitializeNotificationManager()
        {
            _notificationManager = DependencyService.Get<INotificationManager>();
            _notificationManager.NotificationReceived += (sender, eventArgs) =>
            {
                var eventData = (NotificationEventArgs)eventArgs;
                ShowNotification(eventData.Title, eventData.Message);
            };
        }

        private void ShowNotification(string title, string message)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                var msg = new Label()
                {
                    Text = $"Notification Received:\nTitle: {title}\nMessage: {message}"
                };
            });
        }
        #endregion
    }
}