using System;
using System.Runtime.InteropServices;

namespace RobloxPlayerNotifierApp
{
    internal static class NativeMethods
    {
        public const int HWND_BROADCAST = 0xffff;
        public static readonly int WM_SHOWME = RegisterWindowMessage("ROBLOX_PLAYER_NOTIFIER_SHOW_WINDOW");

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int RegisterWindowMessage(string message);
    }
}
