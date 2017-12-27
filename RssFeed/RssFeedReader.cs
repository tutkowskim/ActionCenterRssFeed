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
    public class RssFeedReader : INotifyPropertyChanged
    {
        private readonly static log4net.ILog logger = log4net.LogManager.GetLogger("tutkowski.rssfeed.reader");
        private Timer _timer;
        private SyndicationFeed _feed;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The uri to the rss feed.
        /// </summary>
        [XmlElement("rss_feed_uri")]
        public string RssFeedUri { get; set; } = string.Empty;

        [XmlElement("rss_feed_credentials")]
        public Credentials RssFeedCredentials { get; set; } = new Credentials();

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
                    logger.Warn(string.Format("Failed to get the timer enabled state for {0}. The timer is null.", RssFeedUri));
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
                    logger.Warn(string.Format("Failed to set the timer enabled state for {0}. The timer is null.", RssFeedUri));
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
                    logger.Warn(string.Format("Failed to get the timer interval for {0}. The timer is null.", RssFeedUri));
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
                    logger.Warn(string.Format("Failed to set the timer interval for {0}. The timer is null.", RssFeedUri));
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
                XmlReaderSettings readerSettings = null;
                using (SecureString password = RssFeedCredentials.GetSecurePassword())
                {
                    readerSettings = new XmlReaderSettings()
                    {
                        XmlResolver = new XmlUrlResolver()
                        {
                            Credentials = new NetworkCredential(RssFeedCredentials.Username, password)
                        }
                    };
                }
                using (XmlReader reader = XmlReader.Create(RssFeedUri, readerSettings))
                {
                    _feed = SyndicationFeed.Load(reader);
                }
            }
            catch (Exception e)
            {
                Enabled = false;
                ToastManager.Toast("Failed to Update Feed", string.Format("Failed to update the feed {0}. Check settings and renable the feed.", RssFeedUri));
                logger.Error(string.Format("Failed to update the feed {0} due to the exception {1}", RssFeedUri, e.Message));
            }

            logger.Debug(string.Format("Finished updating the feed for {0}", RssFeedUri));
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
