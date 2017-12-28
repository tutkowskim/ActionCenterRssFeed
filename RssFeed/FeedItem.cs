using System;
using System.Xml.Serialization;

namespace RssFeed
{
    /// <summary>
    /// An RSS Feed Item.
    /// </summary>
    public class FeedItem
    {
        private readonly static log4net.ILog logger = log4net.LogManager.GetLogger("tutkowski.rssfeed.feeditem");

        [XmlIgnore]
        /// <summary>
        /// The feed this item was published to.
        /// </summary>
        public Feed Feed { get; set; }

        /// <summary>
        /// A unique identifer for this item in the feed.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// The title for this item.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// A summary or description for this item.
        /// </summary>
        public string Summary { get; set; } = string.Empty;

        /// <summary>
        /// The date this item was published to the feed.
        /// </summary>
        public DateTime PublishDate { get; set; } = DateTime.Now;

        /// <summary>
        /// The title for the feed this item came from.
        /// </summary>
        public string FeedTitle
        {
            get
            {
                return Feed != null ? Feed.Title : string.Empty;
            }
        }

        /// <summary>
        /// An external link to this item.
        /// </summary>
        public string Link { get; set; } = string.Empty;

        /// <summary>
        /// Open the link to this feed item.
        /// </summary>
        public void OpenLink()
        {
            if (!string.IsNullOrWhiteSpace(Link))
            {
                System.Diagnostics.Process.Start(Link);
            }
            else
            {
                logger.Info(string.Format("Unable to open link for the item {0} in the feed, since it is null or empty.", Title, FeedTitle));
            }
        }

        /// <summary>
        /// The logo for the feed that this item came from.
        /// </summary>
        public string FeedImageUrl
        {
            get
            {
                return Feed != null ? Feed.ImageUri : string.Empty;
            }
        }

        /// <summary>
        /// The feed for this item has a logo.
        /// </summary>
        public bool HasImageUri
        {
            get
            {
                return Feed != null ? Feed.HasImageUri : false;
            }
        }
    }
}
