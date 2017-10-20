using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NLog;
using RobloxPlayerNotifierApp.Models;

namespace RobloxPlayerNotifierApp.Services
{
    public class RobloxPlayerStatusService
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public async Task<PlayerStatusModel> GetPlayerStatus(string robloxPlayerName)
        {
            using (var client = new HttpClient())
            {
                var requestUrl = $"http://www.roblox.com/User.aspx?UserName={robloxPlayerName}";

                _logger.Trace($"Requesting '{requestUrl}'");
                var response = await client.GetStringAsync(requestUrl);

                var status = PlayerStatus.Offline;
                if (response.Contains("icon-game"))
                    status = PlayerStatus.Playing;
                else if(response.Contains("icon-online"))
                    status = PlayerStatus.Online;

                _logger.Debug($"PlayerStatus for '{robloxPlayerName}' is '{status}'");

                return new PlayerStatusModel(robloxPlayerName, status, requestUrl);
            }
        }
    }
}
