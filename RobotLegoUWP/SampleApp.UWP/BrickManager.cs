using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lego.Ev3.Core;
using Lego.Ev3.UWP;

namespace SampleApp.UWP
{
    public class BrickManager : INotifyPropertyChanged
    {
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

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// connects the brick
        /// </summary>
        /// <returns></returns>
        public async Task ConnectAsync(string deviceName = "EV3")
        {
            //BluetoothCommunication bc = new BluetoothCommunication();
            Brick = new Brick(new BluetoothCommunication(),true);
            if (Brick == null) return;
            await Brick.ConnectAsync(deviceName);
            Connected = true;

            Brick.BrickChanged += Brick_BrickChanged;
        }

        /// <summary>
        /// checks if input ports are connected every time the brick changes
        /// </summary>
        private async void Brick_BrickChanged(object sender, BrickChangedEventArgs e)
        {
            //if (!Connected) return;
            try
            {
                PortAConnected = await Brick.DirectCommand.ReadyRawAsync(InputPort.A, 0) != int.MinValue;
                PortBConnected = await Brick.DirectCommand.ReadyRawAsync(InputPort.B, 0) != int.MinValue;
                PortCConnected = await Brick.DirectCommand.ReadyRawAsync(InputPort.C, 0) != int.MinValue;
                PortDConnected = await Brick.DirectCommand.ReadyRawAsync(InputPort.D, 0) != int.MinValue;
            }
            catch(Exception exc)
            { }
        }


        /// <summary>
        /// disconnects the brick
        /// </summary>
        public async void Disconnect()
        {
            Brick.BrickChanged -= Brick_BrickChanged;

            await Task.Delay(2000);

            Brick.Disconnect();
            Connected = false;
            PortAConnected = false;
            PortBConnected = false;
            PortCConnected = false;
            PortDConnected = false;
        }
    }
}
