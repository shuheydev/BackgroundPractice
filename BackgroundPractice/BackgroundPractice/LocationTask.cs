using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace BackgroundPractice
{
    public class LocationTask
    {
        private int _notificationNumber;
        private INotificationManager _notificationManager;
        public LocationTask()
        {
            _notificationManager = DependencyService.Get<INotificationManager>();
        }

        Location _location;
        
        public async Task GetLocation(CancellationToken token)
        {
            var request = new GeolocationRequest(GeolocationAccuracy.Best);
            _location = await Xamarin.Essentials.Geolocation.GetLastKnownLocationAsync();

            await Task.Run(async () =>
            {
                for (long i = 0; i < long.MaxValue; i++)
                {
                    token.ThrowIfCancellationRequested();

                    await Task.Delay(10000);

                    _location = await Geolocation.GetLocationAsync(request);
                    if (_location != null)
                    {
                        var message = new TickedMessage
                        {
                            Message = $"{_location.Latitude.ToString()} , {_location.Longitude.ToString()}"
                            //Message = i.ToString()
                        };

                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            MessagingCenter.Send<TickedMessage>(message, nameof(TickedMessage));
                        });
                    }

                    if (i % 10 == 0)
                    {
                        _notificationNumber++;
                        string title = $"Local Notification #{_notificationNumber}";
                        string notifyMessage = $"You have now receive {_notificationNumber} nogirications";
                        _notificationManager.ScheduleNotification(title, notifyMessage);
                    }
                }
            }, token);
        }
    }
}
