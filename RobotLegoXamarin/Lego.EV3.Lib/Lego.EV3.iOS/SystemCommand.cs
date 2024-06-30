using System;
using System.IO;
using System.Threading.Tasks;
using Lego.Ev3.Core;

namespace Lego.EV3.iOS
{
    public class SystemCommand : Lego.Ev3.Core.SystemCommand
    {
        public SystemCommand(Brick brick) : base(brick) { }

        protected override Task<byte[]> GetFileContents(string localPath)
        {
            return Task.FromResult(File.ReadAllBytes(localPath));
        }
    }
}
