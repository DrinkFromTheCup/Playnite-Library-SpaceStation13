using Playnite.SDK;
using Playnite.SDK.Data;
using Playnite.SDK.Models;
using Playnite.SDK.Plugins;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SpaceStation13
{
    [LoadPlugin]
    public class SpaceStation13 : LibraryPluginBase<SpaceStation13SettingsViewModel>
    {
        // For Step 1, it is enough to keep this mess as is.
        // I'll probably expand on it later.
        bool SpaceStation13Installed
        {
            get
            {
                if (string.IsNullOrEmpty(SpaceStation13Checks.InstallationPath) || !File.Exists(SpaceStation13Checks.InstallationPath))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        // End of offloaded info about possible games.

        // Start of SpaceStation13 plugin definitions
        public SpaceStation13(IPlayniteAPI api) : base(
            "Space Station 13 (BYOND)",
            Guid.Parse("3bf61d13-9730-442a-b9c0-f7228c8cd529"),
            // No need in auto-close.
            // No need in extra settings either.
            new LibraryPluginProperties { CanShutdownClient = false, HasSettings = false },
            new SpaceStation13ChecksClient(),
            SpaceStation13Checks.Icon,
            (_) => new SpaceStation13SettingsView(),
            api)
        {
            // No settings - no problem. Looks optional.
            //SettingsViewModel = new ParadoxlibrarySettingsViewModel(this, api);
        }

        public override IEnumerable<GameMetadata> GetGames(LibraryGetGamesArgs args)
        {
            return new List<GameMetadata>()
            {
                // Sober people do cycles - but I'm too unskilled currently.

                // Start of new game entry.
                new GameMetadata()
                {
                    Name = "Space Station 13",
                    // Keeping it generic since I didn't finished the research on BYOND's internal IDs yet.
                    // There are at least two different kinds (one of which is needed for connecting to servers -
                    // but we'd better import whole playlist with whole links instead of assembling it on the go.
                    // Eventually.
                    GameId = "spacestation13",
                    GameActions = new List<GameAction>
                    {
                        new GameAction()
                        {
                            Name = "Play",
                            Type = GameActionType.URL,
                            Path = @"byond://",
                            IsPlayAction = true
                        }
                    },
                    IsInstalled = SpaceStation13Installed,
                    Source = new MetadataNameProperty("Space Station 13"),
                    Links = new List<Link>()
                    {
                        new Link("Store", @"http://www.byond.com/games/exadv1/spacestation13")
                    },
                    Platforms = new HashSet<MetadataProperty> { new MetadataSpecProperty("pc_windows") }
                }
                // End of new game entry. Do note that last entry in the list should not have comma as last symbol.
            };
        }


        // Start of blatant install/uninstall links adding.
        // I'd really like to utilize something much more simple since we have only one entry point anyway,
        // in a form of stand-alone launcher, but we're having what we're having for now.
        public override IEnumerable<InstallController> GetInstallActions(GetInstallActionsArgs args)
        {
            if (args.Game.PluginId != Id)
            {
                yield break;
            }

            yield return new SpaceStation13InstallController(args.Game);
        }

        public override IEnumerable<UninstallController> GetUninstallActions(GetUninstallActionsArgs args)
        {
            if (args.Game.PluginId != Id)
            {
                yield break;
            }

            yield return new SpaceStation13UninstallController(args.Game);
        }
        // End of blatant install/uninstall links adding.

        // No settings - no problem. Looks optional.
        //public override ISettings GetSettings(bool firstRunSettings)
        //{
        //    return settings;
        //}

        public override UserControl GetSettingsView(bool firstRunSettings)
        {
            return new SpaceStation13SettingsView();
        }
    }
}