using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Media;
using System.Threading;
using System.Windows;
using RobloxPlayerNotifierApp.Models;
using RobloxPlayerNotifierApp.Services;
using RobloxPlayerNotifierApp.ViewModels;

namespace RobloxPlayerNotifierApp.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        private readonly SoundPlayer    _alertSoundPlayer = new SoundPlayer("Sounds\\Air Horn-SoundBible.com-964603082.wav");
        private MainViewModel           _viewModel;

        public MainView()
        {
            InitializeComponent();

            this.DataContext = _viewModel = new MainViewModel();

            _viewModel.PlayerStatusChanged += PlayerStatusChanged;

        }

        private void PlayerStatusChanged(PlayerStatusModel playerStatusModel)
        {
            Console.WriteLine($"Status changed for '{playerStatusModel.Name}', to '{playerStatusModel.Status}' - let's send a notification?");

            if (playerStatusModel.Status == PlayerStatus.Playing)
                _alertSoundPlayer.Play();
        }




        private void MainView_OnLoaded(object sender, RoutedEventArgs e)
        {
            _viewModel.Start();
        }
    }
}
