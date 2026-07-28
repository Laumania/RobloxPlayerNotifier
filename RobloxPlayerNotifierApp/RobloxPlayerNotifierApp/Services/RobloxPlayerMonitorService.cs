using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RobloxPlayerNotifierApp.Models;

namespace RobloxPlayerNotifierApp.Services
{
    public class RobloxPlayerMonitorService
    {
        public Action<PlayerStatusModel> PlayerStatusChanged;

        private TimeSpan _statusCheckInterval;
        private readonly RobloxPlayerStatusService _statusService = new RobloxPlayerStatusService();
        private readonly ConcurrentDictionary<string, PlayerStatusModel> _previousPlayerStatuses = new ConcurrentDictionary<string, PlayerStatusModel>(StringComparer.OrdinalIgnoreCase);
        private readonly ConcurrentDictionary<string, bool> _monitoredPlayers = new ConcurrentDictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
        private CancellationTokenSource _cts;

        public RobloxPlayerMonitorService()
        {
            var intervalAsString = ConfigurationManager.AppSettings["PlayerStatusCheckIntervalInSeconds"] ?? "15";
            if (double.TryParse(intervalAsString, out double intervalAsDouble))
            {
                _statusCheckInterval = TimeSpan.FromSeconds(Math.Max(5, intervalAsDouble));
            }
            else
            {
                _statusCheckInterval = TimeSpan.FromSeconds(15);
            }
        }

        public void SetCheckInterval(int seconds)
        {
            if (seconds >= 5)
            {
                _statusCheckInterval = TimeSpan.FromSeconds(seconds);
            }
        }

        public void Start(IEnumerable<string> playerNamesToMonitor)
        {
            if (IsRunning)
                return;

            _monitoredPlayers.Clear();
            foreach (var name in playerNamesToMonitor)
            {
                if (!string.IsNullOrWhiteSpace(name))
                {
                    _monitoredPlayers[name.Trim()] = true;
                }
            }

            IsRunning = true;
            _cts = new CancellationTokenSource();
            RunActualMonitoringTask(_cts.Token);
        }

        public void AddPlayer(string playerName)
        {
            if (!string.IsNullOrWhiteSpace(playerName))
            {
                var trimmed = playerName.Trim();
                _monitoredPlayers[trimmed] = true;
                Task.Run(() => CheckSinglePlayerAsync(trimmed));
            }
        }

        public void RemovePlayer(string playerName)
        {
            if (!string.IsNullOrWhiteSpace(playerName))
            {
                var trimmed = playerName.Trim();
                _monitoredPlayers.TryRemove(trimmed, out _);
                _previousPlayerStatuses.TryRemove(trimmed, out _);
            }
        }

        public void TriggerImmediateCheck()
        {
            Task.Run(async () =>
            {
                foreach (var playerName in _monitoredPlayers.Keys.ToList())
                {
                    await CheckSinglePlayerAsync(playerName);
                }
            });
        }

        public void Stop()
        {
            IsRunning = false;
            _cts?.Cancel();
        }

        private async Task RunActualMonitoringTask(CancellationToken token)
        {
            while (IsRunning && !token.IsCancellationRequested)
            {
                var currentPlayers = _monitoredPlayers.Keys.ToList();
                foreach (var playerName in currentPlayers)
                {
                    if (token.IsCancellationRequested) break;
                    await CheckSinglePlayerAsync(playerName);
                }

                try
                {
                    await Task.Delay(_statusCheckInterval, token);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }
        }

        private async Task CheckSinglePlayerAsync(string playerName)
        {
            var currentPlayerStatus = await _statusService.GetPlayerStatus(playerName);
            if (currentPlayerStatus == null)
                return;

            if (_previousPlayerStatuses.TryGetValue(playerName, out var previousPlayerStatus))
            {
                if (previousPlayerStatus.Status != currentPlayerStatus.Status)
                {
                    OnPlayerStatusChanged(currentPlayerStatus);
                }
            }
            else
            {
                OnPlayerStatusChanged(currentPlayerStatus);
            }

            _previousPlayerStatuses[playerName] = currentPlayerStatus;
        }

        private void OnPlayerStatusChanged(PlayerStatusModel playerStatus)
        {
            PlayerStatusChanged?.Invoke(playerStatus);
        }

        public bool IsRunning { get; private set; }
    }
}
