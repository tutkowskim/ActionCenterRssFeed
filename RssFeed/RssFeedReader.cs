using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Timers;
using System.Xml;
using System.Xml.Serialization;

namespace RssFeed
{
    /// <summary>
    /// RSS Feed Reader which reads an rss fead at a given interval.
    /// </summary>
    public class RssFeedReader
    {
        private readonly static log4net.ILog logger = log4net.LogManager.GetLogger("tutkowski.rssfeed.reader");
        private Timer _timer;
        private SyndicationFeed _feed;

        /// <summary>
        /// The uri to the rss feed.
        /// </summary>
        [XmlElement("rss_feed_uri")]
        public string RssFeedUri { get; set; } = string.Empty;

        /// <summary>
        /// The interval at which to read the rss feed.
        /// </summary>
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

        /// <summary>
        /// Constructor
        /// </summary>
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

        /// <summary>
        /// Update a feed and send notifications for new items to the ToastManger
        /// </summary>
        /// <param name="sender">The timer that caused this event.</param>
        /// <param name="e">Timer event args/</param>
        private void UpdateFeedAndSendNotifications(object sender, ElapsedEventArgs e)
        {
            IEnumerable<SyndicationItem> newRssFeedEntries;
            UpdateRssFeed(out newRssFeedEntries);
            foreach (var item in newRssFeedEntries)
            {
                ToastManager.Toast(item.Title.Text, _feed.Title.Text, item.Summary.Text);
            }
        }


        /// <summary>
        /// Update the feed and get the new items.
        /// </summary>
        /// <param name="newItems">The new items.</param>
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

        /// <summary>
        /// Update the RSS feed.
        /// </summary>
        private void UpdateRssFeed()
        {
            logger.Debug(string.Format("Updating the feed for {0}", RssFeedUri));

            try
            {
                using (var reader = XmlReader.Create(RssFeedUri))
                {
                    _feed = SyndicationFeed.Load(reader);
                }
            }
            catch (Exception e)
            {
                logger.Error(string.Format("Failed to update the feed for {0} due to the exception {1}", RssFeedUri, e.Message));
            }

            logger.Debug(string.Format("Finished updating the feed for {0}", RssFeedUri));
        }
    }
}
