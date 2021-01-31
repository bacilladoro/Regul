namespace Regul.S3PI.Settings
{
    /// <summary>
    /// Holds global settings, currently statically defined
    /// </summary>
    public static class Settings
    {
        static Settings()
        {
            // initialisation code, like read from settings file...
        }

        static readonly bool checking = true;
        /// <summary>
        /// When true, run extra checks as part of normal operation.
        /// </summary>
        public static bool Checking { get { return checking; } }

        static readonly bool asBytesWorkaround = true;
        /// <summary>
        /// When true, assume data is dirty regardless of tracking.
        /// </summary>
        public static bool AsBytesWorkaround { get { return asBytesWorkaround; } }
    }
}
