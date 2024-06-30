using AsyncEV3MotorCommandsLib;
using BluetoothDevicesScanner;
using Lego.Ev3.Core;
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

namespace ex_004_SampleApp
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BrickManager brickManager = new BrickManager();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = brickManager;
        }

        internal static Dictionary<char, OutputPort> ports = new Dictionary<char, OutputPort>()
        {
            {'A', OutputPort.A},
            {'B', OutputPort.B},
            {'C', OutputPort.C},
            {'D', OutputPort.D}
        };

        internal static Dictionary<char, InputPort> inputPorts = new Dictionary<char, InputPort>()
        {
            {'A', InputPort.A},
            {'B', InputPort.B},
            {'C', InputPort.C},
            {'D', InputPort.D}
        };

        private BluetoothDevice selectedDevice;


                    

        public BluetoothDevice SelectedDevice
        {
            get { return selectedDevice; }
            set
            {
                selectedDevice = value;
                if (value != null)
                {
                    if (brickManager.Connected)
                        brickManager.Disconnect();
                    DeviceSelected?.Invoke(this, null);
                }
            }
        }

        event EventHandler DeviceSelected;

        private void root_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (brickManager.Connected)
                brickManager.Disconnect();
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button)) return;

            char port = ((sender as Button).Content as string).Last();

            if (!ports.ContainsKey(port)) return;

            //await brickManager.Brick.DirectCommand.TurnMotorAtPowerForTimeAsync(ports[port], 100, 2000, true);
            await brickManager.Brick.DirectCommand.TurnMotorAtPowerAsync(ports[port], 100);
            await Task.Delay(2000);
            await brickManager.DirectCommand.StopMotorAsync(inputPorts[port]);
            await Task.Delay(1000);
            await brickManager.Brick.DirectCommand.TurnMotorAtPowerAsync(ports[port], -100);
            await Task.Delay(2000);
            await brickManager.DirectCommand.StopMotorAsync(inputPorts[port]);
        }

        private void root_Loaded(object sender, RoutedEventArgs e)
        {
            DeviceSelected += MainWindow_DeviceSelected;
        }

        private async void MainWindow_DeviceSelected(object sender, EventArgs e)
        {
            await brickManager.ConnectAsync(SelectedDevice?.DeviceName, SelectedDevice?.COMPort);

            if (brickManager.Connected)
            {
                await brickManager.PlayThirdKindAsync(volume: 10, duration: 250);
            }
        }
    }
}
