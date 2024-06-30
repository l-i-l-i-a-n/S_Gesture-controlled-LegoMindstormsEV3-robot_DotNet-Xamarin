using Lego.Ev3.Core;
using Lego.EV3.PCL;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
//using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
//using Windows.Devices.Bluetooth.Rfcomm;
//using Windows.Devices.Enumeration;
//using Windows.Foundation;
//using Windows.Networking.Sockets;
//using Windows.Storage.Streams;
//using Windows.System.Threading;
//using Windows.UI.Xaml;
//using Windows.UI.Xaml.Controls;

namespace Lego.EV3.PCL
{
    /// <summary>
    /// Communicate with EV3 brick over Bluetooth.
    /// </summary>
    public class BluetoothCommunication : ICommunication
    {
        /// <summary>
        /// Event fired when a complete report is received from the EV3 brick.
        /// </summary>
        public event EventHandler<ReportReceivedEventArgs> ReportReceived;

        //private StreamSocket _socket;
        //private DataReader _reader;
        private CancellationTokenSource _tokenSource;

        private readonly string _deviceName = "EV3";

        /// <summary>
        /// Create a new BluetoothCommunication object
        /// </summary>
        public BluetoothCommunication()
        {
        }

        /// <summary>
        /// Create a new BluetoothCommunication object
        /// </summary>
        /// <param name="device">Devicename of the EV3 brick</param>
        public BluetoothCommunication(string device)
        {
            _deviceName = device;
        }

        /// <summary>
        /// Connect to the EV3 brick.
        /// </summary>
        /// <returns></returns>
        public virtual Task ConnectAsync(string deviceName = null)
        {
            throw new NotImplementedException();
        }

        public virtual async Task<ReadOnlyCollection<string>> FindAllEV3Robots()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Disconnect from the EV3 brick.
        /// </summary>
        public virtual void Disconnect()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Write data to the EV3 brick.
        /// </summary>
        /// <param name="data">Byte array to write to the EV3 brick.</param>
        /// <returns></returns>
        public virtual Task WriteAsync(byte[] data)
        {
            throw new NotImplementedException();
        }

        protected virtual void OnReportReceived(byte[] buffer)
        {
            ReportReceived?.Invoke(this, new ReportReceivedEventArgs { Report = buffer });
        }
    }
    //public sealed class BluetoothCommunication : ICommunication
    //{
    //    /// <summary>
    //    /// Event fired when a complete report is received from the EV3 brick.
    //    /// </summary>
    //    public event EventHandler<ReportReceivedEventArgs> ReportReceived;

    //    //private StreamSocket _socket;
    //    //private DataReader _reader;
    //    private CancellationTokenSource _tokenSource;

    //    private readonly string _deviceName = "EV3";

    //    /// <summary>
    //    /// Create a new BluetoothCommunication object
    //    /// </summary>
    //    public BluetoothCommunication()
    //    {
    //    }

    //    /// <summary>
    //    /// Create a new BluetoothCommunication object
    //    /// </summary>
    //    /// <param name="device">Devicename of the EV3 brick</param>
    //    public BluetoothCommunication(string device)
    //    {
    //        _deviceName = device;
    //    }

    //    /// <summary>
    //    /// Connect to the EV3 brick.
    //    /// </summary>
    //    /// <returns></returns>
    //    public Task ConnectAsync(string deviceName = null)
    //    {
    //        return ConnectAsyncInternal(deviceName);//.AsAsyncAction();
    //    }

    //    //public Task ConnectAsync()
    //    //{
    //    //    return ConnectAsyncInternal();//.AsAsyncAction();
    //    //}

    //    public async Task<DeviceInformationCollection> FindAllEV3Robots()
    //    {
    //        string selector = RfcommDeviceService.GetDeviceSelector(RfcommServiceId.SerialPort);
    //        DeviceInformationCollection devices = await DeviceInformation.FindAllAsync(selector);//, new string[] { "System.Devices.Aep.IsConnected" });
    //        return devices;
    //    }

    //    private async Task ConnectAsyncInternal(string deviceName = null)
    //    {
    //        _tokenSource = new CancellationTokenSource();

    //        //string selector = RfcommDeviceService.GetDeviceSelector(RfcommServiceId.SerialPort);
    //        //DeviceInformationCollection devices = await DeviceInformation.FindAllAsync(selector);
    //        DeviceInformationCollection devices = await FindAllEV3Robots();
    //        //var chooser = new EV3RobotChooser() { DataContext = devices };
    //        //await chooser.ShowAsync();

    //        DeviceInformation device = devices.Where(d => d.Id.Contains(deviceName)).FirstOrDefault();
    //        //(from d in devices where d.Name == deviceName select d).FirstOrDefault();
    //        if (device == null)
    //            throw new Exception("LEGO EV3 brick named '" + deviceName + "' not found.");

    //        RfcommDeviceService service = await RfcommDeviceService.FromIdAsync(device.Id);
    //        if (service == null)
    //            throw new Exception("Unable to connect to LEGO EV3 brick...is the manifest set properly?");

    //        _socket = new StreamSocket();
    //        await _socket.ConnectAsync(service.ConnectionHostName, service.ConnectionServiceName,
    //             SocketProtectionLevel.BluetoothEncryptionAllowNullAuthentication);

    //        _reader = new DataReader(_socket.InputStream);
    //        _reader.ByteOrder = ByteOrder.LittleEndian;

    //        await ThreadPool.RunAsync(PollInput);
    //    }

    //    private async void PollInput(IAsyncAction operation)
    //    {
    //        while (_socket != null)
    //        {
    //            try
    //            {
    //                DataReaderLoadOperation drlo = _reader.LoadAsync(2);
    //                await drlo.AsTask(_tokenSource.Token);
    //                short size = _reader.ReadInt16();
    //                byte[] data = new byte[size];

    //                drlo = _reader.LoadAsync((uint)size);
    //                await drlo.AsTask(_tokenSource.Token);
    //                _reader.ReadBytes(data);

    //                if (ReportReceived != null)
    //                    ReportReceived(this, new ReportReceivedEventArgs { Report = data });
    //            }
    //            catch (TaskCanceledException)
    //            {
    //                return;
    //            }
    //        }
    //    }

    //    /// <summary>
    //    /// Disconnect from the EV3 brick.
    //    /// </summary>
    //    public void Disconnect()
    //    {
    //        _tokenSource.Cancel();
    //        if (_reader != null)
    //        {
    //            try
    //            {
    //                _reader.DetachStream();
    //            }
    //            catch (COMException exc) { }
    //            finally
    //            {
    //                _reader = null;
    //            }
    //        }

    //        if (_socket != null)
    //        {
    //            _socket.Dispose();
    //            _socket = null;
    //        }
    //    }

    //    /// <summary>
    //    /// Write data to the EV3 brick.
    //    /// </summary>
    //    /// <param name="data">Byte array to write to the EV3 brick.</param>
    //    /// <returns></returns>
    //    public Task WriteAsync([ReadOnlyArray]byte[] data)
    //    {
    //        return WriteAsyncInternal(data);//.AsAsyncAction();
    //    }

    //    private async Task WriteAsyncInternal(byte[] data)
    //    {
    //        if (_socket != null)
    //            await _socket.OutputStream.WriteAsync(data.AsBuffer());
    //    }
    //}
}
