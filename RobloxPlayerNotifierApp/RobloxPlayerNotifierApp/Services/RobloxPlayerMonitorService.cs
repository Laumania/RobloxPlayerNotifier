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

        public void Start()
        {
            if (IsRunning)
                return;

            IsRunning = true;
            ActualMonitoring();
        }

        public void Stop()
        {
            IsRunning = false;
        }



        private async Task ActualMonitoring()
        {
            while (IsRunning)
            {
                Console.WriteLine("Working....monitoring!!");
                await Task.Delay(5000);
            }
        }






        public bool IsRunning { get; private set; }
    }
}
