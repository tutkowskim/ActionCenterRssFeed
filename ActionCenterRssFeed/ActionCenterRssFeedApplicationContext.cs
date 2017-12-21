using System;
using System.IO;
using System.Windows.Forms;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace ActionCenterRssFeed
{
    public class ActionCenterRssFeedApplicationContext : ApplicationContext
    {
        private NotifyIcon trayIcon;

        public ActionCenterRssFeedApplicationContext()
        {
            // Initialize Tray Icon
            trayIcon = new NotifyIcon()
            {
                Icon = new System.Drawing.Icon(RssIconPath),
                ContextMenu = new ContextMenu(new MenuItem[] {
                    new MenuItem("Add Notification", AddNotification),
                    new MenuItem("Exit", Exit)
                }),
                Visible = true
            };
        }

        private void AddNotification(object sender, EventArgs e)
        {
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText04);

            // Fill in the text elements
            XmlNodeList stringElements = toastXml.GetElementsByTagName("text");
            for (int i = 0; i < stringElements.Length; i++)
            {
                stringElements[i].AppendChild(toastXml.CreateTextNode("Line " + i));
            }

            // Specify the absolute path to an image
            string imagePath = "file:///" + RssImagePath;
            XmlNodeList imageElements = toastXml.GetElementsByTagName("image");
            imageElements[0].Attributes.GetNamedItem("src").NodeValue = imagePath;

            var blah = toastXml.GetXml();
            ToastNotification toast = new ToastNotification(toastXml);
            ToastNotificationManager.CreateToastNotifier("ActionCenterRssFeedAppUserModelID").Show(toast);
        }

        private void Exit(object sender, EventArgs e)
        {
            // Hide tray icon, otherwise it will remain shown until user mouses over it
            trayIcon.Visible = false;

            Application.Exit();
        }

        private string InstallDirectory
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory;
            }
        }

        private string RssIconPath
        {
            get
            {
                return Path.Combine(InstallDirectory, "RSS.ico");
            }
        }

        private string RssImagePath
        {
            get
            {
                return Path.Combine(InstallDirectory, "RSS.png");
            }
        }
    }
}
