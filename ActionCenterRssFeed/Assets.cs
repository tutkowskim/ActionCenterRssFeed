using System;
using System.IO;

namespace ActionCenterRssFeed
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

        public static string RssIconPath
        {
            get
            {
                return Path.Combine(InstallDirectory, "RSS.ico");
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
