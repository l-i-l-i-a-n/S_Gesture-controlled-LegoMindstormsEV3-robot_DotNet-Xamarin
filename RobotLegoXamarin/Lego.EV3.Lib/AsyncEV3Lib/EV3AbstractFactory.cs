using Lego.Ev3.Core;
using Lego.EV3.PCL;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsyncEV3Lib
{
    public interface IEV3AbstractFactory
    {
        EV3ConnectionManager EV3ConnectionManager { get; }

        BluetoothCommunication BluetoothCommunication { get; }

        Func<Brick, SystemCommand> SystemCommandFactory { get; }
    }

    public abstract class EV3AbstractFactory : IEV3AbstractFactory
    {
        public abstract EV3ConnectionManager EV3ConnectionManager { get; }

        public abstract BluetoothCommunication BluetoothCommunication { get; }

        public abstract Func<Brick, SystemCommand> SystemCommandFactory { get; }
    }
}
