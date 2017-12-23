using System;
using System.IO;

namespace RssFeed
{
    /// <summary>
    /// Static class that resolves the path to file assets.
    /// </summary>
    public static class Assets
    {
        /// <summary>
        /// The install directory of this application.
        /// </summary>
        public static string InstallDirectory
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory;
            }
        }

        /// <summary>
        /// The RSS logo for this application.
        /// </summary>
        public static string RssImagePath
        {
            get
            {
                return Path.Combine(InstallDirectory, "RSS.png");
            }
        }

        /// <summary>
        /// The user specific configuration file for this application.
        /// </summary>
        public static string ConfigurationFile
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RssFeed", "RssFeeds.xml");
            }
        }
    }
}
