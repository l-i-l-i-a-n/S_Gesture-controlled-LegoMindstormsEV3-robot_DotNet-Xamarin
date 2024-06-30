using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BluetoothDevicesScanner;
using System.Collections.ObjectModel;

namespace AsyncEV3MotorCommandsLib
{
    /// <summary>
    /// Logique d'interaction pour EV3RobotChooser.xaml
    /// </summary>
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class EV3RobotChooser : UserControl
    {
        public EV3RobotChooser()
        {
            this.InitializeComponent();
        }

        public BluetoothDevice SelectedDevice
        {
            get { return (BluetoothDevice)GetValue(SelectedDeviceProperty); }
            set { SetValue(SelectedDeviceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedDevice.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedDeviceProperty =
            DependencyProperty.Register("SelectedDevice", typeof(BluetoothDevice), typeof(EV3RobotChooser));


        private ObservableCollection<BluetoothDevice> devices = new ObservableCollection<BluetoothDevice>();

        public ObservableCollection<BluetoothDevice> Devices
        {
            get { return devices; }
            set { devices = value; }
        }

        private async Task StartUnpairedDeviceWatcher()
        {
            await BluetoothManager.FindBluetoothDevices();
            foreach (var ev3 in BluetoothManager.EV3Devices)
            {
                Devices.Add(ev3);
            }
            Status = "";
        }

        private async void Start_Click(object sender, RoutedEventArgs e)
        {
            await Dispatcher.InvokeAsync(() =>
            {
                Status = "Scanning...";
            });
            await StartUnpairedDeviceWatcher();
        }



        public string Status
        {
            get { return (string)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Status.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register("Status", typeof(string), typeof(EV3RobotChooser), new PropertyMetadata(""));


    }
}
