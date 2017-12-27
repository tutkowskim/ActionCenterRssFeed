using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
namespace RssFeed
{
    /// <summary>
    /// Interaction logic for RssFeedConfigWindow.xaml
    /// </summary>
    public partial class RssFeedConfigWindow : Window
    {
        private readonly static log4net.ILog logger = log4net.LogManager.GetLogger("tutkowski.rssfeed.rssfeedconfigwindow");

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="feedReaders">The collection of readers to configure.</param>
        public RssFeedConfigWindow(IEnumerable<RssFeedReader> feedReaders)
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
            RssFeedReader feedReader = passwordBox.DataContext as RssFeedReader;
            if (feedReader != null)
            {
                feedReader.RssFeedCredentials.Password = passwordBox.Password;
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
            RssFeedReader feedReader = passwordBox.DataContext as RssFeedReader;
            if (feedReader != null)
            {
                passwordBox.Password = feedReader.RssFeedCredentials.Password;
            }
            else
            {
                logger.Warn("Unable to get RssFeedReader associated with password box.");
            }
        }
    }
}
