# Stardew Valley Save Editor
A save editor for [Stardew Valley](http://store.steampowered.com/app/413150/).

## Current features
- Game backup and restore.
- Basic editing of some properties.
- Quick launch from within application.
- Worst looking UI layout known to man.

## How to edit more properties
1. Clone, fork or download the repositry.
2. Find your saved game directory (%APPDATA%\StardewValley\Saves\)
3. Open the larger of the two save game files in text editor such as Notepad++ and find the XML node.
4. (optional) Format XML into a readable format with Ctrl+Alt+Shift+B or Plugins -> XML Tools -> Pretty Print.
5. For this example we will want to edit the club coins, its xml path is /SaveGame/player/clubCoins
6. Open the config.json that comes with SVSE and locate the correct tab that you wish to put this new property on.
7. As it is a player property we will add it to the player tab so find the json element we need to edit (look for "title": "Player")
8. Under the items, copy and paste an existing item and modify it for your needs. 
```
{
  "type": "number",
  "path": "/clubCoins",
  "title": "Club coins",
  "min": 1
},
```
Save the file and re-launch SVSE and now you can set the club coins property from within the player tab.
