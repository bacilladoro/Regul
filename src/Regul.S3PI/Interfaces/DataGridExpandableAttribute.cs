using System;
using System.Collections.Generic;

namespace Regul.S3PI.Interfaces
{
    /// <summary>
    /// Indicates that the element should be expandable rather than requiring a popup
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
    [Obsolete]
    public class DataGridExpandableAttribute : Attribute
    {
        bool dataGridExpandable;
        /// <summary>
        /// Indicates that the element should be expandable rather than requiring a popup
        /// </summary>
        public DataGridExpandableAttribute() { dataGridExpandable = true; }
        /// <summary>
        /// If true, indicates that the element should be expandable rather than requiring a popup
        /// </summary>
        /// <param name="value">True to indicate the element should be expandable</param>
        public DataGridExpandableAttribute(bool value) { dataGridExpandable = value; }
        /// <summary>
        /// Indicate whether the element should be expandable (true) or not (false)
        /// </summary>
        public bool DataGridExpandable { get { return dataGridExpandable; } set { dataGridExpandable = value; } }

        /// <summary>
        /// Return the DataGridExpandable value for a Content Field.
        /// </summary>
        /// <param name="t">Type on which Content Field exists.</param>
        /// <param name="index">Content Field name.</param>
        /// <returns>The value of the DataGridExpandableAttribute DataGridExpandable field, if found;
        /// otherwise <c>false</c>.</returns>
        public static bool GetDataGridExpandable(Type t, string index)
        {
            System.Reflection.PropertyInfo pi = t.GetProperty(index);

            if (pi != null)
                foreach (var attr in pi.GetCustomAttributes(typeof(DataGridExpandableAttribute), true))
                    return (attr as DataGridExpandableAttribute).DataGridExpandable;

            return false;
        }
    }
}
