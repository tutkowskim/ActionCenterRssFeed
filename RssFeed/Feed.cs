using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;

namespace RssFeed
{
    public class Feed
    {
        private readonly static log4net.ILog logger = log4net.LogManager.GetLogger("tutkowski.rssfeed.rssfeed");

        public string Title { get; set; } = string.Empty;

        public List<FeedItem> Items { get; set; } = new List<FeedItem>();

        public string ImageUri { get; set; } = string.Empty;

        public bool HasImageUri
        {
            get
            {
                return !string.IsNullOrWhiteSpace(ImageUri);
            }
        }

        public Feed()
        {
        }

        public Feed(SyndicationFeed syndicationFeed)
        {
            // Bail if the feed is null
            if (syndicationFeed == null)
            {
                logger.Warn("The SyndicationFeed is null!");
                return;
            }

            // Set the feed title
            if (syndicationFeed.Title != null)
            {
                var syndicationFeedTitleText = syndicationFeed.Title.Text;
                Title = !string.IsNullOrWhiteSpace(syndicationFeedTitleText) ? syndicationFeedTitleText : string.Empty;
            }
            else
            {
                logger.Warn("The SyndicationFeed title is null even though it is a required field.");
            }

            // Set the feed's image
            ImageUri = syndicationFeed.ImageUrl != null ? syndicationFeed.ImageUrl.AbsolutePath : string.Empty;

            // Set the feed's items
            Items = new List<FeedItem>();
            if (syndicationFeed.Items != null)
            {
                foreach (var syndicationItem in syndicationFeed.Items)
                {
                    string title;
                    string summary;

                    // Either the title or summary must contain a value. Not neccisarlly both.
                    if (syndicationItem.Title != null && syndicationItem.Summary != null)
                    {
                        title = !string.IsNullOrWhiteSpace(syndicationItem.Title.Text) ? syndicationItem.Title.Text : string.Empty;
                        summary = !string.IsNullOrWhiteSpace(syndicationItem.Summary.Text) ? syndicationItem.Summary.Text : string.Empty;
                    }
                    else if (syndicationItem.Title != null && syndicationItem.Summary != null)
                    {
                        title = !string.IsNullOrWhiteSpace(syndicationItem.Title.Text) ? syndicationItem.Title.Text : string.Empty;
                        summary = string.Empty;
                    }
                    else if (syndicationItem.Title == null && syndicationItem.Summary != null)
                    {
                        title = !string.IsNullOrWhiteSpace(syndicationItem.Summary.Text) ? syndicationItem.Summary.Text : string.Empty;
                        summary = string.Empty;
                    }
                    else
                    {
                        continue;
                    }

                    Items.Add(new FeedItem(this)
                    {
                        Id = syndicationItem.Id,
                        Title = title,
                        Summary = summary,
                        PublishDate = syndicationItem.PublishDate.DateTime,
                    });
                }
            }
        }
    }
}
