// GameTimeFX - Vintage Story client-side source mod
// Author  : LoadingTunes
// Version : 1.0.0
// License : MIT
//
// PURPOSE
//   Writes the current in-game hour and temporal storm state to a plain-text
//   file every 2 seconds so external programs can read it without touching
//   the game process directly.
//
// OUTPUT FILE
//   <VintagestoryData>/gametime.txt  (cross-platform via GamePaths.DataPath)
//   Format: "<hour>,<storm>"
//   Examples:
//     "14.3812,0"   2:22 PM in-game, no storm
//     "6.0500,1"    just after 6 AM in-game, temporal storm active
//
// INSTALL
//   Drop gametimefx.zip into:
//   %APPDATA%\VintagestoryData\Mods\
//   Vintage Story compiles and loads it automatically on next launch.
//
// COMPATIBILITY
//   Tested on VS 1.22. Storm detection uses reflection so it degrades
//   gracefully on older versions that lack InTemporalStorm.

using System;
using System.IO;
using System.Reflection;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

[assembly: ModInfo("GameTimeFX", "gametimefx",
    Version     = "1.0.0",
    Description = "Writes in-game time and temporal storm status to a file for external tools.",
    Authors     = new[] { "LoadingTunes" }
)]

namespace GameTimeFX
{
    // Same idea as a Bukkit plugin class - VS finds this because it extends ModSystem
    public class Core : ModSystem
    {
        private string _outputPath;

        // Tell VS this only runs on the client, not the server - like checking getSide() in Forge
        public override bool ShouldLoad(EnumAppSide side) =>
            side == EnumAppSide.Client;

        // Same as onEnable() - runs once when the mod loads
        public override void StartClientSide(ICoreClientAPI api)
        {
            // Resolves to the right data folder on Windows, Mac, and Linux
            _outputPath = Path.Combine(GamePaths.DataPath, "gametime.txt");

            // Same as a Bukkit repeating scheduler - runs every 2000ms
            api.Event.RegisterGameTickListener(_ =>
            {
                try
                {
                    // TotalHours keeps counting up forever, % 24 wraps it back to 0-24
                    double hour  = api.World.Calendar.TotalHours % 24.0;
                    int    storm = 0;

                    // InTemporalStorm doesn't exist in older VS versions
                    // Reflection lets us check at runtime before calling it - same as Class.getMethod() in Java
                    try
                    {
                        PropertyInfo prop = api.World.Calendar.GetType()
                            .GetProperty("InTemporalStorm");
                        if (prop != null)
                            storm = (bool)prop.GetValue(api.World.Calendar) ? 1 : 0;
                    }
                    catch { }

                    // Overwrite the file every tick - this is what the LED script reads
                    File.WriteAllText(_outputPath, $"{hour:F4},{storm}");
                }
                catch { }
            }, 2000);
        }

        // Same as onDisable() - nothing to clean up so it stays empty
        public override void Dispose() { }
    }
}
