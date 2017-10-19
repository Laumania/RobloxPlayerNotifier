using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobloxPlayerNotifierApp.Models
{
    public class PlayerStatusModel
    {
        public PlayerStatusModel(string name, PlayerStatus status)
        {
            Name    = name;
            Status  = status;
        }
        public string Name          { get; }
        public PlayerStatus Status  { get; }
    }
}
