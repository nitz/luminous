namespace System.Collections.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>Extension methods for the Dictionary class.</summary>
    public static class DictionaryExtensions
    {
        public static TValue TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary), "Contract assertion not met: dictionary != null");
            }

            TValue value;
            if (dictionary.TryGetValue(key, out value)) return value;
            return defaultValue;
        }
        public static TValue TryGetNotNullValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary), "Contract assertion not met: dictionary != null");
            }
            if (defaultValue == null)
            {
                throw new ArgumentNullException(nameof(defaultValue), "Contract assertion not met: defaultValue != null");
            }

            TValue value;
            if (dictionary.TryGetValue(key, out value) && value != null) return value;
            return defaultValue;
        }
        public static TValue TryGetNotEmptyValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary), "Contract assertion not met: dictionary != null");
            }
            if (defaultValue == null)
            {
                throw new ArgumentNullException(nameof(defaultValue), "Contract assertion not met: defaultValue != null");
            }
            if (string.IsNullOrEmpty(defaultValue.ToString()))
            {
                throw new ArgumentNullException(nameof(defaultValue), "Contract assertion not met: !string.IsNullOrEmpty(defaultValue.ToString())");
            }

            TValue value;
            if (dictionary.TryGetValue(key, out value) && value != null && value.ToString().Length > 0) return value;
            return defaultValue;
        }
    }
}
