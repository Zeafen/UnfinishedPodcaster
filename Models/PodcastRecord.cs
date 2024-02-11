using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace VlogManager_Client.Models
{
    public class PodcastRecord : INotifyPropertyChanged
    {
        [AutoIncrement]
        public int ID { get;init; }
        public string Name { get; set; }
        public string Summary {  get; set; }

        [NotMapped]
        private string _description { get; set; }
        public string Description { get => _description; set
            {
                if (value == null) return;
                _description = value;
                OnPropertychanged();
            }
        }

        [NotMapped]
        private string _imageUrl { get; set; }
        public string ImageUrl {  get => _imageUrl;
            set{
                if (value == null || !File.Exists(value))
                {
                    _imageUrl = "/Icons/Default Icon.png";
                    OnPropertychanged();
                    return;
                }
                _imageUrl = value;
                OnPropertychanged();
            }}
        public ICollection<Episode> Episodes {  get; set; }
        public string XmlUrl { get; set; }
        public string Link { get; set; }
        public string DirectoryPath {  get; set; }
        [AllowNull]
        public string? GroupName {  get; set; }

        public PodcastRecord() {}
        public PodcastRecord(string name, string description, string link, ICollection<Episode>? episodes = null, string groupName = null)
        {
            Name = name;
            Description = description;
            Episodes = episodes ?? new List<Episode>();
            Link = link;
            ImageUrl = "/Icons/Default Icon.png";
            GroupName = groupName;

        }


        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertychanged([CallerMemberName]string propertyName = "")
        {
            if(propertyName == nameof(Description))
                Summary = Description.Substring(0, Description.IndexOf("."));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
