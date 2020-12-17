using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regul.Structures
{
    public class Settings
    {
        public string CreatorName { get; set; }

        public List<Project> Projects { get; set; } = new();
    }
}
