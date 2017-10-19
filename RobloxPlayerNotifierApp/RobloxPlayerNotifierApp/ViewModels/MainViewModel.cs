using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using RobloxPlayerNotifierApp.Models;
using RobloxPlayerNotifierApp.Services;

namespace RobloxPlayerNotifierApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public Action<PlayerStatusModel> PlayerStatusChanged;

        private readonly RobloxPlayerMonitorService _monitorService;

        public MainViewModel()
        {
            _monitorService                     = new RobloxPlayerMonitorService();
            _monitorService.PlayerStatusChanged += MonitorServicePlayerStatusChanged;
        }

        public void Start()
        {
            _monitorService.Start(new List<string>() { "ComKeanOfficial", "MiltonSej", "MarvinSej" });
        }


        private void MonitorServicePlayerStatusChanged(PlayerStatusModel playerStatusModel)
        {
            if (PlayerStatusChanged != null)
                PlayerStatusChanged(playerStatusModel);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
