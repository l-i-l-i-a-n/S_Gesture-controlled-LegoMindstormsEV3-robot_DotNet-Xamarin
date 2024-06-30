using Lego.Ev3.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AsyncEV3Lib
{
    public class DirectMotorCommandAsync : ManualTask
    {
        /// <summary>
        /// input port to work with (in case of motors)
        /// </summary>
        private InputPort inputPort;

        /// <summary>
        /// used for motors with specified number of steps displacement
        /// </summary>
        MotorCommandData MotorData { get; set; }

        /// <summary>
        /// the EV3 brick
        /// </summary>
        Brick Brick { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="brick">the EV3 brick to work with</param>
        public DirectMotorCommandAsync(Brick brick)
        {
            Brick = brick;
        }

        /// <summary>
        /// turns a motor with a specified number of steps
        /// </summary>
        /// <param name="port">input port of the motor</param>
        /// <param name="displacement">number of steps</param>
        /// <param name="error">accepted error</param>
        /// <param name="power">power (%), can be negative</param>
        /// <param name="brake">true to use the brake at the end of the command</param>
        public async Task StepMotorAtPowerAsync(InputPort port, int displacement, int error = 3, int power = 70, bool brake = true)
        {
            MotorData = new MotorCommandData(Brick, port, displacement, error, power, brake);
            MotorData.PropertyChanged += Port_PropertyChanged;
            await Execute();
            MotorData.PropertyChanged -= Port_PropertyChanged;
        }

        /// <summary>
        /// turns the motor during a certain amount of milliseconds
        /// </summary>
        /// <param name="port">input port of the motor</param>
        /// <param name="milliseconds">time in milliseconds</param>
        /// <param name="power">power (%), can be negative</param>
        /// <param name="brake">true to use the brake at the end of the command</param>
        public async Task TurnMotorAtPowerForTimeAsync(InputPort port, int milliseconds, int power = 70, bool brake = true)
        {
            await StartMotorAtPowerAsync(port, power);
            await Task.Delay(milliseconds);
            await StopMotorAsync(port, brake);
        }

        /// <summary>
        /// starts the motor
        /// </summary>
        /// <param name="port">input port of the motor</param>
        /// <param name="power">power (%), can be negative</param>
        public async Task StartMotorAtPowerAsync(InputPort port, int power = 70)
        {
            await Brick.DirectCommand.TurnMotorAtPowerAsync(Ports[port], power);
        }

        /// <summary>
        /// stops the motor
        /// </summary>
        /// <param name="port">input port of the motor</param>
        /// <param name="brake">true to use the brake at the end of the command</param>
        public async Task StopMotorAsync(InputPort port, bool brake = true)
        {
            await Brick.DirectCommand.StopMotorAsync(Ports[port], brake);
        }

        /// <summary>
        /// plays a tone
        /// </summary>
        /// <param name="frequency">tone frequency</param>
        /// <param name="duration">tone duration</param>
        /// <param name="volume">tone volume on the brick (independent of the volume indicated on the brick)</param>
        public async Task PlayToneAsync(ushort frequency = 200, ushort duration = 1000, int volume = 100)
        {
            await Task.WhenAll(Brick.DirectCommand.PlayToneAsync(volume, frequency, duration), Task.Delay((int)duration));
        }

        /// <summary>
        /// plays a sound
        /// </summary>
        /// <param name="filepath">path to the file of the sound file (from the prjs folder)</param>
        /// <param name="volume">sound volume on the brick (independent of the volume indicated on the brick)</param>
        /// <param name="rootPrjsPath">root folder where the sounds are stored</param>
        /// <param name="duration">sound duration (if you do not indicate the duration, the task is over as soon as it begins to play the sound)</param>
        /// <param name="loop">true if you want to play the sound in loop</param>
        public async Task PlaySoundAsync(string filepath, int volume = 100, string rootPrjsPath= "/home/root/lms2012/prjs/", int? duration = null, bool loop=false)
        {
            Task t = Brick.DirectCommand.PlaySoundAsync(volume, $"{rootPrjsPath}{filepath}", loop);
            if (duration.HasValue)
            {
                await Task.WhenAll(Task.Delay(duration.Value), t);              
            }
            else
            {
                await t;
            }
        }

        /// <summary>
        /// stops a playing sound
        /// </summary>
        public async Task StopSoundAsync()
        {
            await Brick.DirectCommand.StopSoundAsync();
        }

        /// <summary>
        /// the task to await
        /// </summary>
        new public Task Task
        {
            get
            {
                //return Task.WhenAny(base.tcs.Task, Task.Delay(5000));
                return base.tcs.Task;
            }
        }

        Queue<int> lastValues = new Queue<int>();

        /// <summary>
        /// Test pour la connaitre la fin de l'instruction envoyée a la brique
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Port_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            int lastValue = MotorData.Brick.Ports[MotorData.Port].RawValue;
            lastValues.Enqueue(lastValue);
            if (this.MotorData.EndValue - lastValue < this.MotorData.Error
                && this.MotorData.EndValue - lastValue > -this.MotorData.Error)
                this.SetResult(true);
            if (lastValues.Count == 20 && lastValues.Dequeue() == lastValue)
            {
                this.SetResult(true);
            }
        }

        /// <summary>
        /// Execute l'action demandée sur le moteur
        /// </summary>
        public override async Task Execute()
        {
            MotorData.InitTask();
            await MotorData.Brick.DirectCommand.StepMotorAtPowerAsync(Ports[MotorData.Port], MotorData.Power, (uint)MotorData.Displacement, MotorData.Brake);
            await Task;
        }
    }
}
