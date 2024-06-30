using Lego.Ev3.Core;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AsyncEV3MotorCommandsLib;

namespace ex_001_CommandeMoteurSystem
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

        internal static Dictionary<char, OutputPort> ports = new Dictionary<char, OutputPort>()
        {
            {'A', OutputPort.A},
            {'B', OutputPort.B},
            {'C', OutputPort.C},
            {'D', OutputPort.D}
        };

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button)) return;

            char port = ((sender as Button).Content as string).Last();

            if (!ports.ContainsKey(port)) return;

            await brickManager.Brick.DirectCommand.TurnMotorAtPowerForTimeAsync(ports[port], 100, 2000, true);
        }

        private async void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            await brickManager.ConnectAsync();
        }

        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            brickManager.Disconnect();
        }
    }
}
