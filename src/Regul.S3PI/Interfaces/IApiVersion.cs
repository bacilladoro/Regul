using System;

namespace Regul.S3PI.Interfaces
{
    /// <summary>
    /// Support for API versioning
    /// </summary>
    public interface IApiVersion
    {
        /// <summary>
        /// The version of the API in use
        /// </summary>
        Int32 RequestedApiVersion { get; }

        /// <summary>
        /// The best supported version of the API available
        /// </summary>
        Int32 RecommendedApiVersion { get; }
    }
}