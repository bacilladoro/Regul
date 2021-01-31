using Regul.S3PI.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using static System.String;

namespace Regul.S3PI
{
    /// <summary>
    /// Responsible for associating ResourceType in the IResourceIndexEntry with a particular class (a "wrapper") that understands it
    /// or the default wrapper.
    /// </summary>
    public static class WrapperDealer
    {
        /// <summary>
        /// Create a new Resource of the requested type, allowing the wrapper to initialise it appropriately
        /// </summary>
        /// <param name="resourceType">Type of resource (currently a string like "0xDEADBEEF")</param>
        /// <returns></returns>
        public static IResource CreateNewResource(string resourceType) => WrapperForType(resourceType, null);


        /// <summary>
        /// Retrieve a resource from a package, readying the appropriate wrapper
        /// </summary>
        /// <param name="pkg">Package containing <paramref name="rie"/></param>
        /// <param name="rie">Identifies resource to be returned</param>
        /// <returns>A resource from the package</returns>
        public static IResource GetResource(IPackage pkg, IResourceIndexEntry rie) { return GetResource(pkg, rie, false); }


        /// <summary>
        /// Retrieve a resource from a package, readying the appropriate wrapper or the default wrapper
        /// </summary>
        /// <param name="pkg">Package containing <paramref name="rie"/></param>
        /// <param name="rie">Identifies resource to be returned</param>
        /// <param name="AlwaysDefault">When true, indicates WrapperDealer should always use the DefaultResource wrapper</param>
        /// <returns>A resource from the package</returns>
        public static IResource GetResource(IPackage pkg, IResourceIndexEntry rie, bool AlwaysDefault)
        {
            typeMap = new List<KeyValuePair<string, Type>>();
            try
            {
                Type[] ts = Assembly.LoadFrom(Path.GetDirectoryName(typeof(WrapperDealer).Assembly.Location) + "/Regul.S3PI.dll").GetTypes();
                for (int index = 0; index < ts.Length; index++)
                {
                    Type t = ts[index];
                    if (!t.IsSubclassOf(typeof(AResourceHandler))) continue;

                    AResourceHandler arh =
                        (AResourceHandler)t.GetConstructor(new Type[0] { })?.Invoke(Array.Empty<object>());

                    if (arh == null) continue;

                    foreach (Type k in arh.Keys)
                    {
                        for (int i = 0; i < arh[k].Count; i++)
                        {
                            string s = arh[k][i];
                            typeMap.Add(new KeyValuePair<string, Type>(s, k));
                        }
                    }
                }
            }
            catch { }

            typeMap.Sort((x, y) => Compare(x.Key, y.Key, StringComparison.Ordinal));

            return WrapperForType(AlwaysDefault ? "*" : rie["ResourceType"], (pkg as APackage)?.GetResource(rie));
        }

        /// <summary>
        /// Retrieve the resource wrappers known to WrapperDealer.
        /// </summary>
        public static ICollection<KeyValuePair<string, Type>> TypeMap { get { return new List<KeyValuePair<string, Type>>(typeMap); } }

        /// <summary>
        /// Access the collection of wrappers on the &quot;disabled&quot; list.
        /// </summary>
        /// <remarks>Updates to entries in the collection will be used next time a wrapper is requested.
        /// Existing instances of a disabled wrapper will not be invalidated and it will remain possible to
        /// bypass WrapperDealer and instantiate instances of the wrapper class directly.</remarks>
        public static ICollection<KeyValuePair<string, Type>> Disabled { get { return disabled; } }

        #region Implementation
        static List<KeyValuePair<string, Type>> typeMap = null;

        static readonly List<KeyValuePair<string, Type>> disabled = new();

        static IResource WrapperForType(string type, Stream s)
        {
            Type t = typeMap.Find(x => !disabled.Contains(x) && x.Key == type).Value;

            if (t == null)
                t = typeMap.Find(x => !disabled.Contains(x) && x.Key == "*").Value;

            if (Settings.Settings.Checking && t == null)
                throw new InvalidOperationException("Could not find a resource handler");

            return (IResource)t.GetConstructor(new[] { typeof(Stream) })?.Invoke(new object[] { s });
        }
        #endregion
    }
}
