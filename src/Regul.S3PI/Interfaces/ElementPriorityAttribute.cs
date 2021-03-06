using System;

namespace Regul.S3PI.Interfaces
{
    /// <summary>
    /// Element priority is used when displaying elements
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
    public class ElementPriorityAttribute : Attribute
    {
        int priority;
        /// <summary>
        /// Element priority is used when displaying elements
        /// </summary>
        /// <param name="priority">Element priority, lower values are higher priority</param>
        public ElementPriorityAttribute(int priority) { this.priority = priority; }
        /// <summary>
        /// Element priority, lower values are higher priority
        /// </summary>
        public int Priority { get { return priority; } set { priority = value; } }

        /// <summary>
        /// Return the ElementPriority value for a Content Field.
        /// </summary>
        /// <param name="t">Type on which Content Field exists.</param>
        /// <param name="index">Content Field name.</param>
        /// <returns>The value of the ElementPriorityAttribute Priority field, if found;
        /// otherwise Int32.MaxValue.</returns>
        public static int GetPriority(Type t, string index)
        {
            System.Reflection.PropertyInfo pi = t.GetProperty(index);

            if (pi != null)
                foreach (var attr in pi.GetCustomAttributes(typeof(ElementPriorityAttribute), true))
                    return (attr as ElementPriorityAttribute).Priority;

            return int.MaxValue;
        }
    }
}
