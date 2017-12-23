﻿using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Xml.Serialization;
using System.Windows;

namespace RssFeed
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private System.Windows.Forms.NotifyIcon _trayIcon;
        private ObservableCollection<RssFeedReader> _rssFeedReaders;

        protected override void OnStartup(StartupEventArgs e)
        {
            // Call the base startup code
            base.OnStartup(e);

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

        private void EditRssFeeds(object sender, EventArgs e)
        {
            RssFeedConfigWindow rssFeedConfigurationWindow = new RssFeedConfigWindow(_rssFeedReaders);
            rssFeedConfigurationWindow.ShowDialog();
        }

        private void Close(object sender, EventArgs e)
        {
            // Hide tray icon, otherwise it will remain shown until user mouses over it
            _trayIcon.Visible = false;

            Shutdown();
        }

        protected override  void OnExit(ExitEventArgs e)
        {
            // Save the configuration before closeing
            SaveRssFeedConfiguration(Assets.ConfigurationFile, _rssFeedReaders);

            // Call the base impl
            base.OnExit(e);
        }
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
            catch
            {
                // TODO: Log something
            }

        }

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
            catch
            {
                // TODO: Log something
                return new ObservableCollection<RssFeedReader>();
            }
        }
    }
}
