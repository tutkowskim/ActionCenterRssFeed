﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace RssFeed
{
    /// <summary>
    /// Interaction logic for aggregateFeedWindow.xaml
    /// </summary>
    public partial class AggregateFeedWindow : Window
    {
        private readonly static log4net.ILog logger = log4net.LogManager.GetLogger("tutkowski.rssfeed.aggregatefeedwindow");

        private IEnumerable<FeedReader> _feedReaders;
        private ObservableCollection<FeedItem> _feedItems = new ObservableCollection<FeedItem>();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="feedReaders"></param>
        public AggregateFeedWindow(ObservableCollection<FeedReader> feedReaders)
        {
            InitializeComponent();
            _feedReaders = feedReaders;

            // Listen for changes to the feeds
            feedReaders.CollectionChanged += ResfreshItems;
            foreach(FeedReader reader in feedReaders)
            {
                reader.PropertyChanged += ResfreshItems;
            }

            // Set the source of the list view
            FeedItems.ItemsSource = _feedItems;

            // Set the current items in the feeds
            ResfreshItems(null, null);
        }

        /// <summary>
        /// Event handler for the feeds' collection or the feeds changing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResfreshItems(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(delegate ()
            {
                OrderItemsByPublishDateAndDisplay();
            }));
        }

        /// <summary>
        /// Get the items to show, sort them, and display them.
        /// </summary>
        private void OrderItemsByPublishDateAndDisplay()
        {
            List<FeedItem> updatedFeedItemList = new List<FeedItem>();
            foreach (FeedReader reader in _feedReaders)
            {
                if (reader.Feed != null)
                {
                    updatedFeedItemList.AddRange(reader.Feed.Items);
                }
            }

            // Remove any items that no longer exist
            var removedItems = (new List<FeedItem>(_feedItems)).Where(item => !updatedFeedItemList.Any(updatedItem => item.Id == updatedItem.Id));
            foreach (var item in removedItems)
            {
                _feedItems.Remove(item);
            }

            // Add any new items
            var newItems = updatedFeedItemList.Where(updatedItem => !_feedItems.Any(item => updatedItem.Id == item.Id));
            foreach (var item in newItems)
            {
                _feedItems.Add(item);
            }

            // Resort the list if neccissary
            var sortableList = new List<FeedItem>(_feedItems);
            sortableList.Sort((item1, item2) => item2.PublishDate.CompareTo(item1.PublishDate));
            for (int i = 0; i < sortableList.Count; ++i)
            {
                _feedItems.Move(_feedItems.IndexOf(sortableList[i]), i);
            }
        }

        /// <summary>
        /// Action handler for clicking an item in the feed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FeedItemClicked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            FrameworkElement control = sender as FrameworkElement;
            FeedItem item = control.DataContext as FeedItem;
            if (item != null)
            {
                item.OpenLink();
            }
            else
            {
                logger.Warn("Unable to determine the item clicked.");
            }
        }

        /// <summary>
        /// After the web browser for a feed item is initialized set it's content.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WebBrowser_Initialized(object sender, EventArgs e)
        {
            WebBrowser browser = sender as WebBrowser;
            FeedItem item = browser.DataContext as FeedItem;
            if (item != null && browser != null)
            {
                // The summary for an RSS item can be in HTML so we are using a web browser to render it,
                // and in order to make the other text look consistent we will just stick it all into HTML.
                string html = string.Format("<h2>{0}</h2><b>{1}, {2}</b><br/>{3}", 
                    item.Title, 
                    item.FeedTitle,
                    item.PublishDate,
                    item.Summary);
                browser.NavigateToString(html);
            }
            else
            {
                logger.Warn("Unable to determine the webbrowser loaded or the rss feed associated with it.");
            }
        }

        /// <summary>
        /// After the content is loaded for a feed item set the Height so that all the content is visable.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WebBrowser_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            WebBrowser webBrowser = sender as WebBrowser;
            if (webBrowser == null)
            {
                logger.Warn("Unable to determine the web browser that complted loading.");
                return;
            }

            mshtml.HTMLDocument doc = webBrowser.Document as mshtml.HTMLDocument;
            if (doc == null)
            {
                logger.Warn("Unable to get the HTMLDocument from the web browser.");
                return;
            }

            mshtml.IHTMLElement2 elem = doc.activeElement as mshtml.IHTMLElement2;
            if (elem == null)
            {
                logger.Warn("Unable to get the internal html element from the web browser.");
                return;
            }

            doc.body.style.overflow = "hidden";
            webBrowser.Height = elem.scrollHeight;
        }
    }
}
