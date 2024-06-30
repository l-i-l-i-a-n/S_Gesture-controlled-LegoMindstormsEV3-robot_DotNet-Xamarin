using InTheHand.Net.Bluetooth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothDevicesScanner
{
    /// <summary>
    /// data relative to a bluetooth device
    /// </summary>
    public class BluetoothDevice
    {
        /// <summary>
        /// name of this device (if it has one)
        /// </summary>
        public string DeviceName { get; internal set; } = "unknown";

        /// <summary>
        /// class of this device
        /// </summary>
        public ClassOfDevice ClassOfDevice { get; internal set; }

        /// <summary>
        /// indicates if it is currently authenticated or not
        /// </summary>
        public bool Authenticated { get; internal set; }

        /// <summary>
        /// indicates if it is currently connected or not
        /// </summary>
        public bool Connected { get; internal set; }

        /// <summary>
        /// indicates the id of this device
        /// </summary>
        public string DeviceID { get; internal set; }

        public string PnpDeviceID { get; internal set; }

        public string COMPort { get; internal set; }

        /// <summary>
        /// bluetooth address of this device
        /// </summary>
        public string BluetoothAddress { get; internal set; }

        public override string ToString()
        {
            return $"{DeviceName}\n\tCOMPort : {COMPort}\n\tBluetoothAddress : {BluetoothAddress}";
        }
    }
}
