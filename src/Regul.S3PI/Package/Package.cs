using Regul.S3PI.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;

namespace Regul.S3PI.Package
{
    /// <summary>
    /// Implementation of a package
    /// </summary>
    public class Package : APackage
    {
        static readonly bool checking = Settings.Settings.Checking;

        #region AApiVersionedFields

        //No ContentFields override as we don't want to make anything more public than APackage provides
        #endregion

        #region APackage
        #region Whole package
        /// <summary>
        /// Tell the package to save itself to wherever it believes it came from
        /// </summary>
        public override void SavePackage()
        {
            if (checking) if (packageStream == null)
                    throw new InvalidOperationException("Package has no stream to save to");
            if (!packageStream.CanWrite)
                throw new InvalidOperationException("Package is read-only");

            // Lock the header while we save to prevent other processes saving concurrently
            // if it's not a file, it's probably safe not to lock it...
            FileStream fs = packageStream as FileStream;
            string tmpfile = Path.GetTempFileName();
            try
            {
                SaveAs(tmpfile);

                fs?.Lock(0, header.Length);

                BinaryReader r = new BinaryReader(new FileStream(tmpfile, FileMode.Open));
                BinaryWriter w = new BinaryWriter(packageStream);
                packageStream.Position = 0;
                w.Write(r.ReadBytes((int)r.BaseStream.Length));
                packageStream.SetLength(packageStream.Position);
                w.Flush();
                r.Close();
            }
            finally
            {
                File.Delete(tmpfile);
                fs?.Unlock(0, header.Length);
            }

            packageStream.Position = 0;
            header = new BinaryReader(packageStream).ReadBytes(header.Length);
            if (checking) CheckHeader();

            bool wasnull = index == null;
            index = null;
            if (!wasnull) OnResourceIndexInvalidated(this, EventArgs.Empty);
        }

        /// <summary>
        /// Save the package to a given stream
        /// </summary>
        /// <param name="s">Stream to save to</param>
        public override void SaveAs(Stream s)
        {
            BinaryWriter w = new BinaryWriter(s);
            w.Write(header);

            List<uint> lT = new List<uint>();
            List<uint> lG = new List<uint>();
            List<uint> lIh = new List<uint>();

            for (int i = 0; i < Index.Count; i++)
            {
                IResourceIndexEntry x = Index[i];
                if (!lT.Contains(x.ResourceType)) lT.Add(x.ResourceType);
                if (!lG.Contains(x.ResourceGroup)) lG.Add(x.ResourceGroup);
                if (!lIh.Contains((uint)(x.Instance >> 32))) lIh.Add((uint)(x.Instance >> 32));
            }

            uint indexType = (uint)(lIh.Count <= 1 ? 0x04 : 0x00) | (uint)(lG.Count <= 1 ? 0x02 : 0x00) | (uint)(lT.Count <= 1 ? 0x01 : 0x00);

            PackageIndex newIndex = new PackageIndex(indexType);
            for (int i = 0; i < Index.Count; i++)
            {
                IResourceIndexEntry ie = Index[i];
                if (ie.IsDeleted) continue;

                ResourceIndexEntry newIE = (ie as ResourceIndexEntry)?.Clone();
                ((List<IResourceIndexEntry>)newIndex).Add(newIE);
                byte[] value = packedChunk(ie as ResourceIndexEntry);

                newIE.Chunkoffset = (uint)s.Position;
                w.Write(value);
                w.Flush();

                if (value.Length < newIE.Memsize)
                {
                    newIE.Compressed = 0xffff;
                    newIE.Filesize = (uint)value.Length;
                }
                else
                {
                    newIE.Compressed = 0x0000;
                    newIE.Filesize = newIE.Memsize;
                }
            }

            long indexpos = s.Position;
            newIndex.Save(w);
            setIndexcount(w, newIndex.Count);
            setIndexsize(w, newIndex.Size);
            setIndexposition(w, (int)indexpos);
            s.Flush();
        }

        /// <summary>
        /// Save the package to a given file
        /// </summary>
        /// <param name="path">File to save to - will be overwritten or created</param>
        public override void SaveAs(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Create);
            SaveAs(fs);
            fs.Close();
        }

        // Static so cannot be defined on the interface

        /// <summary>
        /// Initialise a new, empty package and return the IPackage reference
        /// </summary>
        /// <returns>IPackage reference to an empty package</returns>
        public static new IPackage NewPackage() => new Package();

        /// <summary>
        /// Open an existing package by filename, read only
        /// </summary>
        /// <param name="packagePath">Fully qualified filename of the package</param>
        /// <returns>IPackage reference to an existing package on disk</returns>
        /// <exception cref="ArgumentNullException"><paramref name="packagePath"/> is null.</exception>
        /// <exception cref="FileNotFoundException">The file cannot be found.</exception>
        /// <exception cref="DirectoryNotFoundException"><paramref name="packagePath"/> is invalid, such as being on an unmapped drive.</exception>
        /// <exception cref="PathTooLongException">
        /// <paramref name="packagePath"/>, or a component of the file name, exceeds the system-defined maximum length.
        /// For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="packagePath"/> is an empty string (""), contains only white space, or contains one or more invalid characters.
        /// <br/>-or-<br/>
        /// <paramref name="packagePath"/> refers to a non-file device, such as "con:", "com1:", "lpt1:", etc. in an NTFS environment.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// <paramref name="packagePath"/> refers to a non-file device, such as "con:", "com1:", "lpt1:", etc. in a non-NTFS environment.
        /// </exception>
        /// <exception cref="System.Security.SecurityException">The caller does not have the required permission.</exception>
        /// <exception cref="InvalidDataException">Thrown if the package header is malformed.</exception>
        public static new IPackage OpenPackage(string packagePath) { return OpenPackage(packagePath, false); }
        /// <summary>
        /// Open an existing package by filename, optionally readwrite
        /// </summary>
        /// <param name="APIversion">(unused)</param>
        /// <param name="packagePath">Fully qualified filename of the package</param>
        /// <param name="readwrite">True to indicate read/write access required</param>
        /// <returns>IPackage reference to an existing package on disk</returns>
        /// <exception cref="ArgumentNullException"><paramref name="packagePath"/> is null.</exception>
        /// <exception cref="FileNotFoundException">The file cannot be found.</exception>
        /// <exception cref="DirectoryNotFoundException"><paramref name="packagePath"/> is invalid, such as being on an unmapped drive.</exception>
        /// <exception cref="PathTooLongException">
        /// <paramref name="packagePath"/>, or a component of the file name, exceeds the system-defined maximum length.
        /// For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="packagePath"/> is an empty string (""), contains only white space, or contains one or more invalid characters.
        /// <br/>-or-<br/>
        /// <paramref name="packagePath"/> refers to a non-file device, such as "con:", "com1:", "lpt1:", etc. in an NTFS environment.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// <paramref name="packagePath"/> refers to a non-file device, such as "con:", "com1:", "lpt1:", etc. in a non-NTFS environment.
        /// </exception>
        /// <exception cref="System.Security.SecurityException">The caller does not have the required permission.</exception>
        /// <exception cref="UnauthorizedAccessException">
        /// The access requested is not permitted by the operating system for <paramref name="packagePath"/>,
        /// such as when access is ReadWrite and the file or directory is set for read-only access.
        /// </exception>
        /// <exception cref="InvalidDataException">Thrown if the package header is malformed.</exception>
        public static new IPackage OpenPackage(string packagePath, bool readwrite) => new Package(new FileStream(packagePath, FileMode.Open, readwrite ? FileAccess.ReadWrite : FileAccess.Read, FileShare.ReadWrite));

        /// <summary>
        /// Releases any internal references associated with the given package
        /// </summary>
        /// <param name="pkg">IPackage reference to close</param>
        public static new void ClosePackage(IPackage pkg)
        {
            if (pkg is Package p)
            {
                if (p.packageStream != null) { try { p.packageStream.Close(); } catch { } p.packageStream = null; }
                p.header = null;
                p.index = null;
            }
        }
        #endregion

        #region Package header
        /// <summary>
        /// Package header: "DBPF" bytes
        /// </summary>
        [ElementPriority(1)]
        public override byte[] Magic { get { byte[] res = new byte[4]; Array.Copy(header, 0, res, 0, res.Length); return res; } }
        /// <summary>
        /// Package header: unused
        /// </summary>
        [ElementPriority(2)]
        public override byte[] Unknown1 { get { byte[] res = new byte[24]; Array.Copy(header, 12, res, 0, res.Length); return res; } }
        /// <summary>
        /// Package header: number of entries in the package index
        /// </summary>
        [ElementPriority(3)]
        public override int Indexcount { get { return BitConverter.ToInt32(header, 36); } }
        /// <summary>
        /// Package header: unused
        /// </summary>
        [ElementPriority(4)]
        public override byte[] Unknown2 { get { byte[] res = new byte[4]; Array.Copy(header, 40, res, 0, res.Length); return res; } }
        /// <summary>
        /// Package header: index size on disk in bytes
        /// </summary>
        [ElementPriority(5)]
        public override int Indexsize { get { return BitConverter.ToInt32(header, 44); } }
        /// <summary>
        /// Package header: unused
        /// </summary>
        [ElementPriority(6)]
        public override byte[] Unknown3 { get { byte[] res = new byte[12]; Array.Copy(header, 48, res, 0, res.Length); return res; } }
        /// <summary>
        /// Package header: always 3?
        /// </summary>
        [ElementPriority(7)]
        public override int Indexversion { get { return BitConverter.ToInt32(header, 60); } }
        /// <summary>
        /// Package header: index position in file
        /// </summary>
        [ElementPriority(8)]
        public override int Indexposition { get { int i = BitConverter.ToInt32(header, 64); return i != 0 ? i : BitConverter.ToInt32(header, 40); } }
        /// <summary>
        /// Package header: unused
        /// </summary>
        [ElementPriority(9)]
        public override byte[] Unknown4 { get { byte[] res = new byte[28]; Array.Copy(header, 68, res, 0, res.Length); return res; } }

        /// <summary>
        /// A MemoryStream covering the package header bytes
        /// </summary>
        [ElementPriority(10)]
        public override Stream HeaderStream { get { throw new NotImplementedException(); } }
        #endregion

        #region Package index
        /// <summary>
        /// Package index: the index format in use
        /// </summary>
        [ElementPriority(11)]
        public override uint Indextype { get { return ((PackageIndex)GetResourceList).Indextype; } }

        /// <summary>
        /// Package index: the index
        /// </summary>
        [ElementPriority(12)]
        public override List<IResourceIndexEntry> GetResourceList { get { return Index; } }

        static bool FlagMatch(uint flags, IResourceIndexEntry values, IResourceIndexEntry target)
        {
            if (flags == 0) return true;

            for (int i = 0; i < sizeof(uint); i++)
            {
                uint j = (uint)(1 << i);
                if ((flags & j) == 0) continue;
                string f = values.ContentFields[i];
                if (!target.ContentFields.Contains(f)) return false;
                if (!values[f].Equals(target[f])) return false;
            }
            return true;
        }

        static bool NameMatch(string[] names, TypedValue[] values, IResourceIndexEntry target)
        {
            for (int i = 0; i < names.Length; i++) if (!target.ContentFields.Contains(names[i]) || !values[i].Equals(target[names[i]])) return false;
            return true;
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by <paramref name="flags"/> and <paramref name="values"/>,
        /// and returns the first occurrence within the package index./>.
        /// </summary>
        /// <param name="flags">True bits enable matching against numerically equivalent <paramref name="values"/> entry</param>
        /// <param name="values">Fields to compare against</param>
        /// <returns>The first match, if any; otherwise null.</returns>
        [Obsolete("Please use Find(Predicate<IResourceIndexEntry> Match)")]
        public override IResourceIndexEntry Find(uint flags, IResourceIndexEntry values) { return Index.Find(x => !x.IsDeleted && FlagMatch(flags, values, x)); }

        /// <summary>
        /// Searches for an element that matches the conditions defined by <paramref name="names"/> and <paramref name="values"/>,
        /// and returns the first occurrence within the package index./>.
        /// </summary>
        /// <param name="names">Names of fields to compare</param>
        /// <param name="values">Fields to compare against</param>
        /// <returns>The first match, if any; otherwise null.</returns>
        [Obsolete("Please use Find(Predicate<IResourceIndexEntry> Match)")]
        public override IResourceIndexEntry Find(string[] names, TypedValue[] values) { return Index.Find(x => !x.IsDeleted && NameMatch(names, values, x)); }

        /// <summary>
        /// Searches the entire <see cref="IPackage"/>
        /// for the first <see cref="IResourceIndexEntry"/> that matches the conditions defined by
        /// the <c>Predicate&lt;IResourceIndexEntry&gt;</c> <paramref name="Match"/>.
        /// </summary>
        /// <param name="Match"><c>Predicate&lt;IResourceIndexEntry&gt;</c> defining matching conditions.</param>
        /// <returns>The first matching <see cref="IResourceIndexEntry"/>, if any; otherwise null.</returns>
        /// <remarks>Note that entries marked as deleted will not be returned.</remarks>
        public override IResourceIndexEntry Find(Predicate<IResourceIndexEntry> Match) { return Index.Find(x => !x.IsDeleted && Match(x)); }

        /// <summary>
        /// Searches the entire <see cref="IPackage"/>
        /// for all <see cref="IResourceIndexEntry"/>s that matches the conditions defined by
        /// <paramref name="flags"/> and <paramref name="values"/>.
        /// </summary>
        /// <param name="flags">True bits enable matching against numerically equivalent <paramref name="values"/> entry.</param>
        /// <param name="values">Field values to compare against.</param>
        /// <returns>An <c>IList&lt;IResourceIndexEntry&gt;</c> of zero or more matches.</returns>
        [Obsolete("Please use FindAll(Predicate<IResourceIndexEntry> Match)")]
        public override List<IResourceIndexEntry> FindAll(uint flags, IResourceIndexEntry values) { return Index.FindAll(x => !x.IsDeleted && FlagMatch(flags, values, x)); }

        /// <summary>
        /// Searches the entire <see cref="IPackage"/>
        /// for all <see cref="IResourceIndexEntry"/>s that matches the conditions defined by
        /// <paramref name="names"/> and <paramref name="values"/>.
        /// </summary>
        /// <param name="names">Names of <see cref="IResourceIndexEntry"/> fields to compare.</param>
        /// <param name="values">Field values to compare against.</param>
        /// <returns>An <c>IList&lt;IResourceIndexEntry&gt;</c> of zero or more matches.</returns>
        [Obsolete("Please use FindAll(Predicate<IResourceIndexEntry> Match)")]
        public override List<IResourceIndexEntry> FindAll(string[] names, TypedValue[] values) { return Index.FindAll(x => !x.IsDeleted && NameMatch(names, values, x)); }

        /// <summary>
        /// Searches the entire <see cref="IPackage"/>
        /// for all <see cref="IResourceIndexEntry"/>s that matches the conditions defined by
        /// the <c>Predicate&lt;IResourceIndexEntry&gt;</c> <paramref name="Match"/>.
        /// </summary>
        /// <param name="Match"><c>Predicate&lt;IResourceIndexEntry&gt;</c> defining matching conditions.</param>
        /// <returns>Zero or more matches.</returns>
        /// <remarks>Note that entries marked as deleted will not be returned.</remarks>
        public override List<IResourceIndexEntry> FindAll(Predicate<IResourceIndexEntry> Match) { return Index.FindAll(x => !x.IsDeleted && Match(x)); }
        #endregion

        #region Package content
        /// <summary>
        /// Add a resource to the package
        /// </summary>
        /// <param name="rk">The resource key</param>
        /// <param name="stream">The stream that contains the resource data</param>
        /// <param name="rejectDups">If true, fail if the resource key already exists</param>
        /// <returns>Null if rejectDups and the resource key exists; else the new IResourceIndexEntry</returns>
        public override IResourceIndexEntry AddResource(IResourceKey rk, Stream stream, bool rejectDups)
        {
            if (rejectDups && Index[rk] != null && !Index[rk].IsDeleted) return null;
            IResourceIndexEntry newrc = Index.Add(rk);
            if (stream != null) ((ResourceIndexEntry)newrc).ResourceStream = stream;

            return newrc;
        }
        /// <summary>
        /// Tell the package to replace the data for the resource indexed by <paramref name="rc"/>
        /// with the data from the resource <paramref name="res"/>
        /// </summary>
        /// <param name="rc">Target resource index</param>
        /// <param name="res">Source resource</param>
        public override void ReplaceResource(IResourceIndexEntry rc, IResource res) { ((ResourceIndexEntry)rc).ResourceStream = res.Stream; }
        /// <summary>
        /// Tell the package to delete the resource indexed by <paramref name="rc"/>
        /// </summary>
        /// <param name="rc">Target resource index</param>
        public override void DeleteResource(IResourceIndexEntry rc)
        {
            if (!rc.IsDeleted)
                (rc as ResourceIndexEntry)?.Delete();
        }
        #endregion
        #endregion


        #region Package implementation
        Stream packageStream;

        private Package()
        {
            header = new byte[96];

            BinaryWriter bw = new BinaryWriter(new MemoryStream(header));
            bw.Write(stringToBytes(magic));
            setIndexsize(bw, new PackageIndex().Size);
            setIndexversion(bw);
            setIndexposition(bw, header.Length);
        }

        private Package(Stream s)
        {
            packageStream = s;
            s.Position = 0;
            header = new BinaryReader(s).ReadBytes(header.Length);
            if (checking) CheckHeader();
        }

        private byte[] packedChunk(ResourceIndexEntry ie)
        {
            byte[] chunk;
            if (ie.IsDirty)
            {
                Stream res = GetResource(ie);
                BinaryReader r = new BinaryReader(res);

                res.Position = 0;
                chunk = r.ReadBytes((int)ie.Memsize);
                if (checking) if (chunk.Length != (int)ie.Memsize)
                        throw new OverflowException(
                            $"packedChunk, dirty resource - T: 0x{ie.ResourceType:X}, G: 0x{ie.ResourceGroup:X}, I: 0x{ie.Instance:X}: Length expected: 0x{ie.Memsize:X}, read: 0x{chunk.Length:X}");

                byte[] comp = ie.Compressed != 0 ? Compression.CompressStream(chunk) : chunk;
                if (comp.Length < chunk.Length)
                    chunk = comp;
            }
            else
            {
                if (checking) if (packageStream == null)
                        throw new InvalidOperationException(
                            $"Clean resource with undefined \"current package\" - T: 0x{ie.ResourceType:X}, G: 0x{ie.ResourceGroup:X}, I: 0x{ie.Instance:X}");
                packageStream.Position = ie.Chunkoffset;
                chunk = (new BinaryReader(packageStream)).ReadBytes((int)ie.Filesize);
                if (checking) if (chunk.Length != (int)ie.Filesize)
                        throw new OverflowException(
                            $"packedChunk, clean resource - T: 0x{ie.ResourceType:X}, G: 0x{ie.ResourceGroup:X}, I: 0x{ie.Instance:X}: Length expected: 0x{ie.Filesize:X}, read: 0x{chunk.Length:X}");
            }
            return chunk;
        }
        #endregion

        #region Header implementation
        static byte[] stringToBytes(string s)
        {
            byte[] bytes = new byte[s.Length]; int i = 0;
            for (int index1 = 0; index1 < s.Length; index1++) bytes[i++] = (byte)s[index1];

            return bytes;
        }
        static string bytesToString(byte[] bytes)
        {
            string s = "";
            for (int i = 0; i < bytes.Length; i++) s += (char)bytes[i];

            return s;
        }

        const string magic = "DBPF";

        byte[] header = new byte[96];

        void setIndexcount(BinaryWriter w, int c) { w.BaseStream.Position = 36; w.Write(c); }
        void setIndexsize(BinaryWriter w, int c) { w.BaseStream.Position = 44; w.Write(c); }
        void setIndexversion(BinaryWriter w) { w.BaseStream.Position = 60; w.Write(3); }
        void setIndexposition(BinaryWriter w, int c) { w.BaseStream.Position = 40; w.Write(0); w.BaseStream.Position = 64; w.Write(c); }

        void CheckHeader()
        {
            if (header.Length != 96)
                throw new InvalidDataException("Hit unexpected end of file.");

            if (bytesToString(Magic) != magic)
                throw new InvalidDataException("Expected magic tag '" + magic + "'.  Found '" + bytesToString(Magic) + "'.");
        }
        #endregion

        #region Index implementation
        PackageIndex index;
        private PackageIndex Index
        {
            get
            {
                if (index == null)
                {
                    index = new PackageIndex(packageStream, Indexposition, Indexsize, Indexcount);
                    OnResourceIndexInvalidated(this, EventArgs.Empty);
                }
                return index;
            }
        }
        #endregion


        // Required by API, not user tools

        /// <summary>
        /// Required internally by s3pi - <b>not</b> for use in user tools.
        /// Please use <c>WrapperDealer.GetResource(int, IPackage, IResourceIndexEntry)</c> instead.
        /// </summary>
        /// <param name="rc">IResourceIndexEntry of resource</param>
        /// <returns>The resource data (uncompressed, if necessary)</returns>
        /// <remarks>Used by WrapperDealer to get the data for a resource.</remarks>
        public override Stream GetResource(IResourceIndexEntry rc)
        {
            if (rc is ResourceIndexEntry rie)
            {
                if (rie.ResourceStream != null) return rie.ResourceStream;

                if (rc.Chunkoffset == 0xffffffff) return null;
                packageStream.Position = rc.Chunkoffset;

                byte[] data;
                if (rc.Filesize == 1 && rc.Memsize == 0xFFFFFFFF) return null;//{ data = new byte[0]; }
                else if (rc.Filesize == rc.Memsize)
                {
                    data = new BinaryReader(packageStream).ReadBytes((int)rc.Filesize);
                }
                else
                {
                    data = Compression.UncompressStream(packageStream, (int)rc.Filesize, (int)rc.Memsize);
                }

                MemoryStream ms = new MemoryStream();
                ms.Write(data, 0, data.Length);
                ms.Position = 0;
                return ms;
            }

            return null;
        }
    }
}
