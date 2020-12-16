using System;
using System.Collections.Generic;

namespace Regul.S3PI.Interfaces
{
    /// <summary>
    /// Base class for versioning support.  Not directly used by the API.
    /// </summary>
    public class VersionAttribute : Attribute
    {
        int version;
        /// <summary>
        /// Version number attribute (base)
        /// </summary>
        /// <param name="Version">Version number</param>
        public VersionAttribute(int Version) { version = Version; }
        /// <summary>
        /// Version number
        /// </summary>
        public int Version { get { return version; } set { version = value; } }
    }

    /// <summary>
    /// Specify the Minumum version from which a field or method is supported
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false,  Inherited = true)]
    public class MinimumVersionAttribute : VersionAttribute
    {
        /// <summary>
        /// Specify the Minumum version from which a field or method is supported
        /// </summary>
        /// <param name="Version">Version number</param>
        public MinimumVersionAttribute(int Version) : base(Version) { }
    }

    /// <summary>
    /// Specify the Maximum version up to which a field or method is supported
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class MaximumVersionAttribute : VersionAttribute
    {
        /// <summary>
        /// Specify the Maximum version up to which a field or method is supported
        /// </summary>
        /// <param name="Version">Version number</param>
        public MaximumVersionAttribute(int Version) : base(Version) { }
    }
}
