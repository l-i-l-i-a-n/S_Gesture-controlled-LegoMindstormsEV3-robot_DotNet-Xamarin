using BluetoothDevicesScanner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBluetoothDevicesScanner
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("*** Detailed method (wait for results and then press a key) ***");
            PrintDevices();
            Console.ReadKey();

            Console.WriteLine("*** Short method to find the only ev3 device authenticated (wait for results and then press a key) ***");
            FindOneConnectedEV3Device();
            Console.ReadKey();
        }

        async static void PrintDevices()
        {
            await BluetoothManager.FindBluetoothDevices();

            if(BluetoothManager.BluetoothDevices.Count == 0)
            {
                Console.WriteLine("No bluetooth devices detected");
                return;
            }

            Console.WriteLine("Bluetooth devices are :");
            foreach(var dev in BluetoothManager.BluetoothDevices)
            {
                Console.WriteLine($"* \t{dev.DeviceName}\n\t\taddress: {dev.BluetoothAddress}\n\t\tconnected: {dev.Authenticated}");
            }

            if(BluetoothManager.EV3Devices.Count == 0)
            {
                Console.WriteLine("No EV3 devices detected");
                return;
            }

            Console.WriteLine("\nEV3 devices are :");
            foreach (var ev3 in BluetoothManager.EV3Devices)
            {
                Console.WriteLine($"* \t{ev3.DeviceName}\n\t\taddress: {ev3.BluetoothAddress}\n\t\tconnected: {ev3.Authenticated}\n\t\tCOM Port: {ev3.COMPort}");
            }
        }

        async static void FindOneConnectedEV3Device()
        {
            await BluetoothManager.FindBluetoothDevices();

            string comport = BluetoothManager.EV3Devices.FirstOrDefault()?.COMPort;

            if(string.IsNullOrWhiteSpace(comport))
            {
                Console.WriteLine("No EV3 device connected");
                return;
            }

            Console.WriteLine($"I have found one EV3 device on {comport}");
        }
    }
}
