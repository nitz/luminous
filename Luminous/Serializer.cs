namespace Luminous
{
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    /// <summary>
    /// Provides methods for object serialization/deserialization.
    /// </summary>
    public static class Serializer
    {
        /// <summary>
        /// Deserializes an object from the array of bytes.
        /// </summary>
        public static T Deserialize<T>(byte[] array)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array), "Contract assertion not met: array != null");
            }
            if (!(array.Length > 0))
            {
                throw new ArgumentException("Contract assertion not met: array.Length > 0", nameof(array));
            }

            using (MemoryStream ms = new MemoryStream(array))
            {
                if (!(ms.CanSeek))
                {
                    throw new ArgumentException("Contract assertion not met: ms.CanSeek", "value");
                }
                if (!(ms.Length > 0))
                {
                    throw new ArgumentException("Contract assertion not met: ms.Length > 0", "value");
                }

                BinaryFormatter formatter = new BinaryFormatter();
                return (T)formatter.Deserialize(ms);
            }
        }

        /// <summary>
        /// Serializes the object to the array of bytes.
        /// </summary>
        public static byte[] Serialize<T>(T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "Contract assertion not met: obj != null");
            }

            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, obj);
                byte[] bytes = ms.ToArray();
                if (!(bytes.Length > 0))
                {
                    throw new ArgumentException("Contract assertion not met: bytes.Length > 0", "value");
                }
                return bytes;
            }
        }

        /// <summary>
        /// Returns a deep copy of the object using serialization.
        /// </summary>
        public static T CopyBySerialization<T>(this T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "Contract assertion not met: obj != null");
            }

            return Deserialize<T>(Serialize<T>(obj));
        }
    }
}
