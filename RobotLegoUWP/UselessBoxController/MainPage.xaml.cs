using AsyncEV3Lib;
using Lego.Ev3.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UselessBoxController
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        BrickManager brickManager = new BrickManager();

        public BoxVM Box { get; private set; } = new BoxVM();
        public MainPage()
        {
            this.InitializeComponent();
            DataContext = brickManager;
        }

        private async void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            //await ConnectToBox();
            DeviceSelected += MainPage_DeviceSelected;
            //await brickManager.ConnectAsync("Bluetooth#Bluetooth6c:40:08:95:63:ed-00:16:53:41:e1:2a");
        }

        private async void MainPage_DeviceSelected(object sender, EventArgs e)
        {
            await ConnectToBox(SelectedDevice?.Id);
            //await ConnectToBox(SelectedDevice?.Name);
        }

        private async System.Threading.Tasks.Task ConnectToBox(string name)
        {
            await brickManager.ConnectAsync(name);

            if (brickManager.Connected)
            {
                await brickManager.PlayThirdKindAsync(volume: 1, duration: 250);
            }

            Box.BrickManager = brickManager;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            Disconnect();
        }

        internal void Disconnect()
        {
            if (brickManager.Connected)
                brickManager.Disconnect();
        }

        private DeviceInformation selectedDevice;


        public DeviceInformation SelectedDevice
        {
            get { return selectedDevice; }
            set
            {
                selectedDevice = value;
                if (value != null)
                {
                    if (brickManager.Connected)
                        brickManager.Disconnect();
                    DeviceSelected?.Invoke(this, null);
                }
            }
        }

        event EventHandler DeviceSelected;

        private async void MoveArm_Clicked(object sender, RoutedEventArgs e)
        {
            await Box.MoveArm_ToTurnOff();
        }

        private async void MoveSedondaryArm_Clicked(object sender, RoutedEventArgs e)
        {
            await Box.MoveSecondaryArm_ToTurnOn();
        }

        private async void MoveForward_Clicked(object sender, RoutedEventArgs e)
        {
            await Box.MoveForward();
        }

        private async void MoveBackward_Clicked(object sender, RoutedEventArgs e)
        {
            await Box.MoveBackward();
        }

        private async void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if ((sender as ToggleSwitch).IsOn)
            {
                await Box.TurnOnTheLight();
            }
            else
            {
                await Box.TurnOffTheLight();
            }
        }

        private async void LittleDance_Clicked(object sender, RoutedEventArgs e)
        {
            await Box.LittleDance();
        }

        private async void LittleVibration_Clicked(object sender, RoutedEventArgs e)
        {
            await Box.LittleVibration();
        }

        private async void LittleSad_Clicked(object sender, RoutedEventArgs e)
        {
            await Box.LittleSad();
        }
    }
}
