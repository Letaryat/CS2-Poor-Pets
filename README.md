# CS2-Poor-Pets
This plugin allows for players to create a little companion that would follow them around.<br/>
[![poor-developer discord server](https://i.imgur.com/8L6KsUZ.png)](https://discord.gg/mEmdyqM3Um)
[![ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/H2H8TK0L9)

## [📺] Video presentation
[![CS2-Poor-Pets Showcase](http://img.youtube.com/vi/22hd4rVBVBI/0.jpg)](https://www.youtube.com/watch?v=22hd4rVBVBI "CS2-Poor-Pets Showcase")

## [📌] Dependencies
- [Metamod](https://www.sourcemm.net/downloads.php?branch=dev),
- [MultiAddonManager](https://github.com/Source2ZE/MultiAddonManager/releases) - For pet models,
- [CounterStrikeSharp](https://github.com/schwarper/CS2MenuManager),
- [CS2MenuManager](https://github.com/schwarper/CS2MenuManager),

## [📌] Setup
- Download latest release,
- Drag files to /plugins/
- Restart your server,
- Config file should be created in configs/plugins/
- Edit to your liking,

## [🛡️] Commands
| Command  | Description |
| ------------- | ------------- |
| css_pets | Opens up pets menu |

### [📝] Pet configuration
| Option  | Description |
| ------------- | ------------- |
| petName (string) | Name of a pet that would show up in the menu |
| isVipOnly (bool) | If pet should be only available for player with vip flag |
| petModel (string) | Path in your addon to a Pet model |
| idleAnimation (string) | Animation for a pet when idling |
| runAnimation (string) | Animation for a pet when running |
| spawnAnimation (string) | Animation for a pet when spawning |
| deathAnimation (string) | Animation for a pet when player dies |
| isFlying (bool) | If pet should fly |
| moveSpeed (float) | Speed of a pet |
| rotationOffset (float) | Rotation offset of pet model |
| followDistance (float) | How long distance should be between pet and the player to start following player |
| stopDistance (float) | Distance between Player and Pet to stop pet |
| offset (float[]) | Offset from player: X,Y,Z |

### [📝] Config example:
```
{
  "DatabaseConfig": {
    "DB_HOST": "localhost",
    "DB_Port": 3306,
    "DB_User": "root",
    "DB_Name": "db_",
    "DB_Password": "password"
  },
  "VipFlag": "@pets/vip",
  "UpdatePerTicks": 35,
  "TimeAfterDeathToDeletePet": 10,
  "Pets": [
    {
      "petName": "Beaver",
      "isVipOnly": false,
      "petModel": "models/pets/cskull/dota2/beaverknight/beaverknight.vmdl",
      "idleAnimation": "@courier_idle",
      "runAnimation": "@courier_run",
      "spawnAnimation": "@courier_spawn",
      "isFlying": false,
      "moveSpeed": 200,
      "rotation_offset": -90,
      "follow_distance": 100,
      "stop_distance": 70,
      "offset": [
        -30,
        -20,
        0
      ]
    },
    {
      "petName": "Snowl",
      "isVipOnly": false,
      "petModel": "models/pets/cskull/dota2/snowl/snowl_flying.vmdl",
      "idleAnimation": "@courier_idle",
      "runAnimation": "@courier_run",
      "spawnAnimation": "@courier_spawn",
      "isFlying": true,
      "moveSpeed": 200,
      "rotation_offset": -90,
      "follow_distance": 100,
      "stop_distance": 70,
      "offset": [
        -30,
        -20,
        50
      ]
    },
    {
      "petName": "Llama",
      "isVipOnly": true,
      "petModel": "models/pets/cskull/dota2/livery_llama_courier/livery_llama_courier.vmdl",
      "idleAnimation": "@courier_idle",
      "runAnimation": "@courier_run",
      "spawnAnimation": "@courier_spawn",
      "isFlying": false,
      "moveSpeed": 200,
      "rotation_offset": -90,
      "follow_distance": 100,
      "stop_distance": 70,
      "offset": [
        -30,
        -20,
        0
      ]
    }
  ],
  "ConfigVersion": 1
}
```

## [❤️] Special thanks to:
- [Krazy](https://steamcommunity.com/sharedfiles/filedetails/?id=3384346532) - For pet models,
- [Schwarper](https://github.com/schwarper/CS2MenuManager) - For menu,

### [🚨] Plugin might be poorly written and have some issues. I have no idea what I am doing, but when tested it worked fine.