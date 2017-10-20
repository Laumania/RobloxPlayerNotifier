using System;
using System.Collections.Concurrent;
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

        private readonly TimeSpan                               _statusCheckInterval    = TimeSpan.FromMilliseconds(5000);
        private readonly RobloxPlayerStatusService              _statusService          = new RobloxPlayerStatusService();
        private readonly IDictionary<string, PlayerStatusModel> _previousPlayerStatuses = new Dictionary<string, PlayerStatusModel>();

        public void Start(IEnumerable<string> playerNamesToMonitor)
        {
            if (IsRunning)
                return;

            IsRunning = true;
            RunActualMonitoringTask(playerNamesToMonitor);
        }

        public void Stop()
        {
            IsRunning = false;
        }



        private async Task RunActualMonitoringTask(IEnumerable<string> playerNamesToMonitor)
        {
            while (IsRunning)
            {
                foreach (var playerName in playerNamesToMonitor)
                {
                    var currentPlayerStatus = await _statusService.GetPlayerStatus(playerName);

                    PlayerStatusModel previousPlayerStatus;

                    if(_previousPlayerStatuses.TryGetValue(playerName, out previousPlayerStatus))
                    {
                        if(previousPlayerStatus.Status != PlayerStatus.Playing && currentPlayerStatus.Status == PlayerStatus.Playing)
                            OnPlayerStatusChanged(currentPlayerStatus);
                    }
                    else if (currentPlayerStatus.Status == PlayerStatus.Playing)
                    {
                        OnPlayerStatusChanged(currentPlayerStatus);
                    }

                    _previousPlayerStatuses[playerName] = currentPlayerStatus;
                }

                await Task.Delay(_statusCheckInterval);
            }
        }

        private void OnPlayerStatusChanged(PlayerStatusModel playerStatus)
        {
            if (PlayerStatusChanged != null)
                PlayerStatusChanged(playerStatus);
        }


        public bool IsRunning { get; private set; }
    }
}
