using System;
using System.Collections.Generic;
using System.Timers;
using System.Windows.Forms;

namespace ActionCenterRssFeed
{
    public class ActionCenterRssFeedApplicationContext : ApplicationContext
    {
        
        private List<RssFeed> rssFeeds;
        private System.Timers.Timer timer;
        private NotifyIcon trayIcon;

        public ActionCenterRssFeedApplicationContext()
        {
            // Hard code an RSS feed for now
            rssFeeds = new List<RssFeed>();
            rssFeeds.Add(new RssFeed("C:/Users/tutkowskim/Downloads/TestFeed.rss"));

            // Create a timer to update the notifications
            timer = new System.Timers.Timer()
            {
                AutoReset = true,
                Enabled = true,
                Interval = 500,
            };
            timer.Elapsed += new ElapsedEventHandler(UpdateNotifications);

            // Initialize Tray Icon
            trayIcon = new NotifyIcon()
            {
                Icon = new System.Drawing.Icon(Assets.RssIconPath),
                ContextMenu = new ContextMenu(new MenuItem[] {
                    new MenuItem("Update Notifications", UpdateNotifications),
                    new MenuItem("Exit", Exit)
                }),
                Visible = true
            };
        }

        private void UpdateNotifications(object sender, EventArgs e)
        {
            foreach (var feed in rssFeeds)
            {
                IEnumerable<System.ServiceModel.Syndication.SyndicationItem> newRssFeedEntries = new List<System.ServiceModel.Syndication.SyndicationItem>(); ;
                feed.UpdateRssFeed(out newRssFeedEntries);
                foreach (var item in newRssFeedEntries)
                {
                    ToastManager.Toast(item.Title.Text, item.Summary.Text);
                }
            }
        }

        private void Exit(object sender, EventArgs e)
        {
            // Hide tray icon, otherwise it will remain shown until user mouses over it
            trayIcon.Visible = false;

            Application.Exit();
        }
    }
}
