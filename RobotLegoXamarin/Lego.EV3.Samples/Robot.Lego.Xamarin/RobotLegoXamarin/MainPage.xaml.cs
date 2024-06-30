using AsyncEV3Lib;
using Lego.Ev3.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RobotLegoXamarin
{
	public partial class MainPage : ContentPage
	{
        public BrickManager BrickManager { get; } = new BrickManager((App.Current as App).EV3Factory as EV3AbstractFactory);

        public MainPage()
		{
            //BinaryWriter writer = new BinaryWriter(new MemoryStream());
            //writer.Write(0x9401);
            //writer.Write(1);
            //writer.Write((ushort)587);
            //writer.Write((ushort)250);

            InitializeComponent();
            BindingContext = this;
            this.Appearing += MainPage_Appearing;
            this.Disappearing += MainPage_Disappearing;
		}

        private void MainPage_Disappearing(object sender, EventArgs e)
        {
            if (BrickManager.Connected)
                BrickManager.Disconnect();
        }

        internal void Disconnect()
        {
            if (BrickManager.Connected)
                BrickManager.Disconnect();
        }

        private void MainPage_Appearing(object sender, EventArgs e)
        {
            DeviceSelected += MainPage_DeviceSelected;
        }

        private async void MainPage_DeviceSelected(object sender, EventArgs e)
        {
            await BrickManager.ConnectAsync(SelectedDevice?.Id);

            if (BrickManager.Connected)
            {
                await BrickManager.PlayThirdKindAsync(volume: 1, duration: 250);
            }
        }

        private DeviceInfo selectedDevice;


        public DeviceInfo SelectedDevice
        {
            get { return selectedDevice; }
            set
            {
                selectedDevice = value;
                if (value != null)
                {
                    if (BrickManager.Connected)
                        BrickManager.Disconnect();
                    DeviceSelected?.Invoke(this, null);
                }
            }
        }

        event EventHandler DeviceSelected;

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

        private async void DirectCommands_Click(object sender, EventArgs e)
        {
            if (!(sender is Button)) return;

            (sender as Button).IsEnabled = false;

            await BrickManager.DirectCommand.StepMotorAtPowerAsync(InputPort.A, 100);
            await BrickManager.DirectCommand.TurnMotorAtPowerForTimeAsync(InputPort.A, 2000, 100);
            await BrickManager.DirectCommand.PlayToneAsync();
            await BrickManager.DirectCommand.TurnMotorAtPowerForTimeAsync(InputPort.A, 2000, 100);

            (sender as Button).IsEnabled = true;
        }

        private static byte[] GetResourcePath(string resourcePath)
        {
            Assembly assembly = Assembly.GetAssembly(typeof(MainPage));

            var resourceNames = assembly.GetManifestResourceNames();
            var resourcePaths = resourceNames
                .Where(x => x.EndsWith(resourcePath, StringComparison.CurrentCultureIgnoreCase))
                .ToArray();

            var stream = assembly.GetManifestResourceStream(resourcePaths.Single());
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }



        private async void PlayTonesAndSounds_Click(object sender, EventArgs e)
        {
            if (!(sender is Button)) return;

            (sender as Button).IsEnabled = false;



            await BrickManager.CreateDirectoryAsync("../prjs/test");
            await BrickManager.WriteFileAsync(GetResourcePath("Sounds.wilhelm_scream.rsf"), "../prjs/test/wilhelm_scream.rsf");
            await BrickManager.WriteFileAsync(GetResourcePath("Sounds.cheerful.rsf"), "../prjs/test/cheerful.rsf");
            await BrickManager.WriteFileAsync(GetResourcePath("Sounds.determined.rsf"), "../prjs/test/determined.rsf");
            await BrickManager.WriteFileAsync(GetResourcePath("Sounds.Cat purr.rsf"), "../prjs/test/Cat purr.rsf");

            await BrickManager.DirectCommand.PlaySoundAsync("test/wilhelm_scream", duration: 1000);
            await BrickManager.DirectCommand.PlayToneAsync();
            await BrickManager.DirectCommand.PlaySoundAsync("test/cheerful", duration: 1000);
            await BrickManager.DirectCommand.PlayToneAsync(frequency: 2000, duration: 2000, volume: 50);
            await BrickManager.DirectCommand.PlaySoundAsync("test/determined", duration: 2000, loop: true);
            await Task.Delay(3000);
            await BrickManager.DirectCommand.PlaySoundAsync("test/Cat purr", duration: 10000, loop: true);
            await BrickManager.DirectCommand.StopSoundAsync();

            (sender as Button).IsEnabled = true;

        }

        private async void TestMotors_Click(object sender, EventArgs e)
        {
            if (!(sender is Button)) return;

            (sender as Button).IsEnabled = false;

            BrickManager.BatchCommand.AddStepMotorAtPowerAsync(InputPort.B, 100, error: 12);
            BrickManager.BatchCommand.AddStepMotorAtPowerAsync(InputPort.C, 1000, error: 12, power: 100);
            BrickManager.BatchCommand.TurnMotorAtPowerForTimeAsync(InputPort.A, 500);
            BrickManager.BatchCommand.PlayTone();
            await BrickManager.BatchCommand.Execute();

            (sender as Button).IsEnabled = true;

        }
        private async void TestStartAndStopMotors_Click(object sender, EventArgs e)
        {
            if (!(sender is Button)) return;

            (sender as Button).IsEnabled = false;

            BrickManager.BatchCommand.StartMotorAtPowerAsync(InputPort.A, 20);
            BrickManager.BatchCommand.StartMotorAtPowerAsync(InputPort.C, 100);
            await Task.WhenAll(BrickManager.BatchCommand.Execute(), Task.Delay(3000)).ContinueWith(async t =>
            {
                BrickManager.BatchCommand.StopMotorAsync(InputPort.A);
                BrickManager.BatchCommand.StopMotorAsync(InputPort.C);
                await BrickManager.BatchCommand.Execute();
            });

            (sender as Button).IsEnabled = true;

        }

        private async void TestDrawImages_Click(object sender, EventArgs e)
        {
            if (!(sender is Button)) return;

            (sender as Button).IsEnabled = false;

            await BrickManager.CreateDirectoryAsync("../prjs/test");
            await BrickManager.WriteFileAsync(GetResourcePath("Images.ClubInfoRocks.rgf"), "../prjs/test/ClubInfoRocks.rgf");
            await BrickManager.WriteFileAsync(GetResourcePath("Images.Tired middle.rgf"), "../prjs/test/Tired middle.rgf");

            await BrickManager.DrawImageAsync(0, 0, "test/Tired middle.rgf");

            await Task.Delay(3000);

            await BrickManager.DrawImageAsync(0, 0, "test/ClubInfoRocks.rgf");

            (sender as Button).IsEnabled = true;

        }


        //public static readonly BindableProperty SelectedDeviceProperty =
        //    BindableProperty.Create("SelectedDevice", typeof(DeviceInfo), typeof(MainPage), null, BindingMode.TwoWay, propertyChanged: (sender, oldValue, newValue) =>
        //    {
        //        if (newValue != null)
        //        {
        //            if ((sender as MainPage).brickManager.Connected)
        //                (sender as MainPage).brickManager.Disconnect();
        //            (sender as MainPage).DeviceSelected?.Invoke((sender as MainPage), null);
        //        }
        //    });

        //public DeviceInfo SelectedDevice
        //{
        //    get { return (DeviceInfo)GetValue(SelectedDeviceProperty); }
        //    set { SetValue(SelectedDeviceProperty, value); }
        //}

    }
}
