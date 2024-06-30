using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncEV3MotorCommandsLib
{
    public class ManualTaskCollection : List<ManualTask>
    {
        async public Task Execute()
        {
            foreach (var mc in this)
            {
                mc.Execute();
            }
            await Task.WhenAll(this.Select(mc => mc.Task));
        }

        public void AddManualTasks(params ManualTask[] c) { this.AddRange(c); }
    }
}
