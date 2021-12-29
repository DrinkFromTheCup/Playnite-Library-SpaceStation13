using Microsoft.Win32;
using Playnite.SDK;
using Playnite.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SpaceStation13
{
    public class SpaceStation13ChecksClient : LibraryClient
    {
        public override string Icon => SpaceStation13Checks.Icon;

        public override bool IsInstalled => SpaceStation13Checks.IsInstalled;

        public override void Open()
        {
            ProcessStarter.StartProcess(SpaceStation13Checks.ClientExecPath, string.Empty);
        }
    }

    public class SpaceStation13Checks
    {

        // Do note that since we got everything necessary in InstallationPath few blocks below,
        // we don't need to append anything anywhere. Unless it bugs. Which it won't.
        public static string ClientExecPath
        {
            get
            {
                var path = InstallationPath;
                return string.IsNullOrEmpty(path) ? string.Empty : Path.Combine(path, "");
            }
        }

        public static bool IsInstalled
        {
            get
            {
                if (string.IsNullOrEmpty(ClientExecPath) || !File.Exists(ClientExecPath))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        // The plugin's Sequence begins here.
        // Our #1 goal is to get the path to launcher.
        // For this particular game, however, we're blessed with direct link to the launcher's executable.
        // Even better, since we don't need to launch anything else ever (for now) - it all begins here
        // and here it all ends. The rest of the code just works by example.
        public static string InstallationPath
        {
            get
            {
                RegistryKey key;
                key = Registry.ClassesRoot.OpenSubKey(@"byond\shell\open\command");
                if (key?.GetValueNames().Contains("byond.exe") == true)
                {
                    // Keeping old slashes replacer out of habit. Will be cleaned up eventually.
                    return key.GetValue("").ToString().Replace("\\\\", "\\");
                }          

                return string.Empty;
            }
        }

        public static string Icon => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"icon.png");
    }
}