using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VlogManager_Client.Comands;
using VlogManager_Client.Models;

namespace VlogManager_Client.ViewModel
{
    public class PodcastConfiguringVM
    {

        public PodcastRecord RecordToConfig { get; init; }
        public List<string> Groups { get; init; }

        public PodcastConfiguringVM(PodcastRecord record)
        {
            RecordToConfig = record;
            Groups = Application.Current.Resources["GroupNames"] as List<string> ?? new List<string> { "Audio", "Video" };
        }

        //Processes another podcast icon selection
        public void SelectAnotherImage()
        {
            var dlg = new OpenFileDialog();
            dlg.Filter = "Image files(*.bmp,*.jpg)|*.bmp;*.jpg|*.png;|*.tif)";
            if(dlg.ShowDialog() ?? false)
                if (dlg.FileName == string.Empty) return;
                    RecordToConfig.ImageUrl = dlg.FileName;
        }

        /// <summary>
        /// changes podcast's name
        /// </summary>
        /// <param name="newName"> Podcast new name</param>
        public void ChangePodcastName(string newName)
        {
            RecordToConfig.Name = newName;
        }

        /// <summary>
        /// Changes podcast's group
        /// </summary>
        /// <param name="groupName">new group name or name of existing one</param>
        public void ChangePodcastGroup(string groupName)
        {
            RecordToConfig.GroupName = groupName;
        }
    }
}
