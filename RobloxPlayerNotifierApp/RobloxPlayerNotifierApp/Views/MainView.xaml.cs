using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Media;
using System.Threading;
using System.Windows;
using NLog;
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
        private readonly Logger         _logger             = LogManager.GetCurrentClassLogger();
        private readonly SoundPlayer    _alertSoundPlayer   = new SoundPlayer("Sounds\\EasyPeasyLemonSqueezy.wav");
        private readonly MainViewModel  _viewModel;

        public MainView()
        {
            InitializeComponent();

            this.DataContext = _viewModel = new MainViewModel();

            _viewModel.PlayerStatusChanged += PlayerStatusChanged;
        }

        private void PlayerStatusChanged(PlayerStatusModel playerStatusModel)
        {
            _logger.Info($"Status changed for '{playerStatusModel.Name}', to '{playerStatusModel.Status}'");

            if (playerStatusModel.Status == PlayerStatus.Playing)
                _alertSoundPlayer.Play();
        }




        private void MainView_OnLoaded(object sender, RoutedEventArgs e)
        {
            _viewModel.Start();
        }
    }
}
