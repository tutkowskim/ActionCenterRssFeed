using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Timers;
using System.Xml;
using System.Xml.Serialization;

namespace RssFeed
{
    public class RssFeedReader
    {
        private Timer _timer;

        [XmlElement("rss_feed_uri")]
        public string RssFeedUri { get; set } = string.Empty;


        [XmlElement("update_interval")]
        public double FeedUpdateInterval
        {
            get
            {
                return _timer != null ? _timer.Interval : -1;
            }
            set
            {
                if (_timer != null && _timer.Interval != value)
                {
                    _timer.Interval = value;
                }
            }
        }

        private SyndicationFeed _feed;

        public RssFeedReader()
        {
            // Create a timer to update the notifications
            _timer = new Timer()
            {
                AutoReset = true,
                Enabled = true,
                Interval = 500,
            };
            _timer.Elapsed += new ElapsedEventHandler(UpdateFeedAndSendNotifications);
        }

        private void UpdateFeedAndSendNotifications(object sender, ElapsedEventArgs e)
        {
            IEnumerable<SyndicationItem> newRssFeedEntries;
            UpdateRssFeed(out newRssFeedEntries);
            foreach (var item in newRssFeedEntries)
            {
                ToastManager.Toast(item.Title.Text, _feed.Title.Text, item.Summary.Text);
            }
        }

        private void UpdateRssFeed()
        {
            try
            {
                using (var reader = XmlReader.Create(RssFeedUri))
                {
                    _feed = SyndicationFeed.Load(reader);
                }
            }
            catch
            {
                // There was an issue reading the stream. Ignore it for now
            }
        }

        private void UpdateRssFeed(out IEnumerable<SyndicationItem> newItems)
        {
            // Save off the current feed
            SyndicationFeed oldFeed = _feed;

            // Update the feed
            UpdateRssFeed();

            // Determine the new items
            if (_feed == null)
            {
                newItems = new List<SyndicationItem>();
            }
            else if (oldFeed == null)
            {
                newItems = _feed.Items;
            }
            else
            {
                newItems = _feed.Items.Where(item => !oldFeed.Items.Any(oldItem => oldItem.Id == item.Id));
            }
        }
    }
}
