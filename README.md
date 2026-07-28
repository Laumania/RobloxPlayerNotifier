# RobloxPlayerNotifier 🚀

A modern Windows application to track your favorite Roblox players and receive instant notifications (audio & Windows toast notifications) when they come **Online** or start **Playing** a game!

> **NOTE: Windows Only Application (.NET 8.0 WPF)**

---

## Key Features & Improvements

- **Official Roblox APIs**: Upgraded from legacy web scraping to official Roblox APIs for real-time presence checks.
- **Modern .NET 8.0 WPF UI**: Sleek, modern interface built with .NET 8.
- **Full UI Management**: Easily **Add** or **Delete** players directly inside the application UI, and customize the status check interval without editing config files manually.
- **System Tray & Background Mode**: Minimizing or closing the window minimizes it cleanly to the Windows System Tray (notification area).
- **Windows Toast Notifications**: Displays native Windows notification banners whenever a player goes online or joins a game.
- **Audio Alerts**: Plays a sound notification when monitored players change status to Online/Playing.
- **Single Instance Protection**: Re-launching the executable automatically brings the running background window back to the front.
- **One-Click Profile Access**: Quick "Join" button to launch the player's Roblox profile in your default browser.

---

## Getting Started

### 1. Download & Run
1. Download the latest build from the [Releases](https://github.com/Laumania/RobloxPlayerNotifier/releases) page.
2. Run `RobloxPlayerNotifierApp.exe`.

### 2. Manage Players & Settings in UI
- **Add Player**: Type any Roblox username into the top input box and click **+ Add Player**.
- **Delete Player**: Click the **Delete** button next to any player in the list.
- **Adjust Interval**: Modify the **Interval (s)** input box to change how often the app checks player statuses (e.g. 15s).
- **Refresh**: Click **Refresh** to trigger an immediate status check.

---

## Building from Source

### Requirements
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later.

### Build Commands
```powershell
# Clone the repository
git clone https://github.com/Laumania/RobloxPlayerNotifier.git
cd RobloxPlayerNotifier

# Build and publish
dotnet publish RobloxPlayerNotifierApp/RobloxPlayerNotifierApp/RobloxPlayerNotifierApp.csproj -c Release
```

---

## License
Licensed under the [MIT License](LICENSE).
