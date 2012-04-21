using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace NuvoControl.Common
{

    /// <summary>
    /// This class contains extension methods for List (=IEnumerable) classes.
    /// Extensions for List (=IEnumerable) classes. The ToString() method is overwritten, to return a full string of the list
    /// and not jsut the class name (as done in teh default implementation).
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// ToString method for generic interface IEnumerable (from System.Collections.Generic)
        /// copied from: http://www.codemeit.com/linq/c-array-delimited-tostring.html 
        /// </summary>
        /// <typeparam name="T">Type used for the underlaing list</typeparam>
        /// <param name="source">Instance of the underlaing list</param>
        /// <param name="separator">Straing used as separator between the list entries</param>
        /// <returns>String representing the list entries.</returns>
        public static string ToString<T>(this IEnumerable<T> source, string separator)
        {
            if (source == null)
                throw new ArgumentException("Parameter source can not be null.");

            if (string.IsNullOrEmpty(separator))
                throw new ArgumentException("Parameter separator can not be null or empty.");

            string[] array = source.Where(n => n != null).Select(n => n.ToString()).ToArray();

            return string.Join(separator, array);
        }

        /// <summary>
        /// ToString method for generic interface IEnumerable (from System.Collections)
        /// copied from: http://www.codemeit.com/linq/c-array-delimited-tostring.html 
        /// </summary>
        /// <param name="source">Instance of the underlaing list</param>
        /// <param name="separator">Straing used as separator between the list entries</param>
        /// <returns>String representing the list entries.</returns>
        public static string ToString(this IEnumerable source, string separator)
        {
            if (source == null)
                throw new ArgumentException("Parameter source can not be null.");

            if (string.IsNullOrEmpty(separator))
                throw new ArgumentException("Parameter separator can not be null or empty.");

            string[] array = source.Cast<object>().Where(n => n != null).Select(n => n.ToString()).ToArray();

            return string.Join(separator, array);
        }

    }
}
