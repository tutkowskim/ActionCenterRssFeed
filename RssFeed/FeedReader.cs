using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.ServiceModel.Syndication;
using System.Timers;
using System.Xml;
using System.Xml.Serialization;

namespace RssFeed
{
    /// <summary>
    /// RSS Feed Reader which reads an rss fead at a given interval.
    /// </summary>
    public class FeedReader : INotifyPropertyChanged
    {
        private readonly static log4net.ILog logger = log4net.LogManager.GetLogger("tutkowski.rssfeed.reader");
        private Timer _timer;
        private Feed _feed = new Feed();

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The current conect of this stream.
        /// </summary>
        /// <remarks>
        /// This is serilalized out so that the notifications aren't redisplayed for an item after restarting the application.
        /// </remarks>
        [XmlElement("feed_content")]
        public Feed Feed {
            get
            {
                return _feed;
            }
            set
            {
                if (_feed != value)
                {
                    _feed = value;
                    OnPropertyChanged("Feed");
                }
            }
        }

        /// <summary>
        /// The uri to the rss feed.
        /// </summary>
        [XmlElement("feed_uri")]
        public string FeedUri { get; set; } = string.Empty;

        [XmlElement("feed_credentials")]
        public Credentials FeedCredentials { get; set; } = new Credentials();

        /// <summary>
        /// Determines if the feed reader is enabled or disabled.
        /// </summary>
        [XmlElement("enabled")]
        public bool Enabled {
            get
            {
                if (_timer != null)
                {
                    return _timer.Enabled;
                }
                else
                {
                    logger.Warn(string.Format("Failed to get the timer enabled state for {0}. The timer is null.", FeedUri));
                    return false;
                }
            }
            set
            {
                if (_timer != null)
                {
                    _timer.Enabled = value;
                    OnPropertyChanged("Enabled");
                }
                else
                {
                    logger.Warn(string.Format("Failed to set the timer enabled state for {0}. The timer is null.", FeedUri));
                }
            }
        }

        /// <summary>
        /// The interval at which to read the rss feed.
        /// </summary>
        [XmlElement("update_interval")]
        public double FeedUpdateInterval
        {
            get
            {
                if (_timer != null)
                {
                    return _timer.Interval;
                }
                else
                {
                    logger.Warn(string.Format("Failed to get the timer interval for {0}. The timer is null.", FeedUri));
                    return -1;
                }
            }
            set
            {
                if (_timer != null)
                {
                    _timer.Interval = value;
                    OnPropertyChanged("FeedUpdateInterval");
                }
                else
                {
                    logger.Warn(string.Format("Failed to set the timer interval for {0}. The timer is null.", FeedUri));
                }
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public FeedReader()
        {
            // Create a timer to update the notifications
            _timer = new Timer()
            {
                AutoReset = true,
                Enabled = false,
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
            IEnumerable<FeedItem> newRssFeedEntries;
            UpdateRssFeed(out newRssFeedEntries);
            foreach (var item in newRssFeedEntries)
            {
                ToastManager.Toast(item.Title, item.FeedTitle, item.Summary);
            }
        }

        /// <summary>
        /// Update the feed and get the new items.
        /// </summary>
        /// <param name="newItems">The new items.</param>
        private void UpdateRssFeed(out IEnumerable<FeedItem> newItems)
        {
            // Save off the current feed
            Feed oldFeed = Feed;

            // Update the feed
            UpdateRssFeed();

            // Determine the new items
            if (Feed == null)
            {
                newItems = new List<FeedItem>();
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

        /// <summary>
        /// Update the RSS feed.
        /// </summary>
        private void UpdateRssFeed()
        {
            logger.Debug(string.Format("Updating the feed for {0}", FeedUri));

            try
            {
                XmlReaderSettings readerSettings = null;
                using (SecureString password = FeedCredentials.GetSecurePassword())
                {
                    readerSettings = new XmlReaderSettings()
                    {
                        XmlResolver = new XmlUrlResolver()
                        {
                            Credentials = new NetworkCredential(FeedCredentials.Username, password)
                        }
                    };
                }
                using (XmlReader reader = XmlReader.Create(FeedUri, readerSettings))
                {
                    Feed = new Feed(SyndicationFeed.Load(reader));
                }
            }
            catch (Exception e)
            {
                Enabled = false;
                ToastManager.Toast("Failed to Update Feed", string.Format("Failed to update the feed {0}. Check settings and renable the feed.", FeedUri));
                logger.Error(string.Format("Failed to update the feed {0} due to the exception {1}", FeedUri, e.Message));
            }

            logger.Debug(string.Format("Finished updating the feed for {0}", FeedUri));
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
