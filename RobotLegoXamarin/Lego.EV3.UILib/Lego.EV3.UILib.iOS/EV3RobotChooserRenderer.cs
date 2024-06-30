using System;
using CoreGraphics;
using Lego.EV3.UILib;
using Lego.EV3.UILib.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using AsyncEV3Lib.iOS;

[assembly: ExportRenderer(typeof(EV3RobotChooser), typeof(EV3RobotChooserRenderer))]
namespace Lego.EV3.UILib.iOS
{
    public class EV3RobotChooserRenderer : ViewRenderer<EV3RobotChooser, UIView>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<EV3RobotChooser> e)
        {
            //if (Control == null)
            //    SetNativeControl(new UIView(new CGRect(0, 0, 300, 256)));
            
            base.OnElementChanged(e);

            Element.ConnectionManager = new EV3ConnectionManager();
        }
         
    }
}
