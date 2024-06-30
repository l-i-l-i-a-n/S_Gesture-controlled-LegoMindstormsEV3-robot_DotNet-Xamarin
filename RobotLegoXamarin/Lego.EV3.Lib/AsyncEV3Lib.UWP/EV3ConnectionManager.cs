using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.UI.Core;

namespace AsyncEV3Lib.UWP
{
    public class EV3ConnectionManager : AsyncEV3Lib.EV3ConnectionManager
    {
        private static DeviceWatcher deviceWatcher = null;

        public override void StartUnpairedDeviceWatcher()
        {
            var dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;

            // Request additional properties
            string[] requestedProperties = new string[] { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected" };
            deviceWatcher = DeviceInformation.CreateWatcher("(System.Devices.Aep.ProtocolId:=\"{e0cbf06c-cd8b-4647-bb8a-263b43f0f974}\")",
                                                            requestedProperties,
                                                            DeviceInformationKind.AssociationEndpoint);
            // Hook up handlers for the watcher events before starting the watcher
            deviceWatcher.Added += new TypedEventHandler<DeviceWatcher, DeviceInformation>(async (watcher, deviceInfo) =>
            {
                // Since we have the collection databound to a UI element, we need to update the collection on the UI thread.
                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    // Make sure device name isn't blank
                    //if (deviceInfo.Name != "" && deviceInfo.Id.Contains("00:16:53"))
                    {
                        Devices.Add(new DeviceInfo() { Name = deviceInfo.Name, Id = deviceInfo.Id });
                    }

                });
            });

            deviceWatcher.Updated += new TypedEventHandler<DeviceWatcher, DeviceInformationUpdate>(async (watcher, deviceInfoUpdate) =>
            {
                await dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                {
                });
            });

            deviceWatcher.EnumerationCompleted += new TypedEventHandler<DeviceWatcher, Object>(async (watcher, obj) =>
            {
                await dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                {
                });
            });

            deviceWatcher.Removed += new TypedEventHandler<DeviceWatcher, DeviceInformationUpdate>(async (watcher, deviceInfoUpdate) =>
            {
                await dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                {
                });
            });

            deviceWatcher.Stopped += new TypedEventHandler<DeviceWatcher, Object>(async (watcher, obj) =>
            {
                await dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                {
                    try
                    {
                        Devices.Clear();
                    }
                    catch(Exception e) { }
                });
            });

            deviceWatcher.Start();
        }

        public override void StopWatcher()
        {
            if (null != deviceWatcher)
            {
                if ((DeviceWatcherStatus.Started == deviceWatcher.Status ||
                     DeviceWatcherStatus.EnumerationCompleted == deviceWatcher.Status))
                {
                    //deviceWatcher.Stop();
                }
                deviceWatcher = null;
            }
        }
    }
}
