using System;
using System.IO;
using System.Text.Json;
using Regul.Structures;

namespace Regul.Core
{
    public static class FileSettings
    {
        public static Settings LoadSettings() => JsonSerializer.Deserialize<Settings>(File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "settings.json"));

        public static void SaveSettings() => File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "settings.json", JsonSerializer.Serialize<Settings>(Program.Settings));
    }
}
