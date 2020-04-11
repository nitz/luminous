namespace System
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Runtime.Serialization.Formatters.Binary;

    /// <summary>Extension methods for the Object class.</summary>
    public static class ObjectExtensions
    {
        #region Conversion

        /// <summary>Checks whether the object can be converted to the provided type.</summary>
        public static bool CanConvert<T>(this object @this, IFormatProvider formatProvider = null)
        {
            return CanConvert(@this, typeof(T), formatProvider);
        }

        /// <summary>Checks whether the object can be converted to the provided type.</summary>
        public static bool CanConvert(this object @this, Type conversionType, IFormatProvider formatProvider = null)
        {
            if (conversionType == null)
            {
                throw new ArgumentNullException(nameof(conversionType), "Contract assertion not met: conversionType != null");
            }

            try
            {
                System.Convert.ChangeType(@this, conversionType, formatProvider);
                return true;
            }
            catch (InvalidCastException) { }
            catch (FormatException) { }
            catch (OverflowException) { }
            catch (ArgumentNullException) { }
            return false;
        }

        /// <summary>Converts the object to the provided type.</summary>
        public static T Convert<T>(this object @this, IFormatProvider formatProvider = null)
        {
            if ((@this == null && typeof(T).IsValueType))
            {
                throw new InvalidCastException(nameof(@this), "Contract assertion not met: !(@this == null && typeof(T).IsValueType)");
            }

            var type = typeof(T);
            var result = Convert(@this, type, formatProvider);
            if (result == null)
            {
                if (type.IsValueType) throw new InvalidCastException();
                return default(T); // null
            }
            return (T)result;
        }

        /// <summary>Converts the object to the provided type.</summary>
        public static object Convert(this object @this, Type conversionType, IFormatProvider formatProvider = null)
        {
            if (conversionType == null)
            {
                throw new ArgumentNullException(nameof(conversionType), "Contract assertion not met: conversionType != null");
            }

            return System.Convert.ChangeType(@this, conversionType, formatProvider);
        }

        #endregion
    }
}
