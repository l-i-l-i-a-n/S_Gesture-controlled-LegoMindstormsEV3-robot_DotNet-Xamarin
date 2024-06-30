using AsyncEV3Lib;
using Lego.Ev3.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SampleAppBatchCommand.UWP
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

        private async void DirectCommands_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button)) return;

            (sender as Button).IsEnabled = false;

            await brickManager.DirectCommand.StepMotorAtPowerAsync(InputPort.A, 100);
            await brickManager.DirectCommand.TurnMotorAtPowerForTimeAsync(InputPort.A, 2000, 100);
            await brickManager.DirectCommand.PlayToneAsync();
            await brickManager.DirectCommand.TurnMotorAtPowerForTimeAsync(InputPort.A, 2000, 100);

            (sender as Button).IsEnabled = true;
        }

        private async void PlayTonesAndSounds_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button)) return;

            (sender as Button).IsEnabled = false;

            await brickManager.CreateDirectoryAsync("../prjs/test");
            await brickManager.CopyFileAsync(@"Assets/sounds/wilhelm_scream.rsf", "../prjs/test/wilhelm_scream.rsf");
            await brickManager.CopyFileAsync(@"Assets/sounds/cheerful.rsf", "../prjs/test/cheerful.rsf");
            await brickManager.CopyFileAsync(@"Assets/sounds/determined.rsf", "../prjs/test/determined.rsf");
            await brickManager.CopyFileAsync(@"Assets/sounds/Cat purr.rsf", "../prjs/test/Cat purr.rsf");

            await brickManager.DirectCommand.PlaySoundAsync("test/wilhelm_scream", duration: 1000);
            await brickManager.DirectCommand.PlayToneAsync();
            await brickManager.DirectCommand.PlaySoundAsync("test/cheerful", duration: 1000);
            await brickManager.DirectCommand.PlayToneAsync(frequency:2000, duration:2000, volume:50);
            await brickManager.DirectCommand.PlaySoundAsync("test/determined", duration: 2000, loop: true);
            await Task.Delay(3000);
            await brickManager.DirectCommand.PlaySoundAsync("test/Cat purr", duration: 10000, loop: true);
            await brickManager.DirectCommand.StopSoundAsync();

            (sender as Button).IsEnabled = true;

        }

        private async void TestMotors_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button)) return;

            (sender as Button).IsEnabled = false;

            brickManager.BatchCommand.AddStepMotorAtPowerAsync(InputPort.B, 100, error:12);
            brickManager.BatchCommand.AddStepMotorAtPowerAsync(InputPort.C, 1000, error: 12, power: 100);
            brickManager.BatchCommand.TurnMotorAtPowerForTimeAsync(InputPort.A, 500);
            brickManager.BatchCommand.PlayTone();
            await brickManager.BatchCommand.Execute();

            (sender as Button).IsEnabled = true;

        }
        private async void TestStartAndStopMotors_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button)) return;

            (sender as Button).IsEnabled = false;

            brickManager.BatchCommand.StartMotorAtPowerAsync(InputPort.A, 20);
            brickManager.BatchCommand.StartMotorAtPowerAsync(InputPort.C, 100);
            await Task.WhenAll(brickManager.BatchCommand.Execute(), Task.Delay(3000)).ContinueWith(async t =>
            {
                brickManager.BatchCommand.StopMotorAsync(InputPort.A);
                brickManager.BatchCommand.StopMotorAsync(InputPort.C);
                await brickManager.BatchCommand.Execute();
            });

            (sender as Button).IsEnabled = true;

        }

        private async void TestDrawImages_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button)) return;

            (sender as Button).IsEnabled = false;

            await brickManager.CreateDirectoryAsync("../prjs/test");
            await brickManager.CopyFileAsync(@"Assets/images/ClubInfoRocks.rgf", "../prjs/test/ClubInfoRocks.rgf");
            await brickManager.CopyFileAsync(@"Assets/images/Tired middle.rgf", "../prjs/test/Tired middle.rgf");

            await brickManager.DrawImageAsync(0, 0, "test/Tired middle.rgf");

            await Task.Delay(3000);

            await brickManager.DrawImageAsync(0, 0, "test/ClubInfoRocks.rgf");

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
                await brickManager.PlayThirdKindAsync(volume:10, duration:250);
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            if (brickManager.Connected)
                brickManager.Disconnect();
        }

        internal void Disconnect()
        {
            if (brickManager.Connected)
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
