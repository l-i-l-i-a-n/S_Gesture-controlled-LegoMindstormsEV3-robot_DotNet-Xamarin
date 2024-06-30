using System;
using System.Collections.Generic;
using System.Diagnostics;
using CoreBluetooth;
using CoreFoundation;
using Foundation;
using System.Timers;
using ExternalAccessory;
using System.Linq;

namespace AsyncEV3Lib.iOS
{
    public class EV3ConnectionManager : AsyncEV3Lib.EV3ConnectionManager
    {
        public EV3ConnectionManager() : base()
        {
            ScanAndAdd();
        }

        private void ScanAndAdd()
        {
            EAAccessoryManager mgr = EAAccessoryManager.SharedAccessoryManager;

            var accessories = mgr.ConnectedAccessories;
            foreach (var accessory in accessories)
            {
                if (accessory.ProtocolStrings.Where(s => s == "COM.LEGO.MINDSTORMS.EV3").Any())
                {
                    var id = accessory.ValueForKey(new NSString(@"macAddress")).ToString();
                    var deviceInfo = new DeviceInfo { Id = id, Name = $"EV3 ({id})" };

                    if (Devices.Contains(deviceInfo))
                        continue;

                    Devices.Add(deviceInfo);
                }
            }
        }

        public override async void StartUnpairedDeviceWatcher()
        {
            EAAccessoryManager mgr = EAAccessoryManager.SharedAccessoryManager;
            try
            {
                await mgr.ShowBluetoothAccessoryPickerAsync(null);
            }
            catch(NSErrorException e)
            {
                Debug.WriteLine(e);
            }

            ScanAndAdd();

            //var accessories = mgr.ConnectedAccessories;
            //foreach (var accessory in accessories)
            //{
            //    if (accessory.ProtocolStrings.Where(s => s == "COM.LEGO.MINDSTORMS.EV3").Any())
            //    {
                    

            //        var session = new EASession(accessory, "COM.LEGO.MINDSTORMS.EV3");

            //        var outputStream = session.OutputStream;
            //        outputStream.Delegate = new MyOutputStreamDelegate();
            //        outputStream.Schedule(NSRunLoop.Current, "kCFRunLoopDefaultMode");
            //        outputStream.Open();
            //    }
            //}
        }

        public override async void StopWatcher()
        {
            
        }
    }




}
