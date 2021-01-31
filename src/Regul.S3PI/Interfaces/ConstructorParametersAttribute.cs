using System;

namespace Regul.S3PI.Interfaces
{
    /// <summary>
    /// Specify the constructor parameters for a descendant of an abstract class
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    [Obsolete("Use DependentList<T>.Add(T instance) or DependendList<T>.Add(Type concrete-of-T).")]
    public class ConstructorParametersAttribute : Attribute
    {
        /// <summary>
        /// The constructor parameters
        /// </summary>
        public readonly object[] parameters;
        /// <summary>
        /// Specify the constructor parameters for a descendant of an abstract class
        /// </summary>
        /// <param name="parameters">The constructor parameters</param>
        public ConstructorParametersAttribute(object[] parameters) { this.parameters = parameters;}
    }
}
