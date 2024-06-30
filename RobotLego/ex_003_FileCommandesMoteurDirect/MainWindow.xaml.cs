using AsyncEV3MotorCommandsLib;
using Lego.Ev3.Core;
using Lego.Ev3.Desktop;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace ex_003_FileCommandesMoteurDirect
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BrickManager brickManager = new BrickManager();
        public ObservableCollection<string> Commandes { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            Commandes = new ObservableCollection<string>();
            DataContext = brickManager;
            listBoxCommandes.DataContext = this;
        }

        private static Dictionary<char, OutputPort> ports = new Dictionary<char, OutputPort>()
        {
            {'A', OutputPort.A},
            {'B', OutputPort.B},
            {'C', OutputPort.C},
            {'D', OutputPort.D}
        };

        private static Dictionary<char, InputPort> inputPorts = new Dictionary<char, InputPort>()
        {
            {'A', InputPort.A},
            {'B', InputPort.B},
            {'C', InputPort.C},
            {'D', InputPort.D},
        };

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button) || !(sender as Button).IsEnabled) return;

            char port = ((sender as Button).Content as string).Last();

            if (!ports.ContainsKey(port)) return;

            Commandes.Add((sender as Button).Content as string);
        }

        private async void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            await brickManager.ConnectAsync();
        }

        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            brickManager.Disconnect();
        }

        Brick brick { get { return brickManager.Brick; } }

        private async void Methode1(object sender, RoutedEventArgs e)
        {
            foreach (var commande in Commandes)
            {
                await brick.DirectCommand.TurnMotorAtPowerForTimeAsync(ports[commande.Last()], 100, 2000, true);
                await brick.DirectCommand.PlayToneAsync(100, 2000, 500);
            }
            Commandes.Clear();
        }

        private async void Methode2(object sender, RoutedEventArgs e)
        {
            foreach (var commande in Commandes)
            {
                await brick.DirectCommand.TurnMotorAtPowerForTimeAsync(ports[commande.Last()], 100, 2000, true);
                await Task.Delay(2000);
                await brick.DirectCommand.PlayToneAsync(100, 2000, 500);
                await Task.Delay(500);
            }
            Commandes.Clear();
        }

        private async void Methode3(object sender, RoutedEventArgs e)
        {
            foreach (var commande in Commandes)
            {
                await Task.WhenAll(
                        brick.DirectCommand.TurnMotorAtPowerForTimeAsync(ports[commande.Last()], 100, 1500, true),
                        Task.Delay(2000)).ContinueWith((t) => brick.DirectCommand.PlayToneAsync(100, 2000, 500));
            }
            Commandes.Clear();
        }

        private async void Methode4(object sender, RoutedEventArgs e)
        {
            foreach (var com in Commandes)
            {
                await brickManager.DirectCommand.StepMotorAtPowerAsync(inputPorts[com.Last()], 500);
            }
            Commandes.Clear();
        }

        private void Methode5(object sender, RoutedEventArgs e)
        {
            foreach (var com in Commandes)
            {
                brickManager.DirectCommand.StepMotorAtPowerAsync(inputPorts[com.Last()], 500);
            }
            Commandes.Clear();
        }

        private async void Methode6(object sender, RoutedEventArgs e)
        {
            //ManualTaskCollection listTasks = new ManualTaskCollection();
            //listTasks.AddManualTasks(Commandes.Select(com => brickManager.DirectCommand.StepMotorAtPowerAsync(inputPorts[com.Last()], 500)).ToArray());
            //await listTasks.Execute().ContinueWith((t) => brick.DirectCommand.PlayToneAsync(10, 2000, 500))
            //                         .ContinueWith((t) => new DirectMotorCommandAsync(brick, InputPort.A, 500).Execute());
            //Commandes.Clear();
        }

        private async void Methode7(object sender, RoutedEventArgs e)
        {
            foreach (var com in Commandes)
                brickManager.BatchCommand.AddStepMotorAtPowerAsync(inputPorts[com.Last()], 500, error: 15, power: 100);
            await brickManager.BatchCommand.Execute().ContinueWith((t) => brick.DirectCommand.PlayToneAsync(100, 2000, 500));
            Commandes.Clear();
        }
        private async void Methode8(object sender, RoutedEventArgs e)
        {

            //brickManager.BatchCommand.AddStepMotorAtPowerAsync(InputPort.B, 2500, error: 15, power: 100);
            //brickManager.BatchCommand.AddStepMotorAtPowerAsync(InputPort.C, 2500, error: 15, power: 100);
            //BatchMotorCommandAsync test2 = new BatchMotorCommandAsync();
            //    test2.AddMotorAsyncCommand(brick, InputPort.B, 500, error: 15, power: 100);
            //    test2.AddMotorAsyncCommand(brick, InputPort.C, 2500, error: 15, power: 100);
            //var test3 = new DirectMotorCommandAsync(brick, InputPort.A, 500);
            //await test.Execute().ContinueWith((t) => new DirectMotorCommandAsync(brick, InputPort.A, 500).Execute()
            //                        .ContinueWith((t2) => new DirectMotorCommandAsync(brick, InputPort.B, 500, error: 15, power: 100).Execute()
            //                           .ContinueWith((t3) => test2.Execute()
            //                              .ContinueWith((t4) => brick.DirectCommand.PlayToneAsync(100, 2000, 500)))));
            //Commandes.Clear();
        }
    }
}
