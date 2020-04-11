namespace System.IO
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.IO;

    /// <summary>Extension methods for the Stream class.</summary>
    public static class StreamExtensions
    {
        /// <summary>
        /// Returns stream’s contents as an array of bytes.
        /// </summary>
        public static byte[] ReadToEnd(this Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream), "Contract assertion not met: stream != null");
            }

            long? originalPosition = null;
            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    return ms.ToArray();
                }
            }
            finally
            {
                if (stream.CanSeek && originalPosition.HasValue && originalPosition.Value >= 0)
                {
                    stream.Position = originalPosition.Value;
                }
            }
        }
    }
}
