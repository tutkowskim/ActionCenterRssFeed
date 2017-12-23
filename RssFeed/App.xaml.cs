using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Serialization;
using System.Windows;

namespace RssFeed
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly static log4net.ILog logger = log4net.LogManager.GetLogger("tutkowski.rssfeed.app");
        private System.Windows.Forms.NotifyIcon _trayIcon;
        private ObservableCollection<RssFeedReader> _rssFeedReaders;

        /// <summary>
        /// Startup and initialize the application.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            // Call the base startup code
            base.OnStartup(e);

            // Configure log4net
            log4net.Config.XmlConfigurator.Configure();

            // Load rss feed configuration from disk and save it when it changes
            _rssFeedReaders = LoadRssFeedConfiguration(Assets.ConfigurationFile);

            // Initialize Tray Icon
            System.Drawing.Icon icon;
            using (Stream iconStream = GetResourceStream(new Uri("pack://application:,,,/RssFeed;component/RSS.ico")).Stream)
            {
                icon = new System.Drawing.Icon(iconStream);
            }

            _trayIcon = new System.Windows.Forms.NotifyIcon()
            {
                Icon = icon,
                ContextMenu = new System.Windows.Forms.ContextMenu(new System.Windows.Forms.MenuItem[] {
                    new System.Windows.Forms.MenuItem("Edit RSS Feeds", EditRssFeeds),
                    new System.Windows.Forms.MenuItem("Exit", Close)
                }),
                Visible = true
            };
        }

        /// <summary>
        /// Event handler to edit the RSS Feeds.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditRssFeeds(object sender, EventArgs e)
        {
            RssFeedConfigWindow rssFeedConfigurationWindow = new RssFeedConfigWindow(_rssFeedReaders);
            rssFeedConfigurationWindow.ShowDialog();
        }

        /// <summary>
        /// Event handler to close the application.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close(object sender, EventArgs e)
        {
            // Hide tray icon, otherwise it will remain shown until user mouses over it
            _trayIcon.Visible = false;

            Shutdown();
        }

        /// <summary>
        /// Override OnExit to save the configuration before closing the application
        /// </summary>
        /// <param name="e"></param>
        protected override void OnExit(ExitEventArgs e)
        {
            // Save the configuration before closeing
            SaveRssFeedConfiguration(Assets.ConfigurationFile, _rssFeedReaders);

            // Call the base impl
            base.OnExit(e);
        }

        /// <summary>
        /// Helper method to save the configuration
        /// </summary>
        /// <param name="path">The file to save the configuration to.</param>
        /// <param name="rssFeedReaders">The rss feed readers to save</param>
        private static void SaveRssFeedConfiguration(string path, ObservableCollection<RssFeedReader> rssFeedReaders)
        {
            // Create the directory if it doesn't exist
            try
            {
                Directory.GetParent(path).Create();

                // Save out the config file
                var serializer = new XmlSerializer(typeof(ObservableCollection<RssFeedReader>));
                using (var stream = File.Open(path, FileMode.Create, FileAccess.Write))
                {
                    serializer.Serialize(stream, rssFeedReaders);
                }
            }
            catch (Exception e)
            {
                logger.Error(string.Format("Failed to save the configuration to {0} due to the exception {1}", path, e.Message));
            }
        }

        /// <summary>
        /// Helper method to save the configuration
        /// </summary>
        /// <param name="path">The file to load the configuration from.</param>
        /// <returns>The rss feed readers</returns>
        private static ObservableCollection<RssFeedReader> LoadRssFeedConfiguration(string path)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(ObservableCollection<RssFeedReader>));
                using (var stream = File.OpenRead(path))
                {
                    return (ObservableCollection<RssFeedReader>)(serializer.Deserialize(stream));
                }
            }
            catch (Exception e)
            {
                logger.Error(string.Format("Failed to load the configuration from {0} due to the exception {1}", path, e.Message));
                return new ObservableCollection<RssFeedReader>();
            }
        }
    }
}
