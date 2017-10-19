using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using RobloxPlayerNotifierApp.Models;
using RobloxPlayerNotifierApp.Services;

namespace RobloxPlayerNotifierApp.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        private readonly RobloxPlayerMonitorService _monitorService;

        public MainView()
        {
            InitializeComponent();

            _monitorService                     = new RobloxPlayerMonitorService();
            _monitorService.PlayerStatusChanged += PlayerStatusChanged;
        }

        private void PlayerStatusChanged(PlayerStatusModel playerStatusModel)
        {
            Console.WriteLine($"STATUS: Changed for '{playerStatusModel.Name}', to '{playerStatusModel.Status}' - let's send a notification?");
        }


        private void MainView_OnLoaded(object sender, RoutedEventArgs e)
        {
            _monitorService.Start();
        }
    }
}
