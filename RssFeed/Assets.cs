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

        public static string ConfigurationFile
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RssFeed", "RssFeeds.xml");
            }
        }
    }
}
