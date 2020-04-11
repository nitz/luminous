namespace System.IO
{
    using System;
    using System.IO;
    using System.Threading;

    /// <summary>Extension methods for the FileInfo class.</summary>
    public static class FileInfoExtensions
    {
        public static FileStream TryOpen(this FileInfo fileInfo, FileAccess access = FileAccess.ReadWrite, FileShare share = FileShare.None)
        {
            if (fileInfo == null)
            {
                throw new ArgumentNullException(nameof(fileInfo), "Contract assertion not met: fileInfo != null");
            }

            return fileInfo.Open(FileMode.Open, access, share);
        }

        public static FileStream WaitAndOpen(this FileInfo fileInfo, TimeSpan timeout, FileAccess access = FileAccess.ReadWrite, FileShare share = FileShare.None)
        {
            if (fileInfo == null)
            {
                throw new ArgumentNullException(nameof(fileInfo), "Contract assertion not met: fileInfo != null");
            }

            DateTime dt = DateTime.UtcNow;
            FileStream fs = null;
            while ((fs = TryOpen(fileInfo, access, share)) == null && (DateTime.UtcNow - dt) < timeout)
            {
                Thread.Sleep(250); // who knows better way and wants a free cookie? ;)
            }
            return fs;
        }
    }
}
