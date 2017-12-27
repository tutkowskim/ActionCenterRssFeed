using System;

namespace RssFeed
{
    /// <summary>
    /// An RSS Feed Item.
    /// </summary>
    public class FeedItem
    {
        private Feed _feed;

        /// <summary>
        /// Default Contructor
        /// </summary>
        public FeedItem()
        {
            _feed = null;
        }

        /// <summary>
        /// Contructor
        /// </summary>
        public FeedItem(Feed feed)
        {
            _feed = feed;
        }

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
                return _feed != null ? _feed.Title : string.Empty;
            }
        }

        /// <summary>
        /// The logo for the feed that this item came from.
        /// </summary>
        public string FeedImageUrl
        {
            get
            {
                return _feed != null ? _feed.ImageUri : string.Empty;
            }
        }

        /// <summary>
        /// The feed for this item has a logo.
        /// </summary>
        public bool HasImageUri
        {
            get
            {
                return _feed != null ? _feed.HasImageUri : false;
            }
        }
    }
}
