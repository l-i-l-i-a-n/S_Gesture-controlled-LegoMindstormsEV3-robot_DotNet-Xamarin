using System;
using System.Linq;
using Android.Bluetooth;

namespace AsyncEV3Lib.Android
{
    public class EV3ConnectionManager : AsyncEV3Lib.EV3ConnectionManager
    {

        public override void StartUnpairedDeviceWatcher()
        {
            BluetoothAdapter adapter = BluetoothAdapter.DefaultAdapter;
            if (adapter == null)
                throw new Exception("No Bluetooth adapter found");

            if (!adapter.IsEnabled)
                throw new Exception("Bluetooth adapter is not enabled");

            if(!adapter.IsDiscovering)
            {
                adapter.StartDiscovery();
            }

            foreach (var dev in adapter.BondedDevices)
            {
                Devices.Add(new DeviceInfo { Id = dev.Address, Name = dev.Name });
            }
        }

        public override void StopWatcher()
        {
            BluetoothAdapter adapter = BluetoothAdapter.DefaultAdapter;
            if (adapter == null)
                throw new Exception("No Bluetooth adapter found");

            if (!adapter.IsEnabled)
                throw new Exception("Bluetooth adapter is not enabled");

            if (adapter.IsDiscovering)
            {
                adapter.CancelDiscovery();
            }
        }
    }
}
