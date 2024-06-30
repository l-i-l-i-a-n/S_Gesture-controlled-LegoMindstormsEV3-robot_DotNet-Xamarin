using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Lego.Ev3.Core;
using Lego.EV3.PCL;

namespace AsyncEV3Lib
{
    /// <summary>
    /// simplifies connection and disconnection to the brick
    /// allows notification of available input ports
    /// </summary>
    public class BrickManager : INotifyPropertyChanged
    {
        public BrickManager(EV3AbstractFactory factory)
        {
            Factory = factory;
        }

        private EV3AbstractFactory Factory { get; set; }
        /// <summary>
        /// the brick
        /// </summary>
        public Brick Brick
        {
            get; set;
        }

        /// <summary>
        /// true if connected to the brick
        /// </summary>
        public bool Connected
        {
            get { return connected; }
            set { connected = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Connected")); }
        }
        private bool connected = false;

        /// <summary>
        /// true if a motor is connected to the input port A
        /// </summary>
        public bool PortAConnected
        {
            get { return portAConnected; }
            set { portAConnected = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PortAConnected")); }
        }
        private bool portAConnected = false;

        /// <summary>
        /// true if a motor is connected to the input port B
        /// </summary>
        public bool PortBConnected
        {
            get { return portBConnected; }
            set { portBConnected = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PortBConnected")); }
        }
        private bool portBConnected = false;

        /// <summary>
        /// true if a motor is connected to the input port C
        /// </summary>
        public bool PortCConnected
        {
            get { return portCConnected; }
            set { portCConnected = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PortCConnected")); }
        }
        private bool portCConnected = false;

        /// <summary>
        /// true if a motor is connected to the input port D
        /// </summary>
        public bool PortDConnected
        {
            get { return portDConnected; }
            set { portDConnected = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PortDConnected")); }
        }
        private bool portDConnected = false;



        private Port[] inputPorts;

        public Port[] InputPorts
        {
            get { return inputPorts; }
            set { inputPorts = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("InputPorts")); }
        }

        public Port InputPort1
        {
            get { return InputPorts?.ElementAt(0); }
        }

        public Port InputPort2
        {
            get { return InputPorts?.ElementAt(1); }
        }

        public Port InputPort3
        {
            get { return InputPorts?.ElementAt(2); }
        }

        public Port InputPort4
        {
            get { return InputPorts?.ElementAt(3); }
        }

        public bool ButtonEnter
        {
            get { return buttonEnter; }
            private set
            {
                if (buttonEnter != value)
                {
                    buttonEnter = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ButtonEnter)));
                }
            }
        }
        private bool buttonEnter;

        public bool ButtonRight
        {
            get { return buttonRight; }
            private set
            {
                if (buttonRight != value)
                {
                    buttonRight = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ButtonRight)));
                }
            }
        }
        private bool buttonRight;

        public bool ButtonUp
        {
            get { return buttonUp; }
            private set
            {
                if (buttonUp != value)
                {
                    buttonUp = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ButtonUp)));
                }
            }
        }
        private bool buttonUp;

        public bool ButtonLeft
        {
            get { return buttonLeft; }
            private set
            {
                if (buttonLeft != value)
                {
                    buttonLeft = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ButtonLeft)));
                }
            }
        }
        private bool buttonLeft;

        public bool ButtonBack
        {
            get { return buttonBack; }
            private set
            {
                if (buttonBack != value)
                {
                    buttonBack = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ButtonBack)));
                }
            }
        }
        private bool buttonBack;

        public bool ButtonDown
        {
            get { return buttonDown; }
            private set
            {
                if (buttonDown != value)
                {
                    buttonDown = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ButtonDown)));
                }
            }
        }
        private bool buttonDown;




        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// connects the brick
        /// </summary>
        /// <returns></returns>
        public async Task ConnectAsync(string deviceName = "EV3")
        {
            //BluetoothCommunication bc = new BluetoothCommunication();
            Brick = new Brick(Factory.BluetoothCommunication, Factory.SystemCommandFactory, true);
            if (Brick == null) return;
            await Brick.ConnectAsync(deviceName);
            Connected = true;

            InputPorts = Brick.Ports.Values.ToArray();
            Brick.BrickChanged += Brick_BrickChanged;

            DirectCommand = new DirectMotorCommandAsync(Brick);
            BatchCommand = new BatchMotorCommandAsync(Brick);
        }

        /// <summary>
        /// play the theme of "Close Encounters of the Third Kind"
        /// </summary>
        /// <param name="volume">tone volume</param>
        /// <param name="duration">duration of every note</param>
        /// <returns></returns>
        /// <remarks>can be used to indicate that the Brick is connected</remarks>
        public async Task PlayThirdKindAsync(int volume = 10, ushort duration = 300)
        {
            if (Brick == null) return;
            await DirectCommand.PlayToneAsync(volume: volume, frequency: 587, duration: duration);
            await DirectCommand.PlayToneAsync(volume: volume, frequency: 659, duration: duration);
            await DirectCommand.PlayToneAsync(volume: volume, frequency: 523, duration: duration);
            await DirectCommand.PlayToneAsync(volume: volume, frequency: 262, duration: duration);
            await DirectCommand.PlayToneAsync(volume: volume, frequency: 392, duration: duration);
        }

        /// <summary>
        /// checks if input ports are connected every time the brick changes
        /// </summary>
        private async void Brick_BrickChanged(object sender, BrickChangedEventArgs e)
        {
            try
            {
                PortAConnected = await Brick.DirectCommand.ReadyRawAsync(InputPort.A, 0) != int.MinValue;
                PortBConnected = await Brick.DirectCommand.ReadyRawAsync(InputPort.B, 0) != int.MinValue;
                PortCConnected = await Brick.DirectCommand.ReadyRawAsync(InputPort.C, 0) != int.MinValue;
                PortDConnected = await Brick.DirectCommand.ReadyRawAsync(InputPort.D, 0) != int.MinValue;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("InputPorts"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("InputPort1"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("InputPort2"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("InputPort3"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("InputPort4"));
                ButtonLeft = e.Buttons.Left;
                ButtonRight = e.Buttons.Right;
                ButtonUp = e.Buttons.Up;
                ButtonDown = e.Buttons.Down;
                ButtonEnter = e.Buttons.Enter;
                ButtonBack = e.Buttons.Back;
            }
            catch (Exception exc)
            { }
        }


        /// <summary>
        /// disconnects the brick
        /// </summary>
        public async void Disconnect()
        {
            if (Brick == null) return;

            Brick.BrickChanged -= Brick_BrickChanged;

            await EnableTopLine(true);

            await Task.Delay(2000);

            Brick.Disconnect();
            Connected = false;
            PortAConnected = false;
            PortBConnected = false;
            PortCConnected = false;
            PortDConnected = false;
        }

        public DirectMotorCommandAsync DirectCommand { get; private set; }
        public BatchMotorCommandAsync BatchCommand { get; private set; }

        public async Task CreateDirectoryAsync(string directory)
        {
            if (Brick == null) return;

            await Brick.SystemCommand.CreateDirectoryAsync(directory);
        }

        public async Task CopyFileAsync(string localPath, string devicePath)
        {
            if (Brick == null) return;

            await Task.Run(() => Brick.SystemCommand.CopyFileAsync(localPath, devicePath));
        }

        public async Task WriteFileAsync(byte[] data, string devicePath)
        {
            if (Brick == null) return;

            await Task.Run(() => Brick.SystemCommand.WriteFileAsync(data, devicePath));
        }


        public async Task DrawImageAsync(ushort x, ushort y, string filePath, string rootPrjsPath = "/home/root/lms2012/prjs/")
        {
            if (Brick == null) return;

            Brick.BatchCommand.CleanUI();
            Brick.BatchCommand.EnableTopLine(false);
            Brick.BatchCommand.DrawFillWindow(Color.Background, x, y);
            Brick.BatchCommand.DrawImage(Color.Foreground, x, y, $"{rootPrjsPath}{filePath}");
            //Brick.BatchCommand.DrawCircle(Color.Foreground, 20, 20, 20, true);
            Brick.BatchCommand.UpdateUI();
            await Brick.BatchCommand.SendCommandAsync();
        }

        public async Task CleanUIAsync()
        {
            if (Brick == null) return;

            Brick.BatchCommand.CleanUI();
            await Brick.BatchCommand.SendCommandAsync();
        }

        public async Task EnableTopLine(bool enabled)
        {
            if (Brick == null) return;

            Brick.BatchCommand.EnableTopLine(enabled);
            Brick.BatchCommand.UpdateUI();
            await Brick.BatchCommand.SendCommandAsync();
        }
    }
}
