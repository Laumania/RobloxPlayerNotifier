using System;
using System.ComponentModel;
using System.IO;
using System.Media;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using NLog;
using RobloxPlayerNotifierApp.Models;
using RobloxPlayerNotifierApp.ViewModels;
using Application = System.Windows.Application;

namespace RobloxPlayerNotifierApp.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int SW_SHOW = 5;
        private const int SW_RESTORE = 9;

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly SoundPlayer _alertSoundPlayer;
        private readonly MainViewModel _viewModel;
        private readonly NotifyIcon _notifyIcon;
        private bool _isExiting;

        public MainView()
        {
            InitializeComponent();

            var soundPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sounds", "EasyPeasyLemonSqueezy.wav");
            if (File.Exists(soundPath))
            {
                _alertSoundPlayer = new SoundPlayer(soundPath);
            }
            else
            {
                _alertSoundPlayer = new SoundPlayer("Sounds\\EasyPeasyLemonSqueezy.wav");
            }

            _notifyIcon = new NotifyIcon
            {
                Visible = true,
                Text = "Roblox Player Notifier"
            };

            var iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RobloxNotifier.ico");
            if (File.Exists(iconPath))
            {
                _notifyIcon.Icon = new System.Drawing.Icon(iconPath);
            }
            else
            {
                var processPath = Environment.ProcessPath ?? System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                _notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(processPath);
            }

            _notifyIcon.DoubleClick += (s, e) => ShowWindowFromTray();
            _notifyIcon.BalloonTipClicked += (s, e) => ShowWindowFromTray();

            var contextMenu = new ContextMenuStrip();
            var showItem = new ToolStripMenuItem("Show App", null, (s, e) => ShowWindowFromTray());
            var exitItem = new ToolStripMenuItem("Exit", null, (s, e) => ExitApplication());
            contextMenu.Items.Add(showItem);
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add(exitItem);
            _notifyIcon.ContextMenuStrip = contextMenu;

            this.DataContext = _viewModel = new MainViewModel();

            _viewModel.PlayerStatusChanged += PlayerStatusChanged;
        }

        private void MainView_OnLoaded(object sender, RoutedEventArgs e)
        {
            _viewModel.Start();
        }

        private void PlayerStatusChanged(PlayerStatusModel playerStatusModel)
        {
            if (playerStatusModel.Status == PlayerStatus.Playing || playerStatusModel.Status == PlayerStatus.Online)
            {
                string actionText = playerStatusModel.Status == PlayerStatus.Playing ? "is playing now!" : "is online now!";
                _logger.Info($"'{playerStatusModel.Name}' {actionText} - '{playerStatusModel.PlayerProfileUrl}'");

                _notifyIcon.ShowBalloonTip(5000, "Roblox Player Notifier", $"{playerStatusModel.Name} {actionText}", ToolTipIcon.Info);

                try
                {
                    _alertSoundPlayer.Play();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Failed to play notification sound");
                }
            }
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                this.Hide();
                this.ShowInTaskbar = false;
                _notifyIcon.ShowBalloonTip(3000, "Roblox Player Notifier", "App is running in the background.", ToolTipIcon.Info);
            }
            base.OnStateChanged(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!_isExiting)
            {
                e.Cancel = true;
                this.Hide();
                this.ShowInTaskbar = false;
                _notifyIcon.ShowBalloonTip(3000, "Roblox Player Notifier", "App is running in the background.", ToolTipIcon.Info);
            }
            else
            {
                _notifyIcon.Dispose();
            }
            base.OnClosing(e);
        }

        public void ShowWindowFromTray()
        {
            if (!this.IsVisible)
            {
                this.Show();
            }
            this.ShowInTaskbar = true;
            this.WindowState = WindowState.Normal;
            this.Activate();
            this.Topmost = true;
            this.Topmost = false;
            this.Focus();

            var windowHandle = new WindowInteropHelper(this).Handle;
            if (windowHandle != IntPtr.Zero)
            {
                ShowWindow(windowHandle, SW_SHOW);
                ShowWindow(windowHandle, SW_RESTORE);
                SetForegroundWindow(windowHandle);
            }
        }

        private void ExitApplication()
        {
            _isExiting = true;
            _notifyIcon.Dispose();
            Application.Current.Shutdown();
        }
    }
}
