using System;

namespace Regul.S3PI.Interfaces
{
    /// <summary>
    /// Exposes a standard set of properties to identify a resource
    /// </summary>
    public interface IResourceKey : System.Collections.Generic.IEqualityComparer<IResourceKey>, IEquatable<IResourceKey>, IComparable<IResourceKey>
    {
        /// <summary>
        /// The "type" of the resource
        /// </summary>
        uint ResourceType { get; set; }
        /// <summary>
        /// The "group" the resource is part of
        /// </summary>
        uint ResourceGroup { get; set; }
        /// <summary>
        /// The "instance" number of the resource
        /// </summary>
        ulong Instance { get; set; }
    }
}
