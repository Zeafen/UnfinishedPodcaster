using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VlogManager_Client.Models
{
    public class Episode
    {
        [AutoIncrement]
        public int ID { get; init; }
        public string Name { get; set; }
        public string Summary { get; set; }
        [NotMapped]
        private string _description {  get; set; }
        public string Description { get => _description; set
            {
                if (value == null) return;
                _description = value;
                if(Description.IndexOf('.') > -1)
                    Summary = Description.Substring(0, Description.IndexOf(".")) + "...";
                else Summary = Description.Substring(0, 20);
            }
        }
        public EpisodeType Format { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime DateReleased { get; set; }
        public bool IsNew { get; set; }
        public readonly string DownloadUri;

        public int Podcast_Id { get; set; }
        public PodcastRecord PodcastRecord { get; set; }

        public Episode() { }
        public Episode(string name, string description, EpisodeType format, DateTime dateReleased, TimeSpan duration, string downloadUri)
        {
            Name = name;
            Description = description;
            Format = format;
            DateReleased = dateReleased;
            DownloadUri = downloadUri;
            IsNew = true;
            Duration = duration;
        }
    }
}
