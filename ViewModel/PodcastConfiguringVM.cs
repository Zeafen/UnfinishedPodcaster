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

        public void SelectAnotherImage()
        {
            var dlg = new OpenFileDialog();
            dlg.Filter = "Image files|*.bmp;*.jpg;*.png;*.tif|";
            dlg.ShowDialog();
            if (dlg.FileName == string.Empty) return;
            RecordToConfig.ImageUrl = dlg.FileName;
        }

        public void ChangePodcastName(string newName)
        {
            RecordToConfig.Name = newName;
        }

        public void ChangePodcastGroup(string groupName)
        {
            RecordToConfig.GroupName = groupName;
        }
    }
}
