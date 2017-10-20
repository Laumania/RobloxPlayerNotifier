using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
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
        public MainViewModel()
        {
            _monitorService                     = new RobloxPlayerMonitorService();
            _monitorService.PlayerStatusChanged += MonitorServicePlayerStatusChanged;

            _viewProfileCommand = new RelayCommand(x =>
            {
                var playerStatus = x as PlayerStatusViewModel;
                System.Diagnostics.Process.Start(playerStatus.PlayerProfileUrl);
            });

            InitializePlayerStatusList();
        }

        private void InitializePlayerStatusList()
        {
            PlayerStatusList.Add(new PlayerStatusViewModel("ComKeanOfficial"));
            PlayerStatusList.Add(new PlayerStatusViewModel("Emil_trier"));
            PlayerStatusList.Add(new PlayerStatusViewModel("Mikkel_trier"));
            PlayerStatusList.Add(new PlayerStatusViewModel("MiltonSej"));
            PlayerStatusList.Add(new PlayerStatusViewModel("MarvinSej"));
        }

        public void Start()
        {
            _monitorService.Start(PlayerStatusList.Select(x => x.Name));
        }


        private void MonitorServicePlayerStatusChanged(PlayerStatusModel playerStatusModel)
        {
            if (PlayerStatusChanged != null)
                PlayerStatusChanged(playerStatusModel);

            var updatedPlayer               = PlayerStatusList.First(x => x.Name == playerStatusModel.Name);
            updatedPlayer.Status            = playerStatusModel.Status;
            updatedPlayer.PlayerProfileUrl  = playerStatusModel.PlayerProfileUrl;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<PlayerStatusViewModel> PlayerStatusList { get; } = new ObservableCollection<PlayerStatusViewModel>();

        public ICommand ViewProfileCommand
        {
            get { return _viewProfileCommand; }
        }
    }
}
