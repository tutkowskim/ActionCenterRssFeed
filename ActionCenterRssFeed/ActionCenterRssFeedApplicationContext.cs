using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ActionCenterRssFeed
{
    public class ActionCenterRssFeedApplicationContext : ApplicationContext
    {
        private NotifyIcon trayIcon;
        private List<RssFeedReader> rssFeedReaders;

        public ActionCenterRssFeedApplicationContext()
        {
            // Hard code an RSS feed for now
            rssFeedReaders = new List<RssFeedReader>();
            rssFeedReaders.Add(new RssFeedReader("C:/Users/tutkowskim/Downloads/TestFeed.rss", 500));

            // Initialize Tray Icon
            trayIcon = new NotifyIcon()
            {
                Icon = new System.Drawing.Icon(Assets.RssIconPath),
                ContextMenu = new ContextMenu(new MenuItem[] {
                    new MenuItem("Exit", Exit)
                }),
                Visible = true
            };
        }

        private void Exit(object sender, EventArgs e)
        {
            // Hide tray icon, otherwise it will remain shown until user mouses over it
            trayIcon.Visible = false;

            Application.Exit();
        }
    }
}
