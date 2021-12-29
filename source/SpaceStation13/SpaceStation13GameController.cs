using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Playnite;
using Playnite.Common;
using Playnite.SDK;
using Playnite.SDK.Models;
using Playnite.SDK.Plugins;



namespace SpaceStation13
{
    public class SpaceStation13InstallController : InstallController
    {
        //private CancellationTokenSource watcherToken;

        public SpaceStation13InstallController(Game game) : base(game)
        {
            if (SpaceStation13Checks.IsInstalled)
            {
                Name = "Run with BYOND";
            }
            else
            // Failsafe for cases where we added games anyway but launcher is nowhere to be seen.
            {
                Name = "Download BYOND";
            }
        }

        public override void Dispose()
        {
            //watcherToken?.Cancel();
        }

        public override void Install(InstallActionArgs args)
        {
            if (SpaceStation13Checks.IsInstalled)
            // If we can use URI - we have to use URI.
            // Especially if we use its entry in registry to detect BYOND location and status.
            {
                ProcessStarter.StartUrl(@"byond://");
            }
            else
            // Failsafe for cases where we added games anyway but launcher is nowhere to be seen.
            {
                ProcessStarter.StartUrl(@"http://www.byond.com/download/");
            }

            //StartInstallWatcher();
        }

        //public async void StartInstallWatcher()
        //{
            //watcherToken = new CancellationTokenSource();
            //await Task.Run(async () =>
            //{
            //    while (true)
            //    {
            //        if (watcherToken.IsCancellationRequested)
            //        {
            //            return;
            //        }

                    // Skipping temporarily to avoid going insane.
                    //var games = Paradox.GetInstalledGames();
                    //if (games.ContainsKey(Game.GameId))
                    //{
                    //    var game = games[Game.GameId];
                    //   var installInfo = new GameInstallationData()
                    //    {
                    //        InstallDirectory = game.InstallDirectory
                    //    };

                    //    InvokeOnInstalled(new GameInstalledEventArgs(installInfo));
                    //    return;
            //        }

            //        await Task.Delay(10000);
            //    }
            //});
        //}
    }

    public class SpaceStation13UninstallController : UninstallController
    {
        //private CancellationTokenSource watcherToken;

        public SpaceStation13UninstallController(Game game) : base(game)
        {
            Name = "Uninstall";
        }

        public override void Dispose()
        {
            //watcherToken?.Cancel();
        }

        public override void Uninstall(UninstallActionArgs args)
        {
            Dispose();
            if (!File.Exists(SpaceStation13Checks.ClientExecPath))
            // Launcher was removed but game persisted somehow.
            {
                throw new FileNotFoundException("Uninstaller not found.");
            }
            // NB: for some reason, Process.Start results in successful launcher launch - but all games
            // are listed as uninstalled. Resorting to ProcessStarter since we prefer it anyway...
            ProcessStarter.StartProcess(SpaceStation13Checks.ClientExecPath);
            //StartUninstallWatcher();
        }

        //public async void StartUninstallWatcher()
        //{
            //watcherToken = new CancellationTokenSource();
            //while (true)
            //{
            //    if (watcherToken.IsCancellationRequested)
            //    {
            //        return;
            //    }

                // Skipping temporarily to avoid going insane.
                //var games = Paradox.GetInstalledGames();
                //if (!games.ContainsKey(Game.GameId))
                //{
            //        InvokeOnUninstalled(new GameUninstalledEventArgs());
            //        return;
                //}

                //await Task.Delay(5000);
            //}
        //}
    }
}
