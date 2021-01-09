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
        public uint ResourceType { get; set; }
        public uint ResourceGroup { get; set; }
        public ulong Instance { get; set; }
        public uint Chunkoffset { get; set; }
        public uint Filesize { get; set; }
        public uint Memsize { get; set; }
        public ushort Compressed { get; set; }
        public ushort Unknown2 { get; set; }

        public byte[] Data { get; set; }
    }
}
