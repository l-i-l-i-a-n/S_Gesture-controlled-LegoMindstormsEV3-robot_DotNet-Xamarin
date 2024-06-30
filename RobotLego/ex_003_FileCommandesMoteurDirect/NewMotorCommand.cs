using Lego.Ev3.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ex_003_FileCommandesMoteurDirect
{
    /// <summary>
    /// Classe abstraite utilisée pour connaitre la fin des actions sur le robot
    /// </summary>
    public abstract class ManualTask
    {
        /// <summary>
        /// La brick du robot
        /// </summary>
        public static Brick Brick { get; set; }

        protected TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

        public abstract Task Execute();

        /// <summary>
        /// Tache en cours
        /// </summary>
        public Task Task
        {
            get { return tcs.Task; }
        }

        /// <summary>
        /// Set la fin de la tache
        /// </summary>
        /// <param name="result"></param>
        public void SetResult(bool result)
        {
            tcs.TrySetResult(result);
        }
    }

    public class DirectMotorAsyncCommand : ManualTask
    {
        static Dictionary<InputPort, OutputPort> Ports = new Dictionary<InputPort, OutputPort>()
        {
            {InputPort.A, OutputPort.A},
            {InputPort.B, OutputPort.B},
            {InputPort.C, OutputPort.C},
            {InputPort.D, OutputPort.D},
        };

        public MotorDataCommand MotorData { get; private set; }

        public DirectMotorAsyncCommand(InputPort port, int displacement, int error = 3, int power = 70)
        {
            MotorData = new MotorDataCommand(Brick, port, displacement, error, power);
            MotorData.PropertyChanged += Port_PropertyChanged;
        }

        new public Task Task
        {
            get
            {
                return Task.WhenAny(base.tcs.Task, Task.Delay(5000));
            }
        }

        /// <summary>
        /// Test pour la connaitre la fin de l'instruction envoyée a la brique
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Port_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (this.MotorData.EndValue - Brick.Ports[MotorData.Port].RawValue < this.MotorData.Error
                && this.MotorData.EndValue - Brick.Ports[MotorData.Port].RawValue > -this.MotorData.Error)
                this.SetResult(true);
        }

        /// <summary>
        /// Execute l'action demandée sur le moteur
        /// </summary>
        public override async Task Execute()
        {
            await Brick.DirectCommand.StepMotorAtPowerAsync(Ports[MotorData.Port], MotorData.Power, (uint)MotorData.Displacement, true);
            await Task;
        }
    }

}
