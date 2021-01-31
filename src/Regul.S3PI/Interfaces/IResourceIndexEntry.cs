using System;
using System.IO;

namespace Regul.S3PI.Interfaces
{
    /// <summary>
    /// An index entry within a package
    /// </summary>
    public interface IResourceIndexEntry : IContentFields, IResourceKey, IEquatable<IResourceIndexEntry>
    {
        /// <summary>
        /// If the resource was read from a package, the location in the package the resource was read from
        /// </summary>
        uint Chunkoffset { get; set; }
        /// <summary>
        /// The number of bytes the resource uses within the package
        /// </summary>
        uint Filesize { get; set; }
        /// <summary>
        /// The number of bytes the resource uses in memory
        /// </summary>
        uint Memsize { get; set; }
        /// <summary>
        /// 0xFFFF if Filesize != Memsize, else 0x0000
        /// </summary>
        ushort Compressed { get; set; }
        /// <summary>
        /// Always 0x0001
        /// </summary>
        ushort Unknown2 { get; set; }

        /// <summary>
        /// A <see cref="MemoryStream"/> covering the index entry bytes
        /// </summary>
        Stream Stream { get; }

        /// <summary>
        /// True if the index entry has been deleted from the package index
        /// </summary>
        bool IsDeleted { get; set; }
    }
}
