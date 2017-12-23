using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Timers;
using System.Xml;

namespace ActionCenterRssFeed
{
    public class RssFeedReader
    {
        private Timer _timer;
        public string RssFeedUri { get; set; }
        public double FeedUpdateInterval
        {
            get
            {
                return _timer != null ? _timer.Interval : -1;
            }
            set
            {
                if (_timer != null)
                {
                    _timer.Interval = value;
                }
            }
        }
        
        public SyndicationFeed Feed
        {
            get;
            private set;
        }

        public RssFeedReader(string uri, double feedUpdateInterval)
        {
            RssFeedUri = uri;

            // Create a timer to update the notifications
            _timer = new Timer()
            {
                AutoReset = true,
                Enabled = true,
                Interval = feedUpdateInterval,
            };
            _timer.Elapsed += new ElapsedEventHandler(UpdateFeedAndSendNotifications);
        }

        private void UpdateFeedAndSendNotifications(object sender, ElapsedEventArgs e)
        {
            IEnumerable<SyndicationItem> newRssFeedEntries;
            UpdateRssFeed(out newRssFeedEntries);
            foreach (var item in newRssFeedEntries)
            {
                ToastManager.Toast(item.Title.Text, Feed.Title.Text, item.Summary.Text);
            }
        }

        private void UpdateRssFeed(out IEnumerable<SyndicationItem> newItems)
        {
            // Save off the current feed
            SyndicationFeed oldFeed = Feed;

            // Update the feed
            using (var reader = XmlReader.Create(RssFeedUri))
            {
                Feed = SyndicationFeed.Load(reader);
            }

            // Determine the new items
            if (Feed == null)
            {
                newItems = new List<SyndicationItem>();
            }
            else if (oldFeed == null)
            {
                newItems = Feed.Items;
            }
            else
            {
                newItems = Feed.Items.Where(item => !oldFeed.Items.Any(oldItem => oldItem.Id == item.Id));
            }
        }
    }
}
