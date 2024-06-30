using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using AsyncEV3Lib;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Lego.EV3.UILib
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EV3RobotChooser : ContentView
	{
        private EV3ConnectionManager connectionManager;

        public EV3ConnectionManager ConnectionManager
        {
            get { return connectionManager; }
            set { connectionManager = value; connectionManager.Devices.CollectionChanged += Devices_CollectionChanged; }
        }

        private void Devices_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach(var item in e.NewItems)
                Devices.Add(item as DeviceInfo);
        }

        public EV3RobotChooser ()
		{
			InitializeComponent ();
            //BindingContext = this;
            //PropertyChanged += EV3RobotChooser_PropertyChanged;
		}

        private void EV3RobotChooser_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Debug.WriteLine(e.PropertyName);
        }

        public ObservableCollection<DeviceInfo> Devices { get; } = new ObservableCollection<DeviceInfo>();

        private void Start_Click(object sender, EventArgs e)
        {
            ConnectionManager.StartUnpairedDeviceWatcher();
        }

        private void Stop_Click(object sender, EventArgs e)
        {
            ConnectionManager.StopWatcher();
        }

        public static readonly BindableProperty SelectedDeviceProperty =
            BindableProperty.Create("SelectedDevice", typeof(DeviceInfo), typeof(EV3RobotChooser), null, BindingMode.TwoWay, propertyChanged: (sender, oldValue, newValue) =>
            {
                if (newValue != null)
                {
                    //(sender as EV3RobotChooser).OnPropertyChanged("SelectedDevice");
                    (sender as EV3RobotChooser).ConnectionManager.StopWatcher();
                    Debug.WriteLine("propChanged");
                }
            });

        public DeviceInfo SelectedDevice
        {
            get { return (DeviceInfo)GetValue(SelectedDeviceProperty); }
            set { SetValue(SelectedDeviceProperty, value); }
        }
    }
}