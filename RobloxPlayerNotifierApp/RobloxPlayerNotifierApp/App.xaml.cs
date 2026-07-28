using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using RobloxPlayerNotifierApp.Views;

namespace RobloxPlayerNotifierApp
{
    public partial class App : Application
    {
        private static Mutex _mutex = null;
        private static EventWaitHandle _wakeupEvent = null;

        protected override void OnStartup(StartupEventArgs e)
        {
            const string mutexName = "RobloxPlayerNotifierApp_SingleInstance_Mutex";
            const string eventName = "RobloxPlayerNotifierApp_SingleInstance_WakeupEvent";

            _mutex = new Mutex(true, mutexName, out bool createdNew);

            if (!createdNew)
            {
                try
                {
                    using (var wakeupEvent = EventWaitHandle.OpenExisting(eventName))
                    {
                        wakeupEvent.Set();
                    }
                }
                catch { }

                Shutdown();
                return;
            }

            _wakeupEvent = new EventWaitHandle(false, EventResetMode.AutoReset, eventName);

            Task.Run(() =>
            {
                while (_wakeupEvent != null)
                {
                    try
                    {
                        if (_wakeupEvent.WaitOne())
                        {
                            Current?.Dispatcher.Invoke(() =>
                            {
                                if (Current.MainWindow is MainView mainView)
                                {
                                    mainView.ShowWindowFromTray();
                                }
                            });
                        }
                    }
                    catch { break; }
                }
            });

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (_wakeupEvent != null)
            {
                try
                {
                    _wakeupEvent.Dispose();
                    _wakeupEvent = null;
                }
                catch { }
            }

            if (_mutex != null)
            {
                try
                {
                    _mutex.ReleaseMutex();
                    _mutex.Dispose();
                    _mutex = null;
                }
                catch { }
            }

            base.OnExit(e);
        }
    }
}
