using Regul.S3PI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regul.Core.TheSims3Type
{
    public class Resource
    {
        public string ResourceName { get; set; }
        public string Tag { get; set; }

        public IResourceIndexEntry ResourceIndexEntry { get; set; }
    }
}
