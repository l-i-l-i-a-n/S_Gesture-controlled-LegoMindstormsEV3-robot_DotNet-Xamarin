using AsyncEV3Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using xaMyo;

namespace FirstSampleApp
{
	public partial class App : Application
	{/// <summary>
	 /// The Hub managing Myos
	 /// </summary>
		public IHub Hub { get; } = DependencyService.Get<IHub>();

		public IEV3AbstractFactory EV3Factory { get; } = DependencyService.Get<IEV3AbstractFactory>();

        public App()
        {
            InitializeComponent();
			//EV3Factory = factory;
			Hub.LockingPolicy = LockingPolicy.Standard;
			MainPage = new FirstSampleApp.MainPage();
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
