using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ActionCenterRssFeed
{
    public class RssFeed
    {
        private string RssFeedUri;

        public RssFeed(string uri)
        {
            RssFeedUri = uri;
        }

        public IEnumerable<SyndicationItem> Items
        {
            get
            {
                SyndicationFeed feed;
                using (var reader = XmlReader.Create(RssFeedUri))
                {
                    feed = SyndicationFeed.Load(reader);
                }
                return feed.Items;
            }
        }
    }
}
