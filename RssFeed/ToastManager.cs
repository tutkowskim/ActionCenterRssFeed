using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace RssFeed
{
    /// <summary>
    /// Post notifications to Window's Toast ToastNotificationManager
    /// </summary>
    public static class ToastManager
    {
        /// <summary>
        /// Id for this application. This must match the id set on the shortcut in the start menu in order for notifactions to work.
        /// </summary>
        private const string ApplicationId = "RssFeedAppUserModelID";

        /// <summary>
        /// Create a Windows Toast Notification
        /// </summary>
        /// <param name="title">Title for the toast</param>
        /// <param name="summary">Summary in the toast</param>
        public static void Toast(string title, string summary)
        {
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText03);

            // Fill in the text elements
            XmlNodeList stringElements = toastXml.GetElementsByTagName("text");
            stringElements[0].AppendChild(toastXml.CreateTextNode(title));
            stringElements[1].AppendChild(toastXml.CreateTextNode(summary));

            // Specify the absolute path to an image
            string imagePath = "file:///" + Assets.RssImagePath;
            XmlNodeList imageElements = toastXml.GetElementsByTagName("image");
            imageElements[0].Attributes.GetNamedItem("src").NodeValue = imagePath;

            ToastNotification toast = new ToastNotification(toastXml);
            ToastNotificationManager.CreateToastNotifier(ApplicationId).Show(toast);
        }

        /// <summary>
        /// Create a Windows Toast Notification for the given feed item
        /// </summary>
        /// <param name="feedItem"></param>
        public static void Toast(FeedItem feedItem)
        {
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText04);

            // Fill in the text elements
            XmlNodeList stringElements = toastXml.GetElementsByTagName("text");
            stringElements[0].AppendChild(toastXml.CreateTextNode(feedItem.Title));
            stringElements[1].AppendChild(toastXml.CreateTextNode(feedItem.FeedTitle));
            stringElements[2].AppendChild(toastXml.CreateTextNode(feedItem.Summary));

            // Specify the absolute path to an image
            string imagePath = "file:///" + Assets.RssImagePath;
            XmlNodeList imageElements = toastXml.GetElementsByTagName("image");
            imageElements[0].Attributes.GetNamedItem("src").NodeValue = imagePath;

            ToastNotification toast = new ToastNotification(toastXml);
            toast.Activated += delegate { feedItem.OpenLink(); };

            ToastNotificationManager.CreateToastNotifier(ApplicationId).Show(toast);
        }
    }
}
