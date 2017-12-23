using System;
using System.IO;

namespace RssFeed
{
    public static class Assets
    {
        public static string InstallDirectory
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory;
            }
        }

        public static string RssImagePath
        {
            get
            {
                return Path.Combine(InstallDirectory, "RSS.png");
            }
        }
    }
}
