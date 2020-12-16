using System;
using System.IO;

namespace Regul.S3PI
{
    public class TS3CC
    {
        public static string Sims3MyDocFolder
        {
            get => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Electronic Arts\\The Sims 3";
        }

        public static string ModFolder => Path.Combine(Sims3MyDocFolder, "Mods");

        public static string DCCacheFolder => Path.Combine(Sims3MyDocFolder, "DCCache");

        public static string InstalledWorldsFolder => Path.Combine(Sims3MyDocFolder, "InstalledWorlds");
    }
}
