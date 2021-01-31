﻿using System;
using System.Collections.Generic;

namespace Regul.S3PI.Interfaces
{
    /// <summary>
    /// An abstract class, descended from <see cref="AResourceKey"/>, providing an abstract implemention of <see cref="IResourceIndexEntry"/>,
    /// representing an index entry within a package.
    /// </summary>
    public abstract class AResourceIndexEntry : AResourceKey, IResourceIndexEntry
    {
        /// <summary>
        /// Initialize a new instance with the default API version and no change <see cref="EventHandler"/>.
        /// </summary>
        public AResourceIndexEntry() : base(null) { handler += OnResourceIndexEntryChanged; }

        private void OnResourceIndexEntryChanged(object sender, EventArgs e)
        {
            ResourceIndexEntryChanged?.Invoke(sender, e);
        }

        #region AApiVersionedFields
        /// <summary>
        /// The list of available field names on this API object
        /// </summary>
        public override List<string> ContentFields { get { return GetContentFields(GetType()); } }
        #endregion

        #region IResourceIndexEntry Members
        /// <summary>
        /// If the resource was read from a package, the location in the package the resource was read from
        /// </summary>
        [ElementPriority(5)]
        public abstract uint Chunkoffset { get; set; }
        /// <summary>
        /// The number of bytes the resource uses within the package
        /// </summary>
        [ElementPriority(6)]
        public abstract uint Filesize { get; set; }
        /// <summary>
        /// The number of bytes the resource uses in memory
        /// </summary>
        [ElementPriority(7)]
        public abstract uint Memsize { get; set; }
        /// <summary>
        /// 0xFFFF if Filesize != Memsize, else 0x0000
        /// </summary>
        [ElementPriority(8)]
        public abstract ushort Compressed { get; set; }
        /// <summary>
        /// Always 0x0001
        /// </summary>
        [ElementPriority(9)]
        public abstract ushort Unknown2 { get; set; }

        /// <summary>
        /// A MemoryStream covering the index entry bytes
        /// </summary>
        public abstract System.IO.Stream Stream { get; }

        /// <summary>
        /// True if the index entry has been deleted from the package index
        /// </summary>
        public abstract bool IsDeleted { get; set; }

        /// <summary>
        /// Raised when the AResourceIndexEntry changes
        /// </summary>
        public EventHandler ResourceIndexEntryChanged;
        #endregion

        #region IEquatable<IResourceIndexEntry>
        /// <summary>
        /// Indicates whether the current <see cref="IResourceIndexEntry"/> instance is equal to another <see cref="IResourceIndexEntry"/> instance.
        /// </summary>
        /// <param name="other">An <see cref="IResourceIndexEntry"/> instance to compare with this instance.</param>
        /// <returns>true if the current instance is equal to the <paramref name="other"/> parameter; otherwise, false.</returns>
        public abstract bool Equals(IResourceIndexEntry other);

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to the current <see cref="AResourceIndexEntry"/>.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with the current <see cref="AResourceIndexEntry"/>.</param>
        /// <returns>true if the specified <see cref="object"/> is equal to the current <see cref="AResourceIndexEntry"/>; otherwise, false.</returns>
        public override bool Equals(object obj) => (AResourceIndexEntry)obj != null && Equals(obj as AResourceIndexEntry);

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override abstract int GetHashCode();
        #endregion
    }
}
