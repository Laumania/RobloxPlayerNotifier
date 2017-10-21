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

            PreviouseWindowState = WindowState;
            LayoutUpdated += Window_LayoutUpdated;
        }

        private void MainView_OnLoaded(object sender, RoutedEventArgs e)
        {
            _viewModel.Start();
        }

        private void PlayerStatusChanged(PlayerStatusModel playerStatusModel)
        {
            

            if (playerStatusModel.Status == PlayerStatus.Playing)
            {
                _logger.Info($"'{playerStatusModel.Name}' is now playing - '{playerStatusModel.PlayerProfileUrl}'");
                _alertSoundPlayer.Play();
                this.Activate(true);
            }
        }



        private void Window_LayoutUpdated(object sender, EventArgs e)
        {
            PreviouseWindowState = WindowState;
        }

        public bool Activate(bool restoreIfMinimized)
        {
            if (restoreIfMinimized && WindowState == WindowState.Minimized)
            {
                WindowState = PreviouseWindowState == WindowState.Normal
                    ? WindowState.Normal : WindowState.Maximized;
            }
            return Activate();
        }

        private WindowState PreviouseWindowState { get; set; }
    }
}
