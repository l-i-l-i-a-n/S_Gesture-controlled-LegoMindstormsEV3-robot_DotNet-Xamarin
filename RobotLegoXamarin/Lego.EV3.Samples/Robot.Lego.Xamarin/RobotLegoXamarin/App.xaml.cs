using AsyncEV3Lib;
using Lego.EV3.PCL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace RobotLegoXamarin
{
	public partial class App : Application
	{
        public IEV3AbstractFactory EV3Factory { get; } = DependencyService.Get<IEV3AbstractFactory>();

        public App ()
		{
			InitializeComponent();
            MainPage = new RobotLegoXamarin.MainPage();
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
