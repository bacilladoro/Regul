using Regul.S3PI;
using System.Collections.Generic;
using System.IO;

namespace Regul.Structures
{
    public class Settings
    {
        public string CreatorName { get; set; }
        public string Theme { get; set; } = "Dazzling";
        public string Language { get; set; }

        public bool DeletingCharacterPortraits { get; set; } = true;
        public bool RemovingLotThumbnails { get; set; }
        public bool RemovingPhotos { get; set; }
        public bool RemovingTextures { get; set; }
        public bool RemovingGeneratedImages { get; set; }
        public bool RemovingFamilyPortraits { get; set; } = true;
        public bool CreateABackup { get; set; }
        public bool ClearCache { get; set; }

        public bool FirstRun { get; set; } = true;

        public string PathToTheSims3Document { get; set; } = TS3CC.Sims3MyDocFolder;
        public string PathToSaves { get; set; } = Path.Combine(TS3CC.Sims3MyDocFolder, "Saves");

        public bool HardwareAcceleration { get; set; }

        public List<Project> Projects { get; set; } = new List<Project>();
    }
}
