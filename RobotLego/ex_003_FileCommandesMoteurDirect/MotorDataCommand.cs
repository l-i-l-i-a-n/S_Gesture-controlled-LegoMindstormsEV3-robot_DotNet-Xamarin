using Lego.Ev3.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ex_003_FileCommandesMoteurDirect
{
    public class MotorDataCommand
    {
        /// <summary>
        /// La brick du robot
        /// </summary>
        public Brick Brick { get; set; }

        public int StartValue { get; private set; }
        public int EndValue { get; private set; }
        public int Error { get; private set; }
        public int Power { get; private set; }
        public InputPort Port { get; private set; }
        public int Displacement { get; private set; }

        public MotorDataCommand(Brick brick, InputPort port, int displacement, int error = 3, int power = 70)
        {
            Brick = brick;
            Port = port;
            StartValue = Brick.Ports[Port].RawValue;
            Displacement = displacement;
            EndValue = StartValue + Displacement;
            Error = error;
            if (power > 100) power = 100;
            if (power < 0) power = 0;
            Power = power * Math.Sign(Displacement);
            //Brick.Ports[Port].PropertyChanged += Port_PropertyChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { Brick.Ports[Port].PropertyChanged += value; }
            remove { Brick.Ports[Port].PropertyChanged -= value; }
        }
    }
}
