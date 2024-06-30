using System;
using AsyncEV3Lib.iOS;
using Lego.Ev3.Core;
using Xamarin.Forms;

[assembly: Dependency(typeof(EV3iOSFactory))]
namespace AsyncEV3Lib.iOS
{
    public class EV3iOSFactory : EV3AbstractFactory
    {
        public override AsyncEV3Lib.EV3ConnectionManager EV3ConnectionManager { get => ev3ConnectionManager; }
        private AsyncEV3Lib.EV3ConnectionManager ev3ConnectionManager = new EV3ConnectionManager();

        public override Lego.EV3.PCL.BluetoothCommunication BluetoothCommunication { get => bluetoothCommunication; }

        public override Func<Brick, SystemCommand> SystemCommandFactory => (brick) => new Lego.EV3.iOS.SystemCommand(brick);

        private Lego.EV3.PCL.BluetoothCommunication bluetoothCommunication = new Lego.EV3.iOS.BluetoothCommunication();
    }
}
