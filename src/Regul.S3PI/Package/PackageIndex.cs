using System.Collections.Generic;
using System.IO;
using Regul.S3PI.Interfaces;

namespace Regul.S3PI.Package
{
    /// <summary>
    /// Internal -- used by Package to manage the package index
    /// </summary>
    internal class PackageIndex : List<IResourceIndexEntry>
    {
        const int numFields = 9;

        uint indextype;
        public uint Indextype { get { return indextype; } }

        int Hdrsize
        {
            get
            {
                int hc = 1;
                for (int i = 0; i < sizeof(uint); i++) if ((indextype & (1 << i)) != 0) hc++;
                return hc;
            }
        }

        public PackageIndex() { }
        public PackageIndex(uint type) { indextype = type; }
        public PackageIndex(Stream s, int indexposition, int indexsize, int indexcount)
        {
            if (s == null) return;
            if (indexposition == 0) return;

            BinaryReader r = new BinaryReader(s);
            s.Position = indexposition;
            indextype = r.ReadUInt32();

            int[] hdr = new int[Hdrsize];
            int[] entry = new int[numFields - Hdrsize];

            hdr[0] = (int)indextype;
            for (int i = 1; i < hdr.Length; i++)
                hdr[i] = r.ReadInt32();

            for (int i = 0; i < indexcount; i++)
            {
                for (int j = 0; j < entry.Length; j++)
                    entry[j] = r.ReadInt32();
                base.Add(new ResourceIndexEntry(hdr, entry));
            }
        }

        public IResourceIndexEntry Add(IResourceKey rk)
        {
            ResourceIndexEntry rc = new ResourceIndexEntry(new int[Hdrsize], new int[numFields - Hdrsize])
            {
                ResourceType = rk.ResourceType,
                ResourceGroup = rk.ResourceGroup,
                Instance = rk.Instance,
                Chunkoffset = 0xffffffff,
                Unknown2 = 1,
                ResourceStream = null
            };


            base.Add(rc);
            return rc;
        }

        public int Size { get { return (Count * (numFields - Hdrsize) + Hdrsize) * 4; } }
        public void Save(BinaryWriter w)
        {
            BinaryReader r = null;
            r = Count == 0 ? new BinaryReader(new MemoryStream(new byte[numFields * 4])) : new BinaryReader(this[0].Stream);
            
            r.BaseStream.Position = 4;
            w.Write(indextype);
            if ((indextype & 0x01) != 0) w.Write(r.ReadUInt32()); else r.BaseStream.Position += 4;
            if ((indextype & 0x02) != 0) w.Write(r.ReadUInt32()); else r.BaseStream.Position += 4;
            if ((indextype & 0x04) != 0) w.Write(r.ReadUInt32()); else r.BaseStream.Position += 4;

            for (int index = 0; index < this.Count; index++)
            {
                IResourceIndexEntry ie = this[index];
                
                r = new BinaryReader(ie.Stream);
                r.BaseStream.Position = 4;
                if ((indextype & 0x01) == 0) w.Write(r.ReadUInt32());
                else r.BaseStream.Position += 4;
                if ((indextype & 0x02) == 0) w.Write(r.ReadUInt32());
                else r.BaseStream.Position += 4;
                if ((indextype & 0x04) == 0) w.Write(r.ReadUInt32());
                else r.BaseStream.Position += 4;
                w.Write(r.ReadBytes((int) (ie.Stream.Length - ie.Stream.Position)));
            }
        }

        /// <summary>
        /// Sort the index by the given field
        /// </summary>
        /// <param name="index">Field to sort by</param>
        public void Sort(string index) { base.Sort(new AApiVersionedFields.Comparer<IResourceIndexEntry>(index)); }

        /// <summary>
        /// Return the index entry with the matching TGI
        /// </summary>
        /// <param name="type">Entry type</param>
        /// <param name="group">Entry group</param>
        /// <param name="instance">Entry instance</param>
        /// <returns>Matching entry</returns>
        public IResourceIndexEntry this[uint type, uint group, ulong instance]
        {
            get
            {
                for (int index = 0; index < this.Count; index++)
                {
                    var rie = (ResourceIndexEntry) this[index];
                    if (rie.ResourceType != type) continue;
                    if (rie.ResourceGroup != @group) continue;
                    if (rie.Instance == instance) return rie;
                }

                return null;
            }
        }

        /// <summary>
        /// Returns requested resource, ignoring EPFlags
        /// </summary>
        /// <param name="rk">Resource key to find</param>
        /// <returns>Matching entry</returns>
        public IResourceIndexEntry this[IResourceKey rk] { get { return this[rk.ResourceType, rk.ResourceGroup, rk.Instance]; } }
    }
}
