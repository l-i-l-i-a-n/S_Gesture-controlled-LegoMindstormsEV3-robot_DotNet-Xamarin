using System;
using Android.Content;
using AsyncEV3Lib.Android;
using Lego.EV3.UILib;
using Lego.EV3.UILib.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(EV3RobotChooser), typeof(EV3RobotChooserRenderer))]
namespace Lego.EV3.UILib.Droid
{
    public class EV3RobotChooserRenderer : ViewRenderer<EV3RobotChooser, Android.Views.View>
    {
        public EV3RobotChooserRenderer(Context context) : base(context)
        { }

        protected override void OnElementChanged(ElementChangedEventArgs<EV3RobotChooser> e)
        {
            //if (Control == null)
            //    SetNativeControl(new UIView(new CGRect(0, 0, 300, 256)));

            base.OnElementChanged(e);

            Element.ConnectionManager = new EV3ConnectionManager();
        }

    }
}
