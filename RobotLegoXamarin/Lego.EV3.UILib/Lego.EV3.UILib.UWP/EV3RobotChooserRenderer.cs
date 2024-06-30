using AsyncEV3Lib.UWP;
using Lego.EV3.UILib;
using Lego.EV3.UILib.UWP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(EV3RobotChooser), typeof(EV3RobotChooserRenderer))]
namespace Lego.EV3.UILib.UWP
{
    public class EV3RobotChooserRenderer : ViewRenderer<EV3RobotChooser, UserControl>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<EV3RobotChooser> e)
        {
            base.OnElementChanged(e);
            Element.ConnectionManager = new EV3ConnectionManager();
            //Element.Devices = Element.ConnectionManager.Devices;
        }
    }
}
