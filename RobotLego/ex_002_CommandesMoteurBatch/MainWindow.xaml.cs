using AsyncEV3MotorCommandsLib;
using Lego.Ev3.Core;
using Lego.Ev3.Desktop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ex_002_CommandesMoteurBatch
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BrickManager brickManager = new BrickManager();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = brickManager;
        }

        private static Dictionary<char, OutputPort> ports = new Dictionary<char, OutputPort>()
        {
            {'A', OutputPort.A},
            {'B', OutputPort.B},
            {'C', OutputPort.C},
            {'D', OutputPort.D}
        };

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button) || !(sender as Button).IsEnabled) return;

            char port = ((sender as Button).Content as string).Last();

            if (!ports.ContainsKey(port)) return;

            brickManager.Brick.BatchCommand.TurnMotorAtPowerForTime(ports[port], 100, 2000, true);
            (sender as Button).IsEnabled = false;
        }

        private async void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            await brickManager.ConnectAsync();
        }

        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            brickManager.Disconnect();
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            await brickManager.Brick.BatchCommand.SendCommandAsync();
        }
    }
}
