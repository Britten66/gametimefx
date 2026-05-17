// GameTimeFX — Vintage Story client-side source mod
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
//     "14.3812,0"   — 2:22 PM in-game, no storm
//     "6.0500,1"    — just after 6 AM in-game, temporal storm active
//
// INSTALL
//   Drop gametimebridge.zip into:
//   %APPDATA%\VintagestoryData\Mods\
//   Vintage Story compiles and loads it automatically on next launch.
//
// COMPATIBILITY
//   Built and tested on VS 1.22. Storm detection uses reflection so it
//   degrades gracefully on older versions that lack InTemporalStorm.

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
    public class Core : ModSystem
    {
        private string _outputPath;

        // Client-side only — no server logic needed
        public override bool ShouldLoad(EnumAppSide side) =>
            side == EnumAppSide.Client;

        public override void StartClientSide(ICoreClientAPI api)
        {
            // GamePaths.DataPath resolves correctly on Windows, Mac, and Linux
            _outputPath = Path.Combine(GamePaths.DataPath, "gametime.txt");

            // Tick every 2000ms — fast enough for smooth LED response, light enough to be invisible
            api.Event.RegisterGameTickListener(_ =>
            {
                try
                {
                    double hour  = api.World.Calendar.TotalHours % 24.0;
                    int    storm = 0;

                    // InTemporalStorm was added in a later VS version; use reflection so
                    // the mod still loads on older builds without a compile error
                    try
                    {
                        PropertyInfo prop = api.World.Calendar.GetType()
                            .GetProperty("InTemporalStorm");
                        if (prop != null)
                            storm = (bool)prop.GetValue(api.World.Calendar) ? 1 : 0;
                    }
                    catch { }

                    File.WriteAllText(_outputPath, $"{hour:F4},{storm}");
                }
                catch { }
            }, 2000);
        }

        public override void Dispose() { }
    }
}
