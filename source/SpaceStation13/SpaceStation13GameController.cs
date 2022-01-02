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

// As seen in https://github.com/JosefNemec/PlayniteExtensions/blob/master/source/Libraries/GogLibrary/GogGameController.cs and the like.
// Despite my bias against keeping redundant processes in memory, borrowing well-written install/uninstall watcher
// seems to be more end-user-friendly than my original intent to just mark everything at will.
// Simplified (almost gutted), since we're handling 1 (one) already-known title only.

namespace SpaceStation13
{
    public class SpaceStation13InstallController : InstallController
    {
        private CancellationTokenSource watcherToken;

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
            watcherToken?.Cancel();
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

            StartInstallWatcher();
        }

        public async void StartInstallWatcher()
        {
            watcherToken = new CancellationTokenSource();
            await Task.Run(async () =>
            {
                while (true)
                {
                    if (watcherToken.IsCancellationRequested)
                    {
                        return;
                    }
                    if (File.Exists(SpaceStation13Checks.InstallationPath))
                    {
                        InvokeOnInstalled(new GameInstalledEventArgs());
                        return;
                    }

                    await Task.Delay(10000);
                }
            });
        }
    }

    public class SpaceStation13UninstallController : UninstallController
    {
        private CancellationTokenSource watcherToken;

        public SpaceStation13UninstallController(Game game) : base(game)
        {
            Name = "Uninstall";
        }

        public override void Dispose()
        {
            watcherToken?.Cancel();
        }

        public override void Uninstall(UninstallActionArgs args)
        {
            Dispose();
            if (!File.Exists(SpaceStation13Checks.ClientExecPath))
            // Launcher was removed but game persisted somehow.
            {
                throw new FileNotFoundException("BYOND has been uninstalled earlier.");
            }
            // Running generic Windows' uninstaller with one simple trick. . .
            ProcessStarter.StartProcess("appwiz.cpl");
            StartUninstallWatcher();
        }

        public async void StartUninstallWatcher()
        {
            watcherToken = new CancellationTokenSource();
            while (true)
            {
                if (watcherToken.IsCancellationRequested)
                {
                    return;
                }

                if (!File.Exists(SpaceStation13Checks.InstallationPath))
                {
                    InvokeOnUninstalled(new GameUninstalledEventArgs());
                    return;
                }

                await Task.Delay(5000);
            }
        }
    }
}
