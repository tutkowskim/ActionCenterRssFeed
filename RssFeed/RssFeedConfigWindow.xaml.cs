using System.Collections.Generic;
using System.Windows;

namespace RssFeed
{
    /// <summary>
    /// Interaction logic for RssFeedConfigWindow.xaml
    /// </summary>
    public partial class RssFeedConfigWindow : Window
    {
        public RssFeedConfigWindow(IEnumerable<RssFeedReader> feedReaders)
        {
            InitializeComponent();
            RssFeedsGrid.ItemsSource = feedReaders;
        }
    }
}
