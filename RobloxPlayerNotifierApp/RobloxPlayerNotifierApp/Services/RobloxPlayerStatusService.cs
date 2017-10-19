using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using RobloxPlayerNotifierApp.Models;

namespace RobloxPlayerNotifierApp.Services
{
    public class RobloxPlayerStatusService
    {
        public async Task<PlayerStatusModel> GetPlayerStatus(string robloxPlayerName)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetStringAsync($"http://www.roblox.com/User.aspx?UserName={robloxPlayerName}");

                var status = PlayerStatus.Offline;
                if (response.Contains("icon-game"))
                    status = PlayerStatus.Playing;
                else if(response.Contains("icon-online"))
                    status = PlayerStatus.Online;

                return new PlayerStatusModel(robloxPlayerName, status);
            }
        }
    }
}
