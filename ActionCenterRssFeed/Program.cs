using System.Windows.Forms;

namespace ActionCenterRssFeed
{
    class Program
    {
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ActionCenterRssFeedApplicationContext());
        }
    }
}
