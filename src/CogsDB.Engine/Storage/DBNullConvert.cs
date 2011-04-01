using System;

namespace CogsDB.Engine.Storage
{
    /// <summary>
    /// Utility class to convert DBNull objects to appropriate datavalues
    /// </summary>
    public static class DBNullConvert
    {
        /// <summary>
        /// Checks an object for a DBNull.Value and returns a default value if found, or casts the object appropriately.
        /// </summary>
        /// <typeparam name="T">The type to convert the object to.</typeparam>
        /// <param name="value">The object to check for DBNull.</param>
        /// <param name="defaultValue">The default value to use in place of DBNull.</param>
        /// <returns>Returns the default value, or the value of the casted object.</returns>
        public static T To<T>(object value, T defaultValue)
        {
            T cast;

            try
            {
                cast = value == DBNull.Value ? defaultValue : (T)value;
            }
            catch
            {
                throw new ArgumentException(string.Format("Argument of type {0} cannot be cast to type {1}", value.GetType(), typeof(T)), "value");
            }

            return cast;
        }

        /// <summary>
        /// Toes the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static T To<T>(object value)
        {
            return To(value, default(T));
        }

        /// <summary>
        /// Converts an object to a Nullable type
        /// </summary>
        /// <typeparam name="T">The type to cast the value to.  Must be a struct type.</typeparam>
        /// <param name="value">The object to test for DBNull.</param>
        /// <returns>Null if object is DBNull, or the cast value of the object. </returns>
        public static T? ToNullable<T>(object value) where T : struct
        {
            T? cast;

            try
            {
                cast = value == DBNull.Value ? null : (T?)value;
            }
            catch
            {
                throw new ArgumentException(string.Format("Argument of type {0} cannot be cast to type {1}", value.GetType(), typeof(T?)), "value");
            }

            return cast;
        }

        /// <summary>
        /// Returns an empty string if the object passed in is DBNull, otherwise the value is cast to a string and returned.
        /// </summary>
        /// <param name="value">The object to check for DBNull.</param>
        /// <returns>The string representation of value, or an empty string if DBNull.</returns>
        public static string ToString(object value)
        {
            return To(value, String.Empty);
        }
    }
}
