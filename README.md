# GameTimeFX

Vintage Story client mod. Writes in-game time and temporal storm state to a plain-text file every 2 seconds so external tools can read it.

## Output

File: `<VintagestoryData>/gametime.txt`

```
14.3812,0
```

First value is the in-game hour (0.0 to 24.0). Second value is the storm flag (0 = clear, 1 = temporal storm active).

## Use cases

- RGB and LED lighting that follows in-game time of day
- Smart bulbs and bias lighting
- Stream overlays showing in-game time
- OBS scene switching at dawn, dusk, or storm
- Wallpaper Engine and Rainmeter widgets
- Sound and ambient audio triggers
- Home Assistant and smart home automation
- Any script or app that can read a text file

## Install

Drop `gametimefx.zip` into your Mods folder:

**Windows:** `%APPDATA%\VintagestoryData\Mods\`
**Mac:** `~/Library/Application Support/VintagestoryData/Mods/`
**Linux:** `~/.config/VintagestoryData/Mods/`

Vintage Story compiles and loads it automatically on next launch. No dependencies. No server install. Client side only.

## Compatibility

Built and tested on VS 1.22. Storm detection falls back gracefully on older versions.

## Mod page

https://mods.vintagestory.at/show/mod/51182

## License

MIT

## Author

LoadingTunes
