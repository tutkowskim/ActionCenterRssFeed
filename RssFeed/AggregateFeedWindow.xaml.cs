using System;
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

            // Set the current items in the feeds
            OrderItemsByPublishDate();
        }

        /// <summary>
        /// Event handler for the feeds' collection or the feeds changing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResfreshItems(object sender, EventArgs e)
        {
            OrderItemsByPublishDate();
        }

        /// <summary>
        /// Get the items to show, sort them, and display them.
        /// </summary>
        private void OrderItemsByPublishDate()
        {
            List<FeedItem> updatedItemList = new List<FeedItem>();

            // Add all of the items to a temporay list
            foreach (FeedReader reader in _feedReaders)
            {
                if(reader.Feed != null)
                {
                    updatedItemList.AddRange(reader.Feed.Items);
                }
            }

            // Sort the items
            updatedItemList.Sort((item1, item2) => item2.PublishDate.CompareTo(item1.PublishDate));

            // Update the list
            Dispatcher.BeginInvoke(new Action(delegate ()
            {
                FeedItems.ItemsSource = updatedItemList;
            }));
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
    }
}
