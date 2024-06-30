using Lego.Ev3.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsyncEV3MotorCommandsLib
{
    /// <summary>
    /// adds commands in batch and sends them all together
    /// </summary>
    public class BatchMotorCommandAsync : ManualTask
    {
        /// <summary>
        /// used for stepping motors
        /// </summary>
        Dictionary<MotorCommandData, bool> MotorData { get; } = new Dictionary<MotorCommandData, bool>();

        /// <summary>
        /// the EV3 brick
        /// </summary>
        Brick Brick { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="brick">the EV3 brick to work with</param>
        public BatchMotorCommandAsync(Brick brick)
        {
            Brick = brick;
        }

        /// <summary>
        /// updated every time a task with a specified time is added 
        /// </summary>
        /// <remarks>not updated if the longest task is removed</remarks>
        int MaxMilliseconds { get; set; } = 0;

        /// <summary>
        /// adds a command whose purpose is to turn a motor with a specified number of steps
        /// </summary>
        /// <param name="port">input port of the motor</param>
        /// <param name="displacement">number of steps</param>
        /// <param name="error">accepted error</param>
        /// <param name="power">power (%), can be negative</param>
        /// <param name="brake">true to use the brake at the end of the command</param>
        public void AddStepMotorAtPowerAsync(InputPort port, int displacement, int error = 12, int power = 70, bool brake = true)
        {
            var md = new MotorCommandData(Brick, port, displacement, error, power, brake);
            lock (MotorData)
            {
                MotorData.Add(md, false);
            }
            md.InitTask();
            md.PropertyChanged += Md_PropertyChanged;
            md.Brick.BatchCommand.StepMotorAtPower(Ports[md.Port], md.Power, (uint)md.Displacement, md.Brake);
        }

        /// <summary>
        /// adds a command whose purpose is to turn the motor during a certain amount of milliseconds
        /// </summary>
        /// <param name="port">input port of the motor</param>
        /// <param name="milliseconds">time in milliseconds</param>
        /// <param name="power">power (%), can be negative</param>
        /// <param name="brake">true to use the brake at the end of the command</param>
        public void TurnMotorAtPowerForTimeAsync(InputPort port, int milliseconds, int power = 70, bool brake = true)
        {
            Brick.BatchCommand.TurnMotorAtPowerForTime(Ports[port], power, (uint)milliseconds, brake);
            MaxMilliseconds = Math.Max(milliseconds, MaxMilliseconds);
        }

        /// <summary>
        /// adds a command whose purpose is to start the motor
        /// </summary>
        /// <param name="port">input port of the motor</param>
        /// <param name="power">power (%), can be negative</param>
        public void StartMotorAtPowerAsync(InputPort port, int power = 70)
        {
            Brick.BatchCommand.StartMotorAtPower(Ports[port], power);
        }

        /// <summary>
        /// adds a command whose purpose is to stop the motor
        /// </summary>
        /// <param name="port">input port of the motor</param>
        /// <param name="brake">true to use the brake at the end of the command</param>
        public void StopMotorAsync(InputPort port, bool brake = true)
        {
            Brick.BatchCommand.StopMotor(Ports[port], brake);
        }

        /// <summary>
        /// adds a command whose purpose is to play a tone
        /// </summary>
        /// <param name="frequency">tone frequency</param>
        /// <param name="duration">tone duration</param>
        /// <param name="volume">tone volume on the brick (independent of the volume indicated on the brick)</param>
        public void PlayTone(ushort frequency = 200, ushort duration = 1000, int volume = 100)
        {
            Brick.BatchCommand.PlayTone(volume, frequency, duration);
            MaxMilliseconds = Math.Max(duration, MaxMilliseconds);
        }

        /// <summary>
        /// adds a command whose purpose is to play a sound
        /// </summary>
        /// <param name="filepath">path to the file of the sound file (from the prjs folder)</param>
        /// <param name="volume">sound volume on the brick (independent of the volume indicated on the brick)</param>
        /// <param name="rootPrjsPath">root folder where the sounds are stored</param>
        /// <param name="duration">sound duration (if you do not indicate the duration, the task is over as soon as it begins to play the sound)</param>
        /// <param name="loop">true if you want to play the sound in loop</param>
        public void PlaySound(string filepath, int volume = 100, string rootPrjsPath = "/home/root/lms2012/prjs/", int? duration = null, bool loop = false)
        {
            Brick.BatchCommand.PlaySound(volume, $"{rootPrjsPath}{filepath}", loop);
            if (duration.HasValue)
            {
                MaxMilliseconds = Math.Max(duration.Value, MaxMilliseconds);
            }
        }

        /// <summary>
        /// adds a command whose purpose is to stop a playing sound
        /// </summary>
        public void StopSound()
        {
            Brick.BatchCommand.StopSound();
        }

        /// <summary>
        /// the task gathering all the commands
        /// </summary>
        new public Task Task
        {
            get
            {
                return Task.WhenAll(base.tcs.Task, Task.Delay(MaxMilliseconds));
            }
        }

        /// <summary>
        /// executes the task
        /// </summary>
        /// <returns></returns>
        public override async Task Execute()
        {
            ResetTask();
            await Brick.BatchCommand.SendCommandAsync();
            if (MotorData.Count == 0)
            {
                SetResult(true);
            }
            else
            {
                CheckIfMotorsAreBlocked();
            }
            await Task;

            lock (MotorData)
            {
                foreach (var md in MotorData.Keys)
                {
                    md.PropertyChanged -= Md_PropertyChanged;
                }
                MotorData.Clear();
            }
        }

        async void CheckIfMotorsAreBlocked()
        {
            lock (MotorData)
            {
                foreach (var md in MotorData.Keys)
                {
                    motorRawValues[md] = Brick.Ports[md.Port].RawValue;
                }
            }

            while (MotorData.Count > 0)
            {
                try
                {
                    await Task.Delay(1000);
                    bool allBlocked = true;
                    lock (MotorData)
                    {
                        foreach (var md in MotorData.Keys)
                        {
                            if (motorRawValues[md] != Brick.Ports[md.Port].RawValue)
                            {
                                allBlocked = false;
                            }
                            motorRawValues[md] = Brick.Ports[md.Port].RawValue;
                        }
                    }
                    if (allBlocked)
                    {
                        SetResult(true);
                    }
                }
                catch (KeyNotFoundException exc)
                {
                    SetResult(true);
                }
            }
        }

        Dictionary<MotorCommandData, int> motorRawValues = new Dictionary<MotorCommandData, int>();

        /// <summary>
        /// checks if the motors have reached the seeked values
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Md_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Port port = sender as Port;

            if (MotorData.Count == 0) return;

            lock (MotorData)
            {
                MotorCommandData motorData = MotorData.Keys.SingleOrDefault(md => md.Port.ToString() == port.Name);

                if (motorData == null) return;

                if (motorData.EndValue - motorData.Brick.Ports[motorData.Port].RawValue < motorData.Error
                    && motorData.EndValue - motorData.Brick.Ports[motorData.Port].RawValue > -motorData.Error)
                {
                    MotorData[motorData] = true;
                }

                if (MotorData.Values.Count(v => !v) == 0)
                {
                    SetResult(true);
                }
            }
        }
    }
}
