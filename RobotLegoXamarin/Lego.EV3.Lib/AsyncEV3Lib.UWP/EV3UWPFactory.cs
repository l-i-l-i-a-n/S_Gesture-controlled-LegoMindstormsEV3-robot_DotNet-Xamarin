using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using System.Text;
using System.Threading.Tasks;
using AsyncEV3Lib.UWP;
using Lego.Ev3.Core;

[assembly: Dependency(typeof(EV3UWPFactory))]
namespace AsyncEV3Lib.UWP
{
    public class EV3UWPFactory : EV3AbstractFactory, IEV3AbstractFactory
    {
        public override AsyncEV3Lib.EV3ConnectionManager EV3ConnectionManager { get => ev3ConnectionManager; }
        private AsyncEV3Lib.EV3ConnectionManager ev3ConnectionManager = new EV3ConnectionManager();

        public override Lego.EV3.PCL.BluetoothCommunication BluetoothCommunication { get => bluetoothCommunication; }

        public override Func<Brick, SystemCommand> SystemCommandFactory => (brick) => new Lego.EV3.UWP.SystemCommand(brick);

        private Lego.EV3.PCL.BluetoothCommunication bluetoothCommunication = new Lego.EV3.UWP.BluetoothCommunication();
    }
}
