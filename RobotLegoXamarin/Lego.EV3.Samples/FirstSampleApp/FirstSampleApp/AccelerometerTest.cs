using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.Essentials;

namespace FirstSampleApp
{
    public class AccelerometerTest : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        public float X
        {
            get { return x; }
            set { x = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("X"));
                
            }
        }
        private float x = 0;

        public float Y
        {
            get { return y; }
            set { y = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Y")); }
        }
        private float y = 0;

        public float Z
        {
            get { return z; }
            set { z = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Z")); }
        }
        private float z = 0;

        // Set speed delay for monitoring changes.
        SensorSpeed speed = SensorSpeed.UI;
        MainPage m;
        public AccelerometerTest(MainPage m)
        {
            // Register for reading changes.
            Gyroscope.ReadingChanged += Gyroscope_ReadingChanged;
            this.m = m;
        }

        void Gyroscope_ReadingChanged(object sender, GyroscopeChangedEventArgs e)
        {
            var data = e.Reading;
            // Process Angular Velocity X, Y, and Z reported in rad/s
            if(m != null)
            m.updateValues(data.AngularVelocity.X, data.AngularVelocity.Y, data.AngularVelocity.Z);
        }

        public void ToggleGyroscope()
        {
            try
            {
                if (Gyroscope.IsMonitoring)
                    Gyroscope.Stop();
                else
                    Gyroscope.Start(speed);
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Feature not supported on device
            }
            catch (Exception ex)
            {
                // Other error has occurred.
            }
        }
    }
}
