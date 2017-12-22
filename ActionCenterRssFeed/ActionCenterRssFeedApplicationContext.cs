using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ActionCenterRssFeed
{
    public class ActionCenterRssFeedApplicationContext : ApplicationContext
    {
        
        private List<RssFeed> rssFeeds;
        private ToastManager toastMaster;
        //private string timer;
        private NotifyIcon trayIcon;

        public ActionCenterRssFeedApplicationContext()
        {
            rssFeeds = new List<RssFeed>();
            rssFeeds.Add(new RssFeed("https://www.nasa.gov/rss/dyn/nasax_vodcast.rss"));

            toastMaster = new ToastManager();

            // Initialize Tray Icon
            trayIcon = new NotifyIcon()
            {
                Icon = new System.Drawing.Icon(Assets.RssIconPath),
                ContextMenu = new ContextMenu(new MenuItem[] {
                    new MenuItem("Add Notification", AddNotifications),
                    new MenuItem("Exit", Exit)
                }),
                Visible = true
            };
        }

        private void AddNotifications(object sender, EventArgs e)
        {
            foreach (var feed in rssFeeds)
            {
                IEnumerable<System.ServiceModel.Syndication.SyndicationItem> newRssFeedEntries = new List<System.ServiceModel.Syndication.SyndicationItem>(); ;
                feed.UpdateRssFeed(out newRssFeedEntries);
                foreach (var item in newRssFeedEntries)
                {
                    toastMaster.Toast(item.Title.Text, item.Summary.Text);
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
