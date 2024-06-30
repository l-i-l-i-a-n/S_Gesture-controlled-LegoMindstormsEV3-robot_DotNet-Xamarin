using Lego.Ev3.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FirstSampleApp
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class InputPortUC : ContentView
	{
		public InputPortUC ()
		{
			InitializeComponent ();
		}

        public static readonly BindableProperty TypeProperty =
            BindableProperty.Create("Type", typeof(DeviceType), typeof(InputPortUC), DeviceType.Unknown, BindingMode.TwoWay, propertyChanged: (sender, oldValue, newValue) =>
            {
                
            });

        public DeviceType Type
        {
            get { return (DeviceType)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        public static readonly BindableProperty SIValueProperty =
            BindableProperty.Create("SIValue", typeof(float), typeof(InputPortUC), 0.0f, BindingMode.TwoWay, propertyChanged: (sender, oldValue, newValue) =>
            {

            });

        public float SIValue
        {
            get { return (float)GetValue(SIValueProperty); }
            set { SetValue(SIValueProperty, value); }
        }
    }
}