using System;
using System.IO;
using System.Collections.Generic;
using System.Windows;

namespace RssFeed
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private System.Windows.Forms.NotifyIcon trayIcon;
        private List<RssFeedReader> rssFeedReaders;

        protected override void OnStartup(StartupEventArgs e)
        {
            // Call the base startup code
            base.OnStartup(e);

            // Hard code an RSS feed for now
            rssFeedReaders = new List<RssFeedReader>();
            rssFeedReaders.Add(new RssFeedReader("C:/Users/tutkowskim/Downloads/TestFeed.rss", 500));

            // Initialize Tray Icon
            System.Drawing.Icon icon;
            using (Stream iconStream = GetResourceStream(new Uri("pack://application:,,,/RssFeed;component/RSS.ico")).Stream)
            {
                icon = new System.Drawing.Icon(iconStream);
            }

            trayIcon = new System.Windows.Forms.NotifyIcon()
            {
                Icon = icon,
                ContextMenu = new System.Windows.Forms.ContextMenu(new System.Windows.Forms.MenuItem[] {
                    new System.Windows.Forms.MenuItem("Exit", Close)
                }),
                Visible = true
            };
        }

        private void Close(object sender, EventArgs e)
        {
            // Hide tray icon, otherwise it will remain shown until user mouses over it
            trayIcon.Visible = false;

            Shutdown();
        }
    }
}
