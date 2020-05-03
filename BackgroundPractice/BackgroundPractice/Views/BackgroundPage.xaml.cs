using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BackgroundPractice
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BackgroundPage : ContentPage
    {
        public BackgroundPage()
        {
            InitializeComponent();
        }

		public DateTimeOffset SleepDate
		{
			set
			{
				this.SleepDateLabel.Text = value.ToString("t");
			}
		}

		public string FirstName
		{
			get
			{
				return this.FirstNameEntry.Text;
			}
			set
			{
				this.FirstNameEntry.Text = value;
			}
		}
	}
}