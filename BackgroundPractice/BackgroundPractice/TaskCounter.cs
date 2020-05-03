using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace BackgroundPractice
{
    public class TaskCounter
    {
        private int _notificationNumber=0;
        private INotificationManager _notificationManager;
        public TaskCounter()
        {
            _notificationManager = DependencyService.Get<INotificationManager>();
        }

        public async Task RunCounter(CancellationToken token)
        {
            await Task.Run(async () =>
            {
                for (long i = 0; i < long.MaxValue; i++)
                {
                    token.ThrowIfCancellationRequested();

                    await Task.Delay(250);
                    var message = new TickedMessage
                    {
                        Message = i.ToString()
                    };

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        MessagingCenter.Send<TickedMessage>(message, nameof(TickedMessage));
                    });

                    _notificationNumber++;

                    if (i % 10 == 0)
                    { 
                        string title = $"Local Notification #{_notificationNumber}";
                        string notifyMessage = $"You have now receive {_notificationNumber} nogirications";
                        _notificationManager.ScheduleNotification(title, notifyMessage);
                    }
                }
            }, token);
        }


        private int _counter = 0;
        public void IncrementCounter(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var message = new TickedMessage
            {
                Message = _counter.ToString()
            };

            MainThread.BeginInvokeOnMainThread(() =>
            {
                MessagingCenter.Send<TickedMessage>(message, nameof(TickedMessage));
            });

            if(_counter% 10==0)
            {
                string title = $"Local Notification #{_notificationNumber}";
                string notifyMessage = $"You have now receive {_notificationNumber} nogirications";
                _notificationManager.ScheduleNotification(title, notifyMessage);
            }

            _notificationNumber++;
        }
    }
}
