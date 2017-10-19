using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RobloxPlayerNotifierApp.Models;

namespace RobloxPlayerNotifierApp.Services
{
    public class RobloxPlayerMonitorService
    {
        public Action<PlayerStatusModel> PlayerStatusChanged;

        private RobloxPlayerStatusService _statusService = new RobloxPlayerStatusService();


        public void Start(IEnumerable<string> playerNamesToMonitor)
        {
            if (IsRunning)
                return;

            IsRunning = true;
            ActualMonitoring(playerNamesToMonitor);
        }

        public void Stop()
        {
            IsRunning = false;
        }



        private async Task ActualMonitoring(IEnumerable<string> playerNamesToMonitor)
        {
            while (IsRunning)
            {
                foreach (var playerName in playerNamesToMonitor)
                {
                    var playerStatus = await _statusService.GetPlayerStatus(playerName);

                    if (PlayerStatusChanged != null)
                        PlayerStatusChanged(playerStatus);
                }

                await Task.Delay(5000);
            }
        }






        public bool IsRunning { get; private set; }
    }
}
