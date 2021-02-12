using System;
using System.Collections.Generic;
using System.IO;

namespace Regul.S3PI.Interfaces
{
    /// <summary>
    /// A resource contained in a package.
    /// </summary>
    public abstract class AResource : AApiVersionedFields, IResource
    {
        #region Attributes
        /// <summary>
        /// Resource data <see cref="System.IO.Stream"/>
        /// </summary>
        protected Stream stream;

        /// <summary>
        /// Indicates the resource stream may no longer reflect the resource content
        /// </summary>
        protected bool dirty;
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new instance of the resource
        /// </summary>
        /// <param name="s"><see cref="System.IO.Stream"/> to use, or null to create from scratch.</param>
        protected AResource(Stream s)
        {
            stream = s;
        }
        #endregion

        #region AApiVersionedFields
        /// <summary>
        /// A <see cref="List{String}"/> of available field names on object
        /// </summary>
        public override List<string> ContentFields { get { return GetContentFields(GetType()); } }
        #endregion

        #region IResource Members
        /// <summary>
        /// The resource content as a <see cref="System.IO.Stream"/>.
        /// </summary>
        public virtual Stream Stream
        {
            get
            {
                if (dirty || Settings.Settings.AsBytesWorkaround)
                {
                    stream = UnParse();
                    dirty = false;
                    //Console.WriteLine(this.GetType().Name + " flushed.");
                }
                stream.Position = 0;
                return stream;
            }
        }
        /// <summary>
        /// The resource content as a <see cref="byte"/> array
        /// </summary>
        public virtual byte[] AsBytes
        {
            get
            {
                MemoryStream s = Stream as MemoryStream;
                if (s != null) return s.ToArray();

                stream.Position = 0;
                return (new BinaryReader(stream)).ReadBytes((int)stream.Length);
            }
        }

        /// <summary>
        /// Raised if the resource is changed
        /// </summary>
        public event EventHandler ResourceChanged;

        #endregion

        /// <summary>
        /// AResource classes must supply an <see cref="UnParse()"/> method that serializes the class to a <see cref="System.IO.Stream"/> that is returned.
        /// </summary>
        /// <returns><see cref="System.IO.Stream"/> containing serialized class data.</returns>
        protected abstract Stream UnParse();

        /// <summary>
        /// AResource classes must use this to indicate the resource has changed.
        /// </summary>
        /// <param name="sender">The resource (or sub-class) that has changed.</param>
        /// <param name="e">(Empty) event data object.</param>
        protected virtual void OnResourceChanged(object sender, EventArgs e)
        {
            dirty = true;
            //Console.WriteLine(this.GetType().Name + " dirtied.");
            ResourceChanged?.Invoke(sender, e);
        }
    }
}
