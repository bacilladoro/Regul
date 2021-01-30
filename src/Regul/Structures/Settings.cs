using System.Collections.Generic;

namespace Regul.Structures
{
    public class Settings
    {
        public string CreatorName { get; set; }
        public string Theme { get; set; } = "Dazzling";
        public string Language { get; set; }

        public bool FirstRun { get; set; } = true;

        public bool HardwareAcceleration { get; set; }

        public List<Project> Projects { get; set; } = new();
    }
}
