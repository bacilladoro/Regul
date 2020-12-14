using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Regul.S3PI
{
    public class TS3CC
    {
        private static Dictionary<string, string> Locales = new Dictionary<string, string>()
    {
      {
        "en-US",
        Encoding.Unicode.GetString(new byte[20]
        {
          (byte) 84,
          (byte) 0,
          (byte) 104,
          (byte) 0,
          (byte) 101,
          (byte) 0,
          (byte) 32,
          (byte) 0,
          (byte) 83,
          (byte) 0,
          (byte) 105,
          (byte) 0,
          (byte) 109,
          (byte) 0,
          (byte) 115,
          (byte) 0,
          (byte) 32,
          (byte) 0,
          (byte) 51,
          (byte) 0
        })
      },
      {
        "zh-CN",
        Encoding.Unicode.GetString(new byte[10]
        {
          (byte) 33,
          (byte) 106,
          (byte) 223,
          (byte) 98,
          (byte) 186,
          (byte) 78,
          (byte) 31,
          (byte) 117,
          (byte) 51,
          (byte) 0
        })
      },
      {
        "zh-TW",
        Encoding.Unicode.GetString(new byte[10]
        {
          (byte) 33,
          (byte) 106,
          (byte) 236,
          (byte) 100,
          (byte) 2,
          (byte) 94,
          (byte) 17,
          (byte) 108,
          (byte) 51,
          (byte) 0
        })
      },
      {
        "cs-CZ",
        Encoding.Unicode.GetString(new byte[20]
        {
          (byte) 84,
          (byte) 0,
          (byte) 104,
          (byte) 0,
          (byte) 101,
          (byte) 0,
          (byte) 32,
          (byte) 0,
          (byte) 83,
          (byte) 0,
          (byte) 105,
          (byte) 0,
          (byte) 109,
          (byte) 0,
          (byte) 115,
          (byte) 0,
          (byte) 32,
          (byte) 0,
          (byte) 51,
          (byte) 0
        })
      },
      {
        "da-DK",
        Encoding.Unicode.GetString(new byte[20]
        {
          (byte) 84,
          (byte) 0,
          (byte) 104,
          (byte) 0,
          (byte) 101,
          (byte) 0,
          (byte) 32,
          (byte) 0,
          (byte) 83,
          (byte) 0,
          (byte) 105,
          (byte) 0,
          (byte) 109,
          (byte) 0,
          (byte) 115,
          (byte) 0,
          (byte) 32,
          (byte) 0,
          (byte) 51,
          (byte) 0
        })
      },
      {
        "nl-NL",
        Encoding.Unicode.GetString(new byte[18]
        {
          (byte) 68,
          (byte) 0,
          (byte) 101,
          (byte) 0,
          (byte) 32,
          (byte) 0,
          (byte) 83,
          (byte) 0,
          (byte) 105,
          (byte) 0,
          (byte) 109,
          (byte) 0,
          (byte) 115,
          (byte) 0,
          (byte) 32,
          (byte) 0,
          (byte) 51,
          (byte) 0
        })
      },
      {
        "fi-FI",
        Encoding.Unicode.GetString(new byte[20]
        {
          (byte) 84,
          (byte) 0,
          (byte) 104,
          (byte) 0,
          (byte) 101,
          (byte) 0,
          (byte) 32,
          (byte) 0,
          (byte) 83,
          (byte) 0,
          (byte) 105,
          (byte) 0,
          (byte) 109,
          (byte) 0,
          (byte) 115,
          (byte) 0,
          (byte) 32,
          (byte) 0,
          (byte) 51,
          (byte) 0
        })
      },
      {
        "fr-FR",
        Encoding.Unicode.GetString(new byte[20]
        {
          (byte) 76,
          (byte) 0,
          (byte) 101,
          (byte) 0,
          (byte) 115,
          (byte) 0,
          (byte) 32,
          (byte) 0,
          (byte) 83,
          (byte) 0,
          (byte) 105,
          (byte) 0,
          (byte) 109,
          (byte) 0,
          (byte) 115,
          (byte) 0,
          (byte) 32,
          (byte) 0,
          (byte) 51,
          (byte) 0
        })
      },
      {
        "de-DE",
        Encoding.Unicode.GetString(new byte[20]
        {
          (byte) 68,
          (byte) 0,
          (byte) 105,
          (byte) 0,
          (byte) 101,
          (byte) 0,
          (byte) 32,
          (byte) 0,
          (byte) 83,
          (byte) 0,
          (byte) 105,
          (byte) 0,
          (byte) 109,
          (byte) 0,
          (byte) 115,
          (byte) 0,
          (byte) 32,
          (byte) 0,
          (byte) 51,
          (byte) 0
        })
      },
      {
        "el-GR",
        Encoding.Unicode.GetString(new byte[20]
        {
          (byte) 84,
          (byte) 0,
          (byte) 104,
          (byte) 0,
          (byte) 101,
          (byte) 0,
          (byte) 32,
          (byte) 0,
          (byte) 83,
          (byte) 0,
          (byte) 105,
          (byte) 0,
          (byte) 109,
          (byte) 0,
          (byte) 115,
          (byte) 0,
          (byte) 32,
          (byte) 0,
          (byte) 51,
          (byte) 0
        })
      },
      {
        "hu-HU",
        Encoding.Unicode.GetString(new byte[20]
        {
          (byte) 84,
          (byte) 0,
          (byte) 104,
          (byte) 0,
          (byte) 101,
          (byte) 0,
          (byte) 32,
          (byte) 0,
          (byte) 83,
          (byte) 0,
          (byte) 105,
          (byte) 0,
          (byte) 109,
          (byte) 0,
          (byte) 115,
          (byte) 0,
          (byte) 32,
          (byte) 0,
          (byte) 51,
          (byte) 0
        })
      },
      {
        "it-IT",
        Encoding.Unicode.GetString(new byte[20]
        {
          (byte) 84,
          (byte) 0,
          (byte) 104,
          (byte) 0,
          (byte) 101,
          (byte) 0,
          (byte) 32,
          (byte) 0,
          (byte) 83,
          (byte) 0,
          (byte) 105,
          (byte) 0,
          (byte) 109,
          (byte) 0,
          (byte) 115,
          (byte) 0,
          (byte) 32,
          (byte) 0,
          (byte) 51,
          (byte) 0
        })
      },
      {
        "ja-JP",
        Encoding.Unicode.GetString(new byte[12]
        {
          (byte) 182,
          (byte) 48,
          (byte) 101,
          byte.MaxValue,
          (byte) 183,
          (byte) 48,
          (byte) 224,
          (byte) 48,
          (byte) 186,
          (byte) 48,
          (byte) 19,
          byte.MaxValue
        })
      },
      {
        "ko-KR",
        Encoding.Unicode.GetString(new byte[8]
        {
          (byte) 236,
          (byte) 194,
          (byte) 136,
          (byte) 201,
          (byte) 32,
          (byte) 0,
          (byte) 51,
          (byte) 0
        })
      },
      {
        "no-NO",
        Encoding.Unicode.GetString(new byte[20]
        {
          (byte) 84,
          (byte) 0,
          (byte) 104,
          (byte) 0,
          (byte) 101,
          (byte) 0,
          (byte) 32,
          (byte) 0,
          (byte) 83,
          (byte) 0,
          (byte) 105,
          (byte) 0,
          (byte) 109,
          (byte) 0,
          (byte) 115,
          (byte) 0,
          (byte) 32,
          (byte) 0,
          (byte) 51,
          (byte) 0
        })
      },
      {
        "pl-PL",
        Encoding.Unicode.GetString(new byte[20]
        {
          (byte) 84,
          (byte) 0,
          (byte) 104,
          (byte) 0,
          (byte) 101,
          (byte) 0,
          (byte) 32,
          (byte) 0,
          (byte) 83,
          (byte) 0,
          (byte) 105,
          (byte) 0,
          (byte) 109,
          (byte) 0,
          (byte) 115,
          (byte) 0,
          (byte) 32,
          (byte) 0,
          (byte) 51,
          (byte) 0
        })
      },
      {
        "pt-PT",
        Encoding.Unicode.GetString(new byte[18]
        {
          (byte) 79,
          (byte) 0,
          (byte) 115,
          (byte) 0,
          (byte) 32,
          (byte) 0,
          (byte) 83,
          (byte) 0,
          (byte) 105,
          (byte) 0,
          (byte) 109,
          (byte) 0,
          (byte) 115,
          (byte) 0,
          (byte) 32,
          (byte) 0,
          (byte) 51,
          (byte) 0
        })
      },
      {
        "pt-BR",
        Encoding.Unicode.GetString(new byte[20]
        {
          (byte) 84,
          (byte) 0,
          (byte) 104,
          (byte) 0,
          (byte) 101,
          (byte) 0,
          (byte) 32,
          (byte) 0,
          (byte) 83,
          (byte) 0,
          (byte) 105,
          (byte) 0,
          (byte) 109,
          (byte) 0,
          (byte) 115,
          (byte) 0,
          (byte) 32,
          (byte) 0,
          (byte) 51,
          (byte) 0
        })
      },
      {
        "ru-RU",
        Encoding.Unicode.GetString(new byte[20]
        {
          (byte) 84,
          (byte) 0,
          (byte) 104,
          (byte) 0,
          (byte) 101,
          (byte) 0,
          (byte) 32,
          (byte) 0,
          (byte) 83,
          (byte) 0,
          (byte) 105,
          (byte) 0,
          (byte) 109,
          (byte) 0,
          (byte) 115,
          (byte) 0,
          (byte) 32,
          (byte) 0,
          (byte) 51,
          (byte) 0
        })
      },
      {
        "es-ES",
        Encoding.Unicode.GetString(new byte[20]
        {
          (byte) 76,
          (byte) 0,
          (byte) 111,
          (byte) 0,
          (byte) 115,
          (byte) 0,
          (byte) 32,
          (byte) 0,
          (byte) 83,
          (byte) 0,
          (byte) 105,
          (byte) 0,
          (byte) 109,
          (byte) 0,
          (byte) 115,
          (byte) 0,
          (byte) 32,
          (byte) 0,
          (byte) 51,
          (byte) 0
        })
      },
      {
        "es-MX",
        Encoding.Unicode.GetString(new byte[20]
        {
          (byte) 84,
          (byte) 0,
          (byte) 104,
          (byte) 0,
          (byte) 101,
          (byte) 0,
          (byte) 32,
          (byte) 0,
          (byte) 83,
          (byte) 0,
          (byte) 105,
          (byte) 0,
          (byte) 109,
          (byte) 0,
          (byte) 115,
          (byte) 0,
          (byte) 32,
          (byte) 0,
          (byte) 51,
          (byte) 0
        })
      },
      {
        "sv-SE",
        Encoding.Unicode.GetString(new byte[20]
        {
          (byte) 84,
          (byte) 0,
          (byte) 104,
          (byte) 0,
          (byte) 101,
          (byte) 0,
          (byte) 32,
          (byte) 0,
          (byte) 83,
          (byte) 0,
          (byte) 105,
          (byte) 0,
          (byte) 109,
          (byte) 0,
          (byte) 115,
          (byte) 0,
          (byte) 32,
          (byte) 0,
          (byte) 51,
          (byte) 0
        })
      },
      {
        "th-TH",
        Encoding.Unicode.GetString(new byte[22]
        {
          (byte) 64,
          (byte) 14,
          (byte) 20,
          (byte) 14,
          (byte) 45,
          (byte) 14,
          (byte) 48,
          (byte) 14,
          (byte) 11,
          (byte) 14,
          (byte) 52,
          (byte) 14,
          (byte) 33,
          (byte) 14,
          (byte) 42,
          (byte) 14,
          (byte) 76,
          (byte) 14,
          (byte) 32,
          (byte) 0,
          (byte) 51,
          (byte) 0
        })
      }
    };

        public static string Sims3MyDocFolder
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Electronic Arts\\The Sims 3";
            }
        }

        public static string ModFolder => Path.Combine(TS3CC.Sims3MyDocFolder, "Mods");

        public static string DCCacheFolder => Path.Combine(TS3CC.Sims3MyDocFolder, "DCCache");

        public static string InstalledWorldsFolder => Path.Combine(TS3CC.Sims3MyDocFolder, "InstalledWorlds");

        public static string CultureName => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Sims\\The Sims 3", false).GetValue("Locale").ToString();
    }
}
