using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Xml;

namespace ActionCenterRssFeed
{
    public class RssFeed
    {
        private string RssFeedUri
        {
            get;
            set;
        }

        public IEnumerable<SyndicationItem> Items
        {
            get;
            private set;
        }

        public RssFeed(string uri)
        {
            RssFeedUri = uri;
        }

        public void UpdateRssFeed()
        {
            using (var reader = XmlReader.Create(RssFeedUri))
            {
                SyndicationFeed feed = SyndicationFeed.Load(reader);
                Items = feed.Items;
            }
        }

        public void UpdateRssFeed(out IEnumerable<SyndicationItem> newItems)
        {
            // Save off the current items
            IEnumerable<SyndicationItem> oldItems = Items;

            // Update the feed
            UpdateRssFeed();

            // Determine the new items
            if (Items == null)
            {
                newItems = null;
            }
            else if (oldItems == null)
            {
                newItems = Items;
            }
            else
            {
                newItems = Items.Where(item => !oldItems.Any(oldItem => oldItem.Id == item.Id));
            }
        }
    }
}
