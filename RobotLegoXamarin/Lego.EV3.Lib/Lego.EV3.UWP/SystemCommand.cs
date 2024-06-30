using Lego.Ev3.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

using System.Runtime.InteropServices.WindowsRuntime;
using System.IO;

namespace Lego.EV3.UWP
{
    public class SystemCommand : Lego.Ev3.Core.SystemCommand
    {
        public SystemCommand(Brick brick) : base(brick) { }
        protected override Task<byte[]> GetFileContents(string localPath)
        {
            //StorageFile sf = await StorageFile.GetFileFromPathAsync(localPath);
            //IBuffer buffer = await FileIO.ReadBufferAsync(sf);
            //return buffer.ToArray();
            return Task.FromResult(File.ReadAllBytes(localPath));

        }
    }
}
