using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobloxPlayerNotifierApp.Models
{
    public class PlayerStatusModel
    {
        public PlayerStatusModel(string name, PlayerStatus status, string playerProfileUrl)
        {
            Name                = name;
            Status              = status;
            PlayerProfileUrl    = playerProfileUrl;
        }
        public string Name              { get; }
        public PlayerStatus Status      { get; }
        public string PlayerProfileUrl  { get; }
    }
}
