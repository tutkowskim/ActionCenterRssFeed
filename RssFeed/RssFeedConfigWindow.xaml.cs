using System.Collections.Generic;
using System.Windows;

namespace RssFeed
{
    /// <summary>
    /// Interaction logic for RssFeedConfigWindow.xaml
    /// </summary>
    public partial class RssFeedConfigWindow : Window
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="feedReaders">The collection of readers to configure.</param>
        public RssFeedConfigWindow(IEnumerable<RssFeedReader> feedReaders)
        {
            InitializeComponent();
            RssFeedsGrid.ItemsSource = feedReaders;
        }
    }
}
