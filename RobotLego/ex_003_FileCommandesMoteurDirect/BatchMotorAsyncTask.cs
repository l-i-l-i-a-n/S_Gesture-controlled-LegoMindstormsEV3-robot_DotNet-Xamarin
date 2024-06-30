using Lego.Ev3.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ex_003_FileCommandesMoteurDirect
{
    public class BatchMotorAsyncTask : ManualTask
    {
        static Dictionary<InputPort, OutputPort> Ports = new Dictionary<InputPort, OutputPort>()
        {
            {InputPort.A, OutputPort.A},
            {InputPort.B, OutputPort.B},
            {InputPort.C, OutputPort.C},
            {InputPort.D, OutputPort.D},
        };

        public Dictionary<MotorDataCommand, bool> MotorData { get; } = new Dictionary<MotorDataCommand, bool>();

        public void AddMotorAsyncCommand(InputPort port, int displacement, int error = 3, int power = 70)
        {
            MotorData.Add(new MotorDataCommand(Brick, port, displacement, error, power), false);
            Brick.BatchCommand.StepMotorAtPower(Ports[port], power, (uint)displacement, true);
        }

        new public Task Task
        {
            get
            {
                return Task.WhenAny(base.tcs.Task, Task.Delay(5000));
            }
        }

        public override async Task Execute()
        {
            foreach(var md in MotorData.Keys)
            {
                md.PropertyChanged += Md_PropertyChanged;
            }
            await Brick.BatchCommand.SendCommandAsync();
            await Task;
        }

        private void Md_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            MotorDataCommand md = sender as MotorDataCommand;
            if (md == null) return;
            if (md.EndValue - Brick.Ports[md.Port].RawValue < md.Error
                && md.EndValue - Brick.Ports[md.Port].RawValue > -md.Error)
                MotorData[md] = true;

            if(MotorData.Values.Count(v => !v) == 0)
                this.SetResult(true);
        }
    }
}
