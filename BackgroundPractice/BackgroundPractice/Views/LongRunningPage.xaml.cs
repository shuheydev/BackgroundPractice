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

            longRunningTask.Clicked += (s, e) =>
            {
                var message = new StartLongRunningTaskMessage();
                MessagingCenter.Send(message, nameof(StartLongRunningTaskMessage));
            };

            stopLongRunningTask.Clicked += (s, e) =>
            {
                var message = new StopLongRunningTaskMessage();
                MessagingCenter.Send(message, nameof(StopLongRunningTaskMessage));
            };

            HandleReceiveMessages();
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
    }
}