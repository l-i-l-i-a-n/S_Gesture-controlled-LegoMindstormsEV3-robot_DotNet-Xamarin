using AsyncEV3Lib;
using Lego.Ev3.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace UselessBoxController
{
    public class BoxVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public BrickManager BrickManager
        {
            get => brickManager;
            set
            {
                brickManager = value;
                brickManager.Brick.BrickChanged += Brick_BrickChanged;
                OnPropertyChanged();
                Task.Run(() => InitSounds());
                IsOccupied = false;
                PropertyChanged += TouchSensorsChanged;
            }
        }

        private async void TouchSensorsChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName.Equals(nameof(IsLightOn)))
            {
                if (IsLightOn) await TurnOnTheLight();
                else await TurnOffTheLight();
            }
        }

        private BrickManager brickManager;

        private async void Brick_BrickChanged(object sender, BrickChangedEventArgs e)
        {
            MotorB = (sender as Brick).Ports[InputPort.B].SIValue;
            MotorC = (sender as Brick).Ports[InputPort.C].SIValue;

            await UpdateLight();

            await MoveIfDetectsSomeone();
            await Reacts();
        }

        async Task UpdateLight()
        {
            if (BrickManager.InputPort3.SIValue == 1 && !IsLightOn) IsLightOn = true;// await TurnOnTheLight();
            else if (BrickManager.InputPort2.SIValue == 1 && IsLightOn) IsLightOn = false; //await TurnOffTheLight();
        }

        public float MotorB
        {
            get => motorB;
            private set
            {
                motorB = value;
                OnPropertyChanged();
            }
        }
        private float motorB;

        public float MotorC
        {
            get => motorC;
            private set
            {
                motorC = value;
                OnPropertyChanged();
            }
        }
        private float motorC;

        public async Task MoveArm_ToTurnOff(int power = 50, int amplitude = 160)
        {
            if (IsOccupied) return;

            IsOccupied = true;
            await PrivateMoveArm_ToTurnOff(power, amplitude);
            IsOccupied = false;
        }

        private async Task PrivateMoveArm_ToTurnOff(int power = 50, int amplitude = 160)
        {
            await Task.Delay(1000);
            BrickManager.BatchCommand.AddStepMotorAtPowerAsync(InputPort.B, amplitude, power: power, brake: true);
            await BrickManager.BatchCommand.Execute();
            if (IsLightOn) IsLightOn = false;//await TurnOffTheLight();
            BrickManager.BatchCommand.AddStepMotorAtPowerAsync(InputPort.B, -amplitude, power: power, brake: true);
            await BrickManager.BatchCommand.Execute();
            await BrickManager.DirectCommand.StopMotorAsync(InputPort.B, false);
            await Task.Delay(500);
        }

        public async Task MoveSecondaryArm_ToTurnOn(int power = 50, int amplitude = 270)
        {
            if (IsOccupied) return;

            IsOccupied = true;
            BrickManager.BatchCommand.AddStepMotorAtPowerAsync(InputPort.C, -amplitude, power: power, brake: true);
            await BrickManager.BatchCommand.Execute();
            BrickManager.BatchCommand.AddStepMotorAtPowerAsync(InputPort.C, amplitude, power: power, brake: true);
            await BrickManager.BatchCommand.Execute();
            await BrickManager.DirectCommand.StopMotorAsync(InputPort.C, false);
            IsOccupied = false;
        }

        public async Task MoveForward(int power = 50, int amplitude = 360)
        {
            if (IsOccupied) return;

            IsOccupied = true;
            await PrivateMoveForward(power, amplitude);
            IsOccupied = false;
        }

        private async Task PrivateMoveForward(int power = 50, int amplitude = 360)
        {
            BrickManager.BatchCommand.AddStepMotorAtPowerAsync(InputPort.A, amplitude, error:20, power: power, brake: true);
            BrickManager.BatchCommand.AddStepMotorAtPowerAsync(InputPort.D, amplitude, error: 20, power: power, brake: true);
            await BrickManager.BatchCommand.Execute();
            await BrickManager.DirectCommand.StopMotorAsync(InputPort.A, false);
            await BrickManager.DirectCommand.StopMotorAsync(InputPort.D, false);
        }

        public async Task MoveBackward(int power = 50, int amplitude = 360)
        {
            await MoveForward(power, -amplitude);
        }

        public async Task TurnOnTheLight() => await ChangeLight(1);

        public async Task TurnOffTheLight() => await ChangeLight(0);

        private async Task ChangeLight(int on_off)
        {
            //if (IsOccupied) return;

            //IsOccupied = true;
            string url = $@"http://192.168.4.1/gpio2/{on_off}";
            Debug.WriteLine(url);
            //var request = WebRequest.CreateHttp(url);
            //            await request.GetResponseAsync();
            Uri requestUri = new Uri(url);

            using (Windows.Web.Http.HttpClient httpClient = new Windows.Web.Http.HttpClient())
            {
                //var httpResponse = await httpClient.GetAsync(requestUri);
                //httpResponse.EnsureSuccessStatusCode();
                //var httpResponseBody = await httpResponse.Content.ReadAsStringAsync();
                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Head, requestUri))
                {
                    //TODO
                    //System.Runtime.InteropServices.COMException à gérer ici
                    try
                    {
                        await httpClient.SendRequestAsync(request, Windows.Web.Http.HttpCompletionOption.ResponseContentRead);
                    }
                    catch(System.Runtime.InteropServices.COMException exc)
                    {
                        Debug.WriteLine("exception lors de la connection avec le module wifi de la lampe.");
                    }
                }
                IsLightOn = (on_off == 1);
            }
                
            //IsOccupied = false;
        }

        private async Task InitSounds()
        {
            await brickManager.CreateDirectoryAsync("../prjs/test");
            await brickManager.CopyFileAsync(@"Assets/sounds/cheerful.rsf", "../prjs/test/cheerful.rsf");
            await brickManager.CopyFileAsync(@"Assets/sounds/determined.rsf", "../prjs/test/determined.rsf");
            await brickManager.CopyFileAsync(@"Assets/sounds/306.rsf", "../prjs/test/306.rsf");
            await brickManager.CopyFileAsync(@"Assets/sounds/ExcitedR2D2.rsf", "../prjs/test/ExcitedR2D2.rsf");
            await brickManager.CopyFileAsync(@"Assets/sounds/Laughing R2D2.rsf", "../prjs/test/Laughing R2D2.rsf");
            await brickManager.CopyFileAsync(@"Assets/sounds/Sad R2D2.rsf", "../prjs/test/Sad R2D2.rsf");
            await brickManager.CopyFileAsync(@"Assets/sounds/Shocked R2D2.rsf", "../prjs/test/Shocked R2D2.rsf");
            await brickManager.CopyFileAsync(@"Assets/sounds/Unbelievable R2D2.rsf", "../prjs/test/Unbelievable R2D2.rsf");
        }

        public async Task LittleDance()
        {
            if (IsOccupied) return;

            IsOccupied = true;
            await PrivateMoveForward(amplitude:50);
            await Task.Delay(100);
            await PrivateMoveForward(amplitude: -50);
            await Task.Delay(100);
            await PrivateMoveForward(amplitude: 100);
            await Task.Delay(100);
            await PrivateMoveForward(amplitude: -100);
            await brickManager.DirectCommand.PlaySoundAsync("test/cheerful", duration: 1000);
            await brickManager.DirectCommand.PlaySoundAsync("test/determined", duration: 2000);
            IsOccupied = false;
        }

        public async Task LittleVibration()
        {
            if (IsOccupied) return;

            IsOccupied = true;
            for (int i = 0; i < 6; i++)
            {
                await PrivateMoveForward(power:100, amplitude: 50);
                //await Task.Delay(20);
                await PrivateMoveForward(power:100, amplitude: -50);
                //await Task.Delay(20);
            }
            
            await Task.WhenAll(brickManager.DirectCommand.PlaySoundAsync("test/Laughing R2D2", duration: 3000), Task.Run(async () =>
            {
                for (int i = 0; i < 6; i++)
                {
                    await PrivateMoveForward(power: 100, amplitude: 100);
                    //await Task.Delay(20);
                    await PrivateMoveForward(power: 100, amplitude: -100);
                    //await Task.Delay(20);
                }
            }));
            await brickManager.DirectCommand.PlaySoundAsync("test/ExcitedR2D2", duration: 3000);
            IsOccupied = false;
        }

        public async Task LittleSad()
        {
            if (IsOccupied) return;

            IsOccupied = true;
            
            await Task.WhenAll(brickManager.DirectCommand.PlaySoundAsync("test/Sad R2D2", duration: 3000), Task.Run(async () =>
            {
                    await PrivateMoveForward(power: 30, amplitude: -200);
            }));
            await Task.WhenAll(brickManager.DirectCommand.PlaySoundAsync("test/Unbelievable R2D2", duration: 3000), Task.Run(async () =>
            {
                await PrivateMoveForward(power: 30, amplitude: 200);
            }));
            //await brickManager.DirectCommand.PlaySoundAsync("test/ExcitedR2D2", duration: 3000);
            IsOccupied = false;
        }

        public bool IsOccupied
        {
            get => isOccupied;
            set { isOccupied = value; OnPropertyChanged(); OnPropertyChanged("IsReady"); }
        }
        private bool isOccupied = true;

        public bool IsReady { get { return !IsOccupied; } }

        public bool IsDetectingPresence
        {
            get => isDetectingPresence;
            set { isDetectingPresence = value; OnPropertyChanged(); }
        }
        private bool isDetectingPresence = false;

        public bool IsReacting
        {
            get => isReacting;
            set { isReacting = value; OnPropertyChanged(); }
        }
        private bool isReacting = false;

        public bool IsLightOn
        {
            get => isLightOn;
            set
            {
                if (isLightOn == value) return;
                
                isLightOn = value; OnPropertyChanged();
            }
        }
        private bool isLightOn = false;

        public async Task MoveIfDetectsSomeone(int offset = 30)
        {
            if (IsOccupied || !IsDetectingPresence) return;

            IsOccupied = true;
            if (BrickManager.InputPort1.SIValue < offset)
            {
                await PrivateMoveForward();
            }
            IsOccupied = false;
        }

        public async Task Reacts()
        {
            if (IsOccupied || !IsReacting) return;

            IsOccupied = true;
            if (BrickManager.InputPort3.SIValue == 1)
            {
                await PrivateMoveArm_ToTurnOff();
            }
            IsOccupied = false;
        }
    }
}
