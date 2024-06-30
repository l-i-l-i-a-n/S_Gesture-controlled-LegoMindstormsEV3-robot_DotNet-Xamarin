using InTheHand.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothDevicesScanner
{
    /// <summary>
    /// detects Bluetooth devices and EV3 devices connected via Bluetooth
    /// </summary>
    public static class BluetoothManager
    {
        const string QueryString = "SELECT Caption,DeviceID,PNPDeviceID FROM Win32_PnPEntity WHERE Caption LIKE '%Bluetooth%(COM%'";

        /// <summary>
        /// scan for bluetooth devices
        /// </summary>
        /// <returns>a read only collection of bluetooth devices</returns>
        /// <remarks>must be called before calling for the properties BluetoothDevices and EV3Devices</remarks>
        public async static Task<ReadOnlyCollection<BluetoothDevice>> FindBluetoothDevices()
        {
            return await Task.Run(() =>
            {
                var temp = ScanDevices().Result.Select(devInfo => new BluetoothDevice
                {
                    DeviceName = devInfo.DeviceName,
                    ClassOfDevice = devInfo.ClassOfDevice,
                    Authenticated = devInfo.Authenticated,
                    Connected = devInfo.Connected,
                    BluetoothAddress = devInfo.DeviceAddress.ToString()
                });

                SelectQuery WMIquery = new SelectQuery(QueryString);
                using (ManagementObjectSearcher WMIqueryResults = new ManagementObjectSearcher(WMIquery))
                {
                    if (WMIqueryResults != null)
                    {
                        foreach (object cur in WMIqueryResults.Get())
                        {
                            ManagementObject mo = (ManagementObject)cur;
                            object id = mo.GetPropertyValue("DeviceID");
                            object pnpId = mo.GetPropertyValue("PNPDeviceID");
                            string caption = mo.GetPropertyValue("Caption").ToString();
                            int indexOfCOMPort = caption.LastIndexOf("COM");
                            string comport = caption.Substring(indexOfCOMPort).Replace(")", "");
                            if (!pnpId.ToString().StartsWith("BTHENUM")) continue;
                            string BTaddress = pnpId.ToString().Split('&')[4].Substring(0, 12);
                            bluetoothDevices.Add(new BluetoothDevice { COMPort = comport, BluetoothAddress = BTaddress });
                        }
                    }
                }
                foreach (var dev in bluetoothDevices)
                {
                    var btDev = temp.SingleOrDefault(d => d.BluetoothAddress == dev.BluetoothAddress);
                    if (btDev != null)
                    {
                        dev.DeviceName = btDev.DeviceName;
                        dev.ClassOfDevice = btDev.ClassOfDevice;
                        dev.Authenticated = btDev.Authenticated;
                        dev.Connected = btDev.Connected;
                    }
                }
                return BluetoothDevices = bluetoothDevices.AsReadOnly();
            });
        }

        private static List<BluetoothDevice> bluetoothDevices = new List<BluetoothDevice>();

        /// <summary>
        /// collection of bluetooth devices detected by this computer
        /// </summary>
        /// <remarks>FindBluetoothDevices must be called before using this property</remarks>
        public static ReadOnlyCollection<BluetoothDevice> BluetoothDevices { get; private set; }

        /// <summary>
        /// collection of EV3 devices detected by this computer
        /// </summary>
        /// <remarks>FindBluetoothDevices must be called before using this property</remarks>
        public static ReadOnlyCollection<BluetoothDevice> EV3Devices { get { return bluetoothDevices.Where(dev => dev.BluetoothAddress.StartsWith("001653") && dev.Authenticated).ToList().AsReadOnly(); } }

        private async static Task<BluetoothDeviceInfo[]> ScanDevices()
        {
            return await Task.Run(() =>
            {
                BluetoothClient bluetoothClient = new BluetoothClient();
                BluetoothDeviceInfo[] devicesInfo = bluetoothClient.DiscoverDevices(100, false, false, false, true);
                return devicesInfo;
            });
        }
    }
}
