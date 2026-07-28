using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using NLog;
using RobloxPlayerNotifierApp.Models;

namespace RobloxPlayerNotifierApp.Services
{
    public class RobloxPlayerStatusService
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly HttpClient _httpClient;
        private readonly ConcurrentDictionary<string, long> _userIdCache = new ConcurrentDictionary<string, long>(StringComparer.OrdinalIgnoreCase);

        public RobloxPlayerStatusService()
        {
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(10)
            };
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "RobloxPlayerNotifierApp/2.0");
        }

        public async Task<PlayerStatusModel> GetPlayerStatus(string robloxPlayerName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(robloxPlayerName))
                    return null;

                if (!_userIdCache.TryGetValue(robloxPlayerName, out long userId))
                {
                    userId = await ResolveUserIdAsync(robloxPlayerName);
                    if (userId > 0)
                    {
                        _userIdCache[robloxPlayerName] = userId;
                    }
                }

                if (userId <= 0)
                {
                    _logger.Warn($"Could not resolve Roblox UserId for '{robloxPlayerName}'");
                    return new PlayerStatusModel(robloxPlayerName, PlayerStatus.Offline, $"https://www.roblox.com/search/users?keyword={robloxPlayerName}");
                }

                var status = await GetPresenceAsync(userId);
                var profileUrl = $"https://www.roblox.com/users/{userId}/profile";

                _logger.Debug($"PlayerStatus for '{robloxPlayerName}' (ID: {userId}) is '{status}'");
                return new PlayerStatusModel(robloxPlayerName, status, profileUrl);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error fetching status for '{robloxPlayerName}'");
                return null;
            }
        }

        private async Task<long> ResolveUserIdAsync(string robloxPlayerName)
        {
            var requestUrl = "https://users.roblox.com/v1/usernames/users";
            var payload = new UsernameLookupRequest { Usernames = new[] { robloxPlayerName }, ExcludeBannedUsers = false };
            var json = JsonSerializer.Serialize(payload);
            using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
            {
                using (var response = await _httpClient.PostAsync(requestUrl, content))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var result = JsonSerializer.Deserialize<UsernameLookupResponse>(responseContent);
                        var user = result?.Data?.FirstOrDefault();
                        if (user != null)
                        {
                            return user.Id;
                        }
                    }
                }
            }
            return 0;
        }

        private async Task<PlayerStatus> GetPresenceAsync(long userId)
        {
            var requestUrl = "https://presence.roblox.com/v1/presence/users";
            var payload = new PresenceLookupRequest { UserIds = new[] { userId } };
            var json = JsonSerializer.Serialize(payload);
            using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
            {
                using (var response = await _httpClient.PostAsync(requestUrl, content))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var result = JsonSerializer.Deserialize<PresenceLookupResponse>(responseContent);
                        var presence = result?.UserPresences?.FirstOrDefault();
                        if (presence != null)
                        {
                            switch (presence.UserPresenceType)
                            {
                                case 2:
                                    return PlayerStatus.Playing;
                                case 1:
                                case 3:
                                    return PlayerStatus.Online;
                                default:
                                    return PlayerStatus.Offline;
                            }
                        }
                    }
                }
            }
            return PlayerStatus.Offline;
        }

        private class UsernameLookupRequest
        {
            [JsonPropertyName("usernames")]
            public string[] Usernames { get; set; }

            [JsonPropertyName("excludeBannedUsers")]
            public bool ExcludeBannedUsers { get; set; }
        }

        private class UsernameLookupResponse
        {
            [JsonPropertyName("data")]
            public List<UserData> Data { get; set; }
        }

        private class UserData
        {
            [JsonPropertyName("id")]
            public long Id { get; set; }

            [JsonPropertyName("name")]
            public string Name { get; set; }
        }

        private class PresenceLookupRequest
        {
            [JsonPropertyName("userIds")]
            public long[] UserIds { get; set; }
        }

        private class PresenceLookupResponse
        {
            [JsonPropertyName("userPresences")]
            public List<UserPresence> UserPresences { get; set; }
        }

        private class UserPresence
        {
            [JsonPropertyName("userPresenceType")]
            public int UserPresenceType { get; set; }

            [JsonPropertyName("userId")]
            public long UserId { get; set; }
        }
    }
}
