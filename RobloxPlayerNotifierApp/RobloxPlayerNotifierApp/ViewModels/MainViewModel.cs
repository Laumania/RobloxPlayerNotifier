using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using RobloxPlayerNotifierApp.Models;
using RobloxPlayerNotifierApp.Services;

namespace RobloxPlayerNotifierApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public Action<PlayerStatusModel> PlayerStatusChanged;

        private readonly RobloxPlayerMonitorService _monitorService;
        private readonly ICommand _viewProfileCommand;
        private readonly ICommand _addPlayerCommand;
        private readonly ICommand _removePlayerCommand;
        private readonly ICommand _refreshStatusCommand;

        private string _newPlayerName = string.Empty;
        private int _checkIntervalSeconds = 15;

        public MainViewModel()
        {
            _monitorService = new RobloxPlayerMonitorService();
            _monitorService.PlayerStatusChanged += MonitorServicePlayerStatusChanged;

            _viewProfileCommand = new RelayCommand(x =>
            {
                if (x is PlayerStatusViewModel playerStatus && !string.IsNullOrEmpty(playerStatus.PlayerProfileUrl))
                {
                    try
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = playerStatus.PlayerProfileUrl,
                            UseShellExecute = true
                        });
                    }
                    catch { }
                }
            });

            _addPlayerCommand = new RelayCommand(x => AddPlayer());
            _removePlayerCommand = new RelayCommand(x => RemovePlayer(x as PlayerStatusViewModel));
            _refreshStatusCommand = new RelayCommand(x => RefreshAllStatus());

            LoadSettingsAndPlayers();
        }

        public string NewPlayerName
        {
            get => _newPlayerName;
            set
            {
                _newPlayerName = value;
                OnPropertyChanged();
            }
        }

        public int CheckIntervalSeconds
        {
            get => _checkIntervalSeconds;
            set
            {
                if (value >= 5 && value <= 3600)
                {
                    _checkIntervalSeconds = value;
                    OnPropertyChanged();
                    _monitorService.SetCheckInterval(value);
                    SaveSettingsToConfig();
                }
            }
        }

        private void LoadSettingsAndPlayers()
        {
            var intervalStr = ConfigurationManager.AppSettings["PlayerStatusCheckIntervalInSeconds"];
            if (int.TryParse(intervalStr, out int interval) && interval >= 5)
            {
                _checkIntervalSeconds = interval;
            }

            var rawConfig = ConfigurationManager.AppSettings["PlayerNamesToMonitor"];
            if (!string.IsNullOrWhiteSpace(rawConfig))
            {
                var playerNamesToMonitor = rawConfig.Split(';', StringSplitOptions.RemoveEmptyEntries);
                foreach (var playerName in playerNamesToMonitor)
                {
                    var trimmed = playerName.Trim();
                    if (!string.IsNullOrEmpty(trimmed) && !PlayerStatusList.Any(p => string.Equals(p.Name, trimmed, StringComparison.OrdinalIgnoreCase)))
                    {
                        PlayerStatusList.Add(new PlayerStatusViewModel(trimmed));
                    }
                }
            }
        }

        public void AddPlayer()
        {
            if (string.IsNullOrWhiteSpace(NewPlayerName))
                return;

            var trimmedName = NewPlayerName.Trim();
            if (PlayerStatusList.Any(p => string.Equals(p.Name, trimmedName, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show($"Player '{trimmedName}' is already in the list.", "Notice", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var newViewModel = new PlayerStatusViewModel(trimmedName);
            PlayerStatusList.Add(newViewModel);
            _monitorService.AddPlayer(trimmedName);
            NewPlayerName = string.Empty;

            SaveSettingsToConfig();
        }

        public void RemovePlayer(PlayerStatusViewModel player)
        {
            if (player == null) return;

            PlayerStatusList.Remove(player);
            _monitorService.RemovePlayer(player.Name);

            SaveSettingsToConfig();
        }

        public void RefreshAllStatus()
        {
            _monitorService.TriggerImmediateCheck();
        }

        public void Start()
        {
            _monitorService.Start(PlayerStatusList.Select(x => x.Name));
        }

        private void SaveSettingsToConfig()
        {
            try
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var namesString = string.Join(";", PlayerStatusList.Select(p => p.Name));

                if (config.AppSettings.Settings["PlayerNamesToMonitor"] == null)
                    config.AppSettings.Settings.Add("PlayerNamesToMonitor", namesString);
                else
                    config.AppSettings.Settings["PlayerNamesToMonitor"].Value = namesString;

                if (config.AppSettings.Settings["PlayerStatusCheckIntervalInSeconds"] == null)
                    config.AppSettings.Settings.Add("PlayerStatusCheckIntervalInSeconds", CheckIntervalSeconds.ToString());
                else
                    config.AppSettings.Settings["PlayerStatusCheckIntervalInSeconds"].Value = CheckIntervalSeconds.ToString();

                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }
            catch { }
        }

        private void MonitorServicePlayerStatusChanged(PlayerStatusModel playerStatusModel)
        {
            if (Application.Current != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    UpdatePlayerStatus(playerStatusModel);
                });
            }
            else
            {
                UpdatePlayerStatus(playerStatusModel);
            }
        }

        private void UpdatePlayerStatus(PlayerStatusModel playerStatusModel)
        {
            PlayerStatusChanged?.Invoke(playerStatusModel);

            var updatedPlayer = PlayerStatusList.FirstOrDefault(x => string.Equals(x.Name, playerStatusModel.Name, StringComparison.OrdinalIgnoreCase));
            if (updatedPlayer != null)
            {
                updatedPlayer.Status = playerStatusModel.Status;
                updatedPlayer.PlayerProfileUrl = playerStatusModel.PlayerProfileUrl;
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<PlayerStatusViewModel> PlayerStatusList { get; } = new ObservableCollection<PlayerStatusViewModel>();

        public ICommand ViewProfileCommand => _viewProfileCommand;
        public ICommand AddPlayerCommand => _addPlayerCommand;
        public ICommand RemovePlayerCommand => _removePlayerCommand;
        public ICommand RefreshStatusCommand => _refreshStatusCommand;
    }
}
