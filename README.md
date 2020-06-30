# RobloxPlayerNotifier
Want to get a notification when your favorite Roblox player (read: Youtuber) is playing?

My kids want to know that too, as they can then try to join their favorite players and maybe meet them in Roblox - yes thats a big thing for kids these days.

My 2 min. Google research told me that Roblox don't have that feature already and nobody had created a similar service that did it. 
To be honest as a developer, I actually wanted to create this small tool myself, just for the fun of it :)

**NOTE: This is a Windows only application**

## Getting started

### 1. Download and unzip
[Download latest release](https://github.com/Laumania/RobloxPlayerNotifier/releases) and unzip it.

### 2. Add your favorite Roblox players
To replace the default Roblox players in the app you need to open the file `RobloxPlayerNotifierApp.exe.config`.

In there you will find this line:
```xml
<add key="PlayerNamesToMonitor" value="ComKeanOfficial;Emil_trier;Mikkel_trier"/>
```
Replace the "ComKeanOfficial;Emil_trier;Mikkel_trier", with a semi-colon (;) seperated list of Roblox players you want to get notifications for.

Example:
```xml
<add key="PlayerNamesToMonitor" value="Player01;Player02;Player03"/>
```
### 3. Run the applications
Now start the application - "RobloxPlayerNotifierApp.exe" and you should see the application running.
IMPORTANT: The application have to run (can be minimized) for it to work, it doesn't run in the background or in tray.


### 4. Await players to play a game
When one of the players you have added to your config file joins a game (try it with your own username to see it working) a notification will be shown in the lower left corner.

## FAQ
- Why didn't you just build this in JavaScript, then it would have worked on Mac and Linux too?
  - Because it's JavaScript! --> https://www.destroyallsoftware.com/talks/wat ... you want more? Ok, https://hackernoon.com/how-it-feels-to-learn-javascript-in-2016-d3a717dd577f


## History
This project was originally created as a quick and dirty solution for my kids to get notified when there favorite youtubers came online in Roblox. Yes, I know, a tool you cannot live without :P
Don't take it too serious as it was created in a few hours. However, it solved the "problem" my kids had :)
