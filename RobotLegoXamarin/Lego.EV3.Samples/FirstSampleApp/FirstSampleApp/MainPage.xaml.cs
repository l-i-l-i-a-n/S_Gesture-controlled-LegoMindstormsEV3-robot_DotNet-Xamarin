 using AsyncEV3Lib;
using Lego.Ev3.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using xaMyo;

namespace FirstSampleApp
{
	public partial class MainPage : ContentPage
	{
        public BrickManager BrickManager { get; } = new BrickManager((App.Current as App).EV3Factory as EV3AbstractFactory);
        public AccelerometerTest hi;
        /// <summary>
        /// the Hub managing Myos
        /// </summary>
        public IHub Hub => (Application.Current as App).Hub;

        /// <summary>
        /// the current connected Myo
        /// </summary>
        IMyo Myo { get; set; }

        public MainPage()
        {
            InitializeComponent();
            hi = new AccelerometerTest(this);
            hi.ToggleGyroscope();
            BindingContext = this;
            this.Appearing += MainPage_Appearing;
            this.Disappearing += MainPage_Disappearing;

            Hub.MyoAttached += Hub_MyoAttached;
            Hub.MyoDetached += Hub_MyoDetached;
            Hub.MyoConnected += Hub_MyoConnected;
            Hub.MyoDisconnected += Hub_MyoDisconnected;

        }

        private void MainPage_Disappearing(object sender, EventArgs e)
        {
            if (BrickManager.Connected)
                BrickManager.Disconnect();
        }

        public void updateValues(float x, float y, float z)
        {
            XText.Text = x.ToString();
            YText.Text = y.ToString();
            ZText.Text = z.ToString();
            if(BrickManager.Connected)
            BrickManager.DirectCommand.StartMotorAtPowerAsync(InputPort.B, (int)(x*20));
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

        /// <summary>
        /// fired when the "Attach" button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Attach_Clicked(object sender, EventArgs e)
        {
            Hub.AttachToAdjacent();
        }

        /// <summary>
        /// fired when the Myo is disconnected
        /// </summary>
        /// <param name="sender">the Hub firing the MyoDisconnected event</param>
        /// <param name="e">contains the Myo that has been disconnected</param>
        private void Hub_MyoDisconnected(object sender, MyoDisconnectedEventArgs e)
        {
           //Console.WriteLine( $"{e.Myo.Name} disconnected");
            Myo = e.Myo;
            Myo.ArmSync -= Myo_ArmSync;
            Myo.ArmUnsync -= Myo_ArmUnsync;
            Myo.GyroscopeReceived -= Myo_GyroscopeReceived;
            Myo.MyoLocked -= Myo_MyoLocked;
            Myo.MyoUnlocked -= Myo_MyoUnlocked;
            Myo.OrientationReceived -= Myo_OrientationReceived;
            Myo.PoseReceived -= Myo_PoseReceived;
            Myo.AccelerometerDataReceived -= Myo_AccelerometerDataReceived;
        }

        /// <summary>
        /// fired when a Myo is connected
        /// </summary>
        /// <param name="sender">the Hub firing the MyoConnected event</param>
        /// <param name="e">contains the Myo that is connected</param>
        private void Hub_MyoConnected(object sender, MyoConnectedEventArgs e)
        {
            Console.WriteLine( $"{e.Myo.Name} connected");
            Myo = e.Myo;
            Myo.ArmSync += Myo_ArmSync;
            Myo.ArmUnsync += Myo_ArmUnsync;
            Myo.GyroscopeReceived += Myo_GyroscopeReceived;
            Myo.MyoLocked += Myo_MyoLocked;
            Myo.MyoUnlocked += Myo_MyoUnlocked;
            Myo.OrientationReceived += Myo_OrientationReceived;
            Myo.PoseReceived += Myo_PoseReceived;
            Myo.AccelerometerDataReceived += Myo_AccelerometerDataReceived;
        }

        /// <summary>
        /// fired when new acceleration data are received
        /// </summary>
        /// <param name="sender">the Myo sending acceleration data</param>
        /// <param name="e">acceleration data</param>
        private void Myo_AccelerometerDataReceived(object sender, AccelerometerReceivedEventArgs e)
        {
           // Console.Out.WriteLine($"Acceleration: {e.AccelerometerData.X:00.00} ; {e.AccelerometerData.Y:00.00} ; {e.AccelerometerData.Z:00.00}");
        }

        /// <summary>
        /// fired when the Myo is detached from the Arm
        /// </summary>
        /// <param name="sender">the Hub managing Myo</param>
        /// <param name="e">the detached Myo</param>
        private void Hub_MyoDetached(object sender, MyoDetachedEventArgs e)
        {
            Console.Out.WriteLine($"{e.Myo.Name} detached");

        }

        /// <summary>
        /// fired when the Myo is attached to the arm
        /// </summary>
        /// <param name="sender">the Hub managing Myos</param>
        /// <param name="e">the attached Myo</param>
        private void Hub_MyoAttached(object sender, MyoAttachedEventArgs e)
        {
            Console.Out.WriteLine($"{e.Myo.Name} attached");

        }

        /// <summary>
        /// fired when a new pose is detected by the Myo
        /// </summary>
        /// <param name="sender">the connected Myo</param>
        /// <param name="e">the new pose</param>
        private void Myo_PoseReceived(object sender, PoseChangedEventArgs e)
        {
            Console.Out.WriteLine($"{e.Pose}");
            switch(e.Pose)
            {
                case PoseType.WaveOut:
                    RotateRight();
                    break;
                case PoseType.WaveIn:
                    RotateLeft();
                    break;
                case PoseType.FingersSpread:
                    Forward();
                    break;
                case PoseType.Fist:
                    Backwards();
                    break;
                default:
                    Stop();
                    break;
            }
        }

        private async void Stop()
        {
            BrickManager.DirectCommand.StartMotorAtPowerAsync(InputPort.C, 0);
            BrickManager.DirectCommand.StartMotorAtPowerAsync(InputPort.D, 0);
            BrickManager.DirectCommand.StartMotorAtPowerAsync(InputPort.A, 0);
        }

        /// <summary>
        /// fired when new orientation data are received
        /// </summary>
        /// <param name="sender">the Myo sending orientation data</param>
        /// <param name="e">orientation data</param>
        private void Myo_OrientationReceived(object sender, OrientationReceivedEventArgs e)
        {
            //Console.Out.WriteLine($"{(sender as IMyo).Name} orientation: {e.Quaternion.W:00.00} ; {e.Quaternion.X:00.00} ; {e.Quaternion.Y:00.00} ; {e.Quaternion.Z:00.00}");
        }

        /// <summary>
        /// fired when the Myo is unlocked
        /// </summary>
        /// <param name="sender">the unlocked Myo</param>
        /// <param name="e"></param>
        private void Myo_MyoUnlocked(object sender, MyoUnlockedEventArgs e)
        {
           // Console.Out.WriteLine($"{(sender as IMyo).Name} unlocked");
        }

        /// <summary>
        /// fired when the Myo is locked
        /// </summary>
        /// <param name="sender">the locked Myo</param>
        /// <param name="e"></param>
        private void Myo_MyoLocked(object sender, MyoLockedEventArgs e)
        {
           //Console.Out.WriteLine($"{(sender as IMyo).Name} locked");
        }

        /// <summary>
        /// fired when new gyroscope data are received
        /// </summary>
        /// <param name="sender">the Myo sending gyroscope data</param>
        /// <param name="e">gyroscope data</param>
        private void Myo_GyroscopeReceived(object sender, GyroscopeReceivedEventArgs e)
        {
            //Console.Out.WriteLine( $"Gyroscope: {e.GyroscopeData.X:000} ; {e.GyroscopeData.Y:000} ; {e.GyroscopeData.Z:000}");
        }

        /// <summary>
        /// fired when the Myo is unsynched from the arm
        /// </summary>
        /// <param name="sender">the unsynched Myo</param>
        /// <param name="e"></param>
        private void Myo_ArmUnsync(object sender, ArmUnsyncEventArgs e)
        {
           // Console.Out.WriteLine($"{(sender as IMyo).Name} unsync");
        }

        /// <summary>
        /// fired when the Myo is synched to the arm
        /// </summary>
        /// <param name="sender">the synched Myo</param>
        /// <param name="e">Arm, Arm X Direction</param>
        private void Myo_ArmSync(object sender, ArmSyncEventArgs e)
        {
            //Console.Out.WriteLine($"{e.Arm} ; {e.ArmXDirection} ; {(sender as IMyo).Arm}");
            Myo.Unlock(UnlockType.Hold);
        }


        private async void MainPage_DeviceSelected(object sender, EventArgs e)
        {
            await BrickManager.ConnectAsync(SelectedDevice?.Id);

            if (BrickManager.Connected)
            {
                await BrickManager.PlayThirdKindAsync(volume: 1, duration: 250);
                //await BrickManager.DirectCommand.StepMotorAtPowerAsync(InputPort.A, 200);
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

        private async void ButtonMotorClick(object sender, EventArgs e)
        {
            if (!(sender is Button)) return;

            char port = ((sender as Button).Text as string).Last();

            if (!ports.ContainsKey(port)) return;

            (sender as Button).IsEnabled = false;

            await BrickManager.DirectCommand.TurnMotorAtPowerForTimeAsync(inputPorts[port], 2000, 100);

            (sender as Button).IsEnabled = true;
        }


        /*
         * Forward button handler
         */
        private async void ButtonForwardClick(object sender, EventArgs e)
        {
            if (!(sender is Button)) return;
            Forward();
        }
        private async void Forward()
        {
            BrickManager.DirectCommand.StartMotorAtPowerAsync(InputPort.C, -100);
            BrickManager.DirectCommand.StartMotorAtPowerAsync(InputPort.D, -100);
        }

        /*
         * Backward button handler
         */
        private async void ButtonBackwardClick(object sender, EventArgs e)
        {
            if (!(sender is Button)) return;
            Backwards();
        }

        private async void Backwards()
        {
            BrickManager.DirectCommand.StartMotorAtPowerAsync(InputPort.C, 100);
            BrickManager.DirectCommand.StartMotorAtPowerAsync(InputPort.D, 100);
        }

        /*
         * Rotqte left button handler
         */
        private async void ButtonRotateLeftClick(object sender, EventArgs e)
        {
            if (!(sender is Button)) return;
            RotateLeft();
        }

        private async void RotateLeft()
        {
            BrickManager.DirectCommand.StartMotorAtPowerAsync(InputPort.C, -100);
            BrickManager.DirectCommand.StartMotorAtPowerAsync(InputPort.D, 100);
            BrickManager.DirectCommand.StartMotorAtPowerAsync(InputPort.A, 100);
        }

        /*
         * Rotate right button handler
         */
        private async void ButtonRotateRightClick(object sender, EventArgs e)
        {
            if (!(sender is Button)) return;
            RotateRight();
        }

        /*
 * Rotate right button handler
 */
        private async void RotateRight()
        {
            BrickManager.DirectCommand.StartMotorAtPowerAsync(InputPort.C, 100);
            BrickManager.DirectCommand.StartMotorAtPowerAsync(InputPort.D, -100);
            BrickManager.DirectCommand.StartMotorAtPowerAsync(InputPort.A, -100);

            
        }

        async void Handle_Clicked(object sender, System.EventArgs e)
        {
            if (BrickManager.Connected)
            {
                await BrickManager.PlayThirdKindAsync(volume: 1, duration: 250);
                await BrickManager.DirectCommand.StepMotorAtPowerAsync(InputPort.A, 200);
            }
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            Hub.OpenScanPage();
        }
    }
}
