using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CoreFoundation;
using ExternalAccessory;
using Foundation;
using Lego.Ev3.Core;

namespace Lego.EV3.iOS
{
    public class BluetoothCommunication : Lego.EV3.PCL.BluetoothCommunication, ICommunication
    {
        MyStreamDelegate StreamDelegate { get; set; }
        public static EASession Session { get; set; }
        bool IsClosed { get; set; } = true;
        public static List<byte[]> WriteBuffer { get; set; } = new List<byte[]>();
        public static byte[] DataBuffer { get; set; } 

        public override System.Threading.Tasks.Task<System.Collections.ObjectModel.ReadOnlyCollection<string>> FindAllEV3Robots()
        {
            return base.FindAllEV3Robots();
        }

        public override Task ConnectAsync(string deviceName = null)
        {
            return Task.Run(() =>
            {
                
                if (!IsClosed) return;

                EAAccessoryManager mgr = EAAccessoryManager.SharedAccessoryManager;
                var accessories = mgr.ConnectedAccessories;

                EAAccessory accessory = accessories.SingleOrDefault(a => a.ProtocolStrings.Any(s => s.Equals("COM.LEGO.MINDSTORMS.EV3"))
                                                                    && deviceName.Contains(a.ValueForKey(new NSString(@"macAddress")).ToString()));
                if (accessory == null)
                    throw new NullReferenceException($"wrong deviceName or device not connected ({deviceName})");


                Session = new EASession(accessory, "COM.LEGO.MINDSTORMS.EV3");
                StreamDelegate = new MyStreamDelegate(this);

                var inputStream = Session.InputStream;
                inputStream.Delegate = StreamDelegate;
                inputStream.Schedule(NSRunLoop.Main, Foundation.NSRunLoopMode.Default);

                var outputStream = Session.OutputStream;
                outputStream.Delegate = StreamDelegate;
                outputStream.Schedule(NSRunLoop.Main, Foundation.NSRunLoopMode.Default);
                inputStream.Open();
                outputStream.Open();

                NSRunLoop.Main.RunUntil(NSDate.FromTimeIntervalSinceNow(0.5));

                IsClosed = false;

                Task t = Task.Factory.StartNew(()=> PollInput());
                //await Windows.System.Threading.ThreadPool.RunAsync(PollInput);


            });
                
        }

        private byte[] sizeBuffer = new byte[2];

        public async Task PollInput()
        {
            var queue = new DispatchQueue("com.ev3ios.connection.queue");
            await Task.Run(() => queue.DispatchAsync(() =>
            {
                while (!IsClosed)
                {
                    if (Session.InputStream.HasBytesAvailable())
                    {
                        nint result = Session.InputStream.Read(sizeBuffer, (nuint)2);
                        if (result > 0)
                        {
                            nint size = (sizeBuffer[1]) << 8 | (sizeBuffer[0]);
                            if (size > 0)
                            {
                                var buffer = new byte[size];
                                var result2 = Session.InputStream.Read(buffer, (nuint)size);

                                if (result2 < 1)
                                {
                                    return;
                                }
                                //Debug.WriteLine($"buffer : {buffer}");
                                OnReportReceived(buffer);
                            }
                        }
                    }
                }
            }));
            /*while (_socket != null)
            {
                try
                {
                    DataReaderLoadOperation drlo = _reader.LoadAsync(2);
                    await drlo.AsTask(_tokenSource.Token);
                    short size = _reader.ReadInt16();
                    byte[] data = new byte[size];

                    drlo = _reader.LoadAsync((uint)size);
                    await drlo.AsTask(_tokenSource.Token);
                    _reader.ReadBytes(data);

                    if (ReportReceived != null)
                        ReportReceived(this, new ReportReceivedEventArgs { Report = data });
                }
                catch (TaskCanceledException)
                {
                    return;
                }
            }*/
        }

        public override void Disconnect()
        {
            if (IsClosed) return;

            Session.OutputStream.Close();
            Session.OutputStream.Unschedule(NSRunLoop.Main, Foundation.NSRunLoop.NSDefaultRunLoopMode);
            Session.OutputStream.Delegate = null;

            Session.InputStream.Close();
            Session.InputStream.Unschedule(NSRunLoop.Main, Foundation.NSRunLoop.NSDefaultRunLoopMode);
            Session.InputStream.Delegate = null;


            IsClosed = true;
        }

        public static bool CanWrite { get; set; } = true;

        public static int MaxLength { get; set; } = 128;

        public static int NbBytesLeftToWrite { get; set; } = 0;

        public override async  Task WriteAsync(byte[] data)
        {
            //WriteBuffer.Add(data);
            //if (WriteBuffer.Count == 1) NbBytesLeftToWrite = WriteBuffer[0].Length;
            //Debug.WriteLine($"WriteAsync : {WriteBuffer.Count}");

            //await StreamDelegate.Write();

            //var queue = new DispatchQueue("com.ev3ios.connection.queue");
            //await Task.Run(() => queue.DispatchAsync(() =>
            //{
            //    if (!CanWrite) return;
            //    CanWrite = false;

            //    DataBuffer = WriteBuffer[0];


            //    //dismissCommandsIfNeeded
            //    if (Session?.OutputStream?.HasSpaceAvailable() == false)
            //    {
            //        Debug.WriteLine("WriteAsync : HasSpaceAvailable = false");
            //        return;
            //    }

            //    NSRunLoop.Main.RunUntil(NSDate.FromTimeIntervalSinceNow(0.5));
            //    //dismissCommandsIfNeeded ???
            //    if (Session?.OutputStream?.HasSpaceAvailable() == false)
            //    {
            //        Debug.WriteLine("WriteAsync : HasSpaceAvailable = false");
            //        return;
            //    }

            //    Debug.WriteLine($"bytesLeftToWrite : {data.Length}");
            //    int size = Math.Min(data.Length, MaxLength);
            //    var bytesWritten = Session?.OutputStream?.Write(data, (nuint)size);
            //    Debug.WriteLine($"size : {size}");
            //    Debug.WriteLine($"bytesWritten : {bytesWritten}");

            //    NbBytesLeftToWrite = data.Length - size;
            //    if (NbBytesLeftToWrite == 0)
            //    {
            //        WriteBuffer.RemoveAt(0);
            //        if (WriteBuffer.Count > 0) NbBytesLeftToWrite = WriteBuffer[0].Length;
            //    }

            //    System.Threading.Thread.Sleep(125);
            //    CanWrite = true;
            //    //else
            //      //  NSRunLoop.Main.RunUntil(NSDate.FromTimeIntervalSinceNow(0.5));

            //}));
            //await Task.Delay(200);
            //await Task.Run(() =>
            //{
            //    if (Session?.OutputStream?.HasSpaceAvailable() == true)
            //        Session?.OutputStream?.Write(data);
            //});


            var queue = new DispatchQueue("com.ev3ios.connection.queue");
            await Task.Run(() => queue.DispatchAsync(() =>
            {
                //if (!CanWrite) return;
                //CanWrite = false;

               // DataBuffer = WriteBuffer[0];


                //dismissCommandsIfNeeded
                if (Session?.OutputStream?.HasSpaceAvailable() == false)
                {
                    Debug.WriteLine("WriteAsync : HasSpaceAvailable = false");
                    return;
                }

                NSRunLoop.Main.RunUntil(NSDate.FromTimeIntervalSinceNow(0.5));
                //dismissCommandsIfNeeded ???
                if (Session?.OutputStream?.HasSpaceAvailable() == false)
                {
                    Debug.WriteLine("WriteAsync : HasSpaceAvailable = false");
                    return;
                }

                Debug.WriteLine($"bytesLeftToWrite : {data.Length}");
                //int size = Math.Min(data.Length, MaxLength);
                var bytesWritten = Session?.OutputStream?.Write(data);
                //Debug.WriteLine($"size : {size}");
                //Debug.WriteLine($"bytesWritten : {bytesWritten}");

                //NbBytesLeftToWrite = data.Length - size;
                //if (NbBytesLeftToWrite == 0)
                //{
                //    WriteBuffer.RemoveAt(0);
                //    if (WriteBuffer.Count > 0) NbBytesLeftToWrite = WriteBuffer[0].Length;
                //}

                //System.Threading.Thread.Sleep(10);
                //CanWrite = true;
                //else
                  //  NSRunLoop.Main.RunUntil(NSDate.FromTimeIntervalSinceNow(0.5));

            }));
            //await Task.Delay(200);
            //await Task.Run(() =>
            //{
            //    if (Session?.OutputStream?.HasSpaceAvailable() == true)
            //        Session?.OutputStream?.Write(data);
            //});
        }
    }

    public class MyStreamDelegate : NSStreamDelegate
    {
        BluetoothCommunication Comm { get; set; }

        public MyStreamDelegate(BluetoothCommunication comm) : base()
        {
            Comm = comm;
        }

        public async Task Write()
        {
            if (!BluetoothCommunication.CanWrite) return;
                BluetoothCommunication.CanWrite = false;
                BluetoothCommunication.DataBuffer = BluetoothCommunication.WriteBuffer[0];

                var queue = new DispatchQueue("com.ev3ios.connection.queue");
                await Task.Run(() => queue.DispatchAsync(() =>
                {
                    if (BluetoothCommunication.Session?.OutputStream?.HasSpaceAvailable() == false)
                    {
                        Debug.WriteLine("WriteAsync : HasSpaceAvailable = false");
                        return;
                    }


                    NSRunLoop.Main.RunUntil(NSDate.FromTimeIntervalSinceNow(0.5));
                    //dismissCommandsIfNeeded ???
                    if (BluetoothCommunication.Session?.OutputStream?.HasSpaceAvailable() == false)
                    {
                        Debug.WriteLine("WriteAsync : HasSpaceAvailable = false");
                        return;
                    }


                    Debug.WriteLine($"HandleEvent");
                    Debug.WriteLine($"bytesLeftToWrite : {BluetoothCommunication.NbBytesLeftToWrite}");
                    int size = Math.Min(BluetoothCommunication.NbBytesLeftToWrite, BluetoothCommunication.MaxLength);
                    var bytesWritten = BluetoothCommunication.Session?.OutputStream?.Write(BluetoothCommunication.DataBuffer, 
                                                                                           BluetoothCommunication.DataBuffer.Length - BluetoothCommunication.NbBytesLeftToWrite,
                                                                                           (nuint)size);
                    Debug.WriteLine($"size : {size}");
                    Debug.WriteLine($"bytesWritten : {bytesWritten}");

                    BluetoothCommunication.NbBytesLeftToWrite -= size;

                    if (BluetoothCommunication.NbBytesLeftToWrite == 0)
                    {
                        BluetoothCommunication.WriteBuffer.RemoveAt(0);
                        if (BluetoothCommunication.WriteBuffer.Count > 0) BluetoothCommunication.NbBytesLeftToWrite = BluetoothCommunication.WriteBuffer[0].Length;
                    }

                    //////////////System.Threading.Thread.Sleep(125);
                    BluetoothCommunication.CanWrite = true;
                    //else
                    //  NSRunLoop.Main.RunUntil(NSDate.FromTimeIntervalSinceNow(0.5));

                }));
                //await Task.Delay(200);
                //await Task.Run(() =>
                //{
                //    if (Session?.OutputStream?.HasSpaceAvailable() == true)
                //        Session?.OutputStream?.Write(data);
                //});



        }

        public override async void HandleEvent(NSStream theStream, NSStreamEvent streamEvent)
        {
            //Debug.WriteLine($"HandleEvent : {BluetoothCommunication.WriteBuffer.Count}");

            //if(streamEvent == NSStreamEvent.HasSpaceAvailable
            //   && BluetoothCommunication.WriteBuffer.Count > 0)
            //{
            //    await Write();
            //}

            //if(streamEvent == NSStreamEvent.HasBytesAvailable)
            //{
            //    await Comm.PollInput();
            //}
        }

    }

    //public class MyOutputStreamDelegate : NSStreamDelegate
    //{


    //    public MyOutputStreamDelegate()
    //    {
    //    }
    //    public override void HandleEvent(NSStream theStream, NSStreamEvent streamEvent)
    //    {
    //        if (streamEvent == NSStreamEvent.HasSpaceAvailable)
    //        {
    //            var test = ((NSOutputStream)theStream);
    //            //Set the color of the Sphero
    //            //var written = ((NSOutputStream)theStream).Write(new byte[] { 0xFF, 0xFF, 0x02, 0x20, 0x0e, 0x05, 0x1F, 0xFF, 0x1B, 0x00, 0x91 }, 11);
    //            //if (written == 11)
    //            //{
    //            //    label.Text = "Sphero should be green";
    //            //}
    //            //hasWritten = true;
    //        }
    //    }
    //}
}
