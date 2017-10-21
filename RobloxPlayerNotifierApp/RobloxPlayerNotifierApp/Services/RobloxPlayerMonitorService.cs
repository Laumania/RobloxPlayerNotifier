using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RobloxPlayerNotifierApp.Models;

namespace RobloxPlayerNotifierApp.Services
{
    public class RobloxPlayerMonitorService
    {
        public Action<PlayerStatusModel> PlayerStatusChanged;

        private readonly TimeSpan                               _statusCheckInterval;
        private readonly RobloxPlayerStatusService              _statusService          = new RobloxPlayerStatusService();
        private readonly IDictionary<string, PlayerStatusModel> _previousPlayerStatuses = new Dictionary<string, PlayerStatusModel>();

        public RobloxPlayerMonitorService()
        {
            var intervalAsString = ConfigurationManager.AppSettings["PlayerStatusCheckIntervalInSeconds"];
            var intervalAsDouble = double.Parse(intervalAsString);
            _statusCheckInterval = TimeSpan.FromSeconds(intervalAsDouble);
        }

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

                    if(currentPlayerStatus == null)
                        continue;

                    PlayerStatusModel previousPlayerStatus;

                    if(_previousPlayerStatuses.TryGetValue(playerName, out previousPlayerStatus))
                    {
                        if(previousPlayerStatus.Status != currentPlayerStatus.Status)
                            OnPlayerStatusChanged(currentPlayerStatus);
                    }
                    else
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
