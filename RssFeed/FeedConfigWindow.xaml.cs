using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
namespace RssFeed
{
    /// <summary>
    /// Interaction logic for RssFeedConfigWindow.xaml
    /// </summary>
    public partial class FeedConfigWindow : Window
    {
        private readonly static log4net.ILog logger = log4net.LogManager.GetLogger("tutkowski.rssfeed.rssfeedconfigwindow");

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="feedReaders">The collection of readers to configure.</param>
        public FeedConfigWindow(IEnumerable<FeedReader> feedReaders)
        {
            InitializeComponent();
            RssFeedsGrid.ItemsSource = feedReaders;
        }

        /// <summary>
        /// Set the password on the rss feed reader when the password is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            FeedReader feedReader = passwordBox.DataContext as FeedReader;
            if (feedReader != null)
            {
                feedReader.FeedCredentials.Password = passwordBox.Password;
            }
            else
            {
                logger.Warn("Unable to get RssFeedReader associated with password box.");
            }
        }

        /// <summary>
        /// Set the password on the password box once it's created.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PasswordBox_Loaded(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            FeedReader feedReader = passwordBox.DataContext as FeedReader;
            if (feedReader != null)
            {
                passwordBox.Password = feedReader.FeedCredentials.Password;
            }
            else
            {
                logger.Warn("Unable to get RssFeedReader associated with password box.");
            }
        }

        /// <summary>
        /// Commit the edit when unloading the datagrid.
        ///<remark>
        /// This resolves the issue of not being able to reopen the config window after
        /// closing the window with an unsaved edit.
        /// </remark>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RssFeedsGrid_Unloaded(object sender, RoutedEventArgs e)
        {
            DataGrid grid = sender as DataGrid;
            if (grid != null)
            {
                grid.CommitEdit(DataGridEditingUnit.Row, true);
            }
        }
    }
}
