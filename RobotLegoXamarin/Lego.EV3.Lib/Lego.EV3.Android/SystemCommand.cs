using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;
using Lego.Ev3.Core;

namespace Lego.EV3.Android
{
    public class SystemCommand : Lego.Ev3.Core.SystemCommand
    {
        public SystemCommand(Brick brick) : base(brick) { }

        protected override async Task<byte[]> GetFileContents(string localPath)
        {
            IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication();
            IsolatedStorageFileStream stream = isf.OpenFile(localPath, FileMode.Open);

            byte[] data = new byte[stream.Length];

            await stream.ReadAsync(data, 0, data.Length);

            return data;
        }
    }
}
