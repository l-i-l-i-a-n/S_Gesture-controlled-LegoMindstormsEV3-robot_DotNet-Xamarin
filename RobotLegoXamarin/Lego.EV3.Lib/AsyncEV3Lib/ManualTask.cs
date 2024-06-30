using Lego.Ev3.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncEV3Lib
{
    /// <summary>
    /// Classe abstraite utilisée pour connaitre la fin des actions sur le robot
    /// </summary>
    public abstract class ManualTask
    {
        protected static Dictionary<InputPort, OutputPort> Ports = new Dictionary<InputPort, OutputPort>()
        {
            {InputPort.A, OutputPort.A},
            {InputPort.B, OutputPort.B},
            {InputPort.C, OutputPort.C},
            {InputPort.D, OutputPort.D},
        };

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

        /// <summary>
        /// resets the task to start it one more time
        /// </summary>
        protected void ResetTask()
        {
            tcs = new TaskCompletionSource<bool>();
        }
    }
}
