using AsyncEV3Lib;
using Lego.Ev3.Core;
using Lego.EV3.UWP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SampleApp.UWP
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        BrickManager brickManager = new BrickManager();
        public MainPage()
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

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button)) return;

            char port = ((sender as Button).Content as string).Last();

            if (!ports.ContainsKey(port)) return;

            (sender as Button).IsEnabled = false;

            //await brickManager.Brick.DirectCommand.TurnMotorAtPowerForTimeAsync(ports[port], 100, 2000, true);
            await brickManager.Brick.DirectCommand.TurnMotorAtPowerAsync(ports[port], 100);
            await Task.Delay(2000);
            await brickManager.DirectCommand.StopMotorAsync(inputPorts[port]);
            await Task.Delay(1000);
            await brickManager.Brick.DirectCommand.TurnMotorAtPowerAsync(ports[port], -100);
            await Task.Delay(2000);
            await brickManager.DirectCommand.StopMotorAsync(inputPorts[port]);

            (sender as Button).IsEnabled = true;

        }

        private async void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            DeviceSelected += MainPage_DeviceSelected;
            //await brickManager.ConnectAsync("Bluetooth#Bluetooth6c:40:08:95:63:ed-00:16:53:41:e1:2a");
        }

        private async void MainPage_DeviceSelected(object sender, EventArgs e)
        {
            await brickManager.ConnectAsync(SelectedDevice?.Id);

            if (brickManager.Connected)
            {
                await brickManager.PlayThirdKindAsync(volume: 100, duration: 250);
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            if(brickManager.Connected)
                brickManager.Disconnect();
        }

        private DeviceInformation selectedDevice;


        public DeviceInformation SelectedDevice
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
    }
}
