using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;

namespace VlogManager_Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            string directoryPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @$"\Enthusiast podcast manager";
            Resources.Add("DataDirectory", directoryPath);
            Resources.Add("GroupNames", new List<string> { "Audio", "Video" });
            Resources.Add("SuggestedPodcasts", new List<string> { "https://feeds.buzzsprout.com/1386361.rss", "https://feed.podbean.com/floatingleaves/feed.xml", "https://feeds.redcircle.com/c1cf2c52-3c7d-4278-9e30-c94bab3800eb", "https://talkingtea.libsyn.com/rss" });
            if (!Directory.Exists(directoryPath +@"\Downloads"))
                Directory.CreateDirectory(directoryPath + @"\Downloads");

        }
    }

}
