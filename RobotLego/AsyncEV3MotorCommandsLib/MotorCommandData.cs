using Lego.Ev3.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncEV3MotorCommandsLib
{
    class MotorCommandData : INotifyPropertyChanged
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

        public bool Brake { get; private set; }

        public MotorCommandData(Brick brick, InputPort port, int displacement, int error = 3, int power = 70, bool brake = true)
        {
            Brick = brick;
            Port = port;
            Displacement = displacement;
            Error = error;
            if (power > 100) power = 100;
            if (power < 0) power = 0;
            Power = power * Math.Sign(Displacement);
            Displacement = Math.Abs(Displacement);
            Brake = brake;
        }

        public void InitTask()
        {
            StartValue = Brick.Ports[Port].RawValue;
            EndValue = StartValue + Displacement;
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { Brick.Ports[Port].PropertyChanged += value; }
            remove { Brick.Ports[Port].PropertyChanged -= value; }
        }
    }
}
