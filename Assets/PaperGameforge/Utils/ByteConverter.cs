using System;

namespace Assets.PaperGameforge.Utils
{
    public static class ByteConverter
    {
        private static readonly string[] SizeSuffixes = { "Bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        public static double ToKilobytes(long bytes) => bytes / 1024.0;
        public static double ToMegabytes(long bytes) => bytes / (1024.0 * 1024.0);
        public static double ToGigabytes(long bytes) => bytes / (1024.0 * 1024.0 * 1024.0);
        public static double ToTerabytes(long bytes) => bytes / (1024.0 * 1024.0 * 1024.0 * 1024.0);

        public static (double size, string unit) GetReadableSize(long bytes)
        {
            if (bytes < 0) { return GetReadableSize(-bytes); }
            if (bytes == 0) { return (0, "Bytes"); }

            int magnitude = (int)Math.Log(bytes, 1024);
            double adjustedSize = bytes / Math.Pow(1024, magnitude);

            return (adjustedSize, SizeSuffixes[magnitude]);
        }
    }
}
