using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncEV3Lib
{
    public abstract class EV3ConnectionManager
    {
        public ObservableCollection<DeviceInfo> Devices => devices;
        private ObservableCollection<DeviceInfo> devices = new ObservableCollection<DeviceInfo>();

        public abstract void StartUnpairedDeviceWatcher();

        public abstract void StopWatcher();
    }
}