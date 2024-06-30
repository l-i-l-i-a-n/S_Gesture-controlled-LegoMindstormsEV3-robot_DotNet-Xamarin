using Lego.Ev3.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ex_003_FileCommandesMoteurDirect
{
    class ManualTaskList : List<ManualTask>
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
