using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace ActionCenterRssFeed
{
    public static class ToastManager
    {
        /// <summary>
        /// Id for this application. This must match the id set on the shortcut in the start menu in order for notifactions to work.
        /// </summary>
        private const string ApplicationId = "ActionCenterRssFeedAppUserModelID";

        /// <summary>
        /// Create a Windows Toast Notification
        /// </summary>
        /// <param name="title">Title for the toast</param>
        /// <param name="summary">Summary in the toast</param>
        public static void Toast(string title, string summary)
        {
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText04);

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
    }
}
