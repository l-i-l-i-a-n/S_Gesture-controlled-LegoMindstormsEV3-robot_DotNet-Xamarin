using System;
using Lego.Ev3.Core;
using Xamarin.Forms;
using AsyncEV3Lib.Android;

[assembly: Dependency(typeof(EV3AndroidFactory))]
namespace AsyncEV3Lib.Android
{
    public class EV3AndroidFactory : EV3AbstractFactory
    {
        public override AsyncEV3Lib.EV3ConnectionManager EV3ConnectionManager { get => ev3ConnectionManager; }
        private AsyncEV3Lib.EV3ConnectionManager ev3ConnectionManager = new EV3ConnectionManager();

        public override Lego.EV3.PCL.BluetoothCommunication BluetoothCommunication { get => bluetoothCommunication; }

        public override Func<Brick, SystemCommand> SystemCommandFactory => (brick) => new Lego.EV3.Android.SystemCommand(brick);

        private Lego.EV3.PCL.BluetoothCommunication bluetoothCommunication = new Lego.EV3.Android.BluetoothCommunication();
    }
}
