using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VlogManager_Client.Models;

namespace VlogManager_Client.DataBase
{
    public class PodcastsContext : DbContext
    {
        public DbSet<PodcastRecord> Podcasts { get; set; }
        public DbSet<Episode> Episodes { get; set;}

        public PodcastsContext()
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=" + Application.Current.Resources["DataDirectory"] + @"\Database.db");
        }

        public void AddPodcast(PodcastRecord? record)
        {
            if (record == null || Podcasts.FirstOrDefault(p => p.Name.Equals(record.Name)) != null) return;
            Podcasts.Add(record);
            SaveChanges();
        }
        public void AddRangePodcast(ICollection<PodcastRecord>? records)
        {
            if (records == null) return;
            foreach (var record in records)
               if( Podcasts.FirstOrDefault(p => p.Name.Equals(record.Name)) != null)
                    records.Remove(record);
            Podcasts.AddRange(records);
            SaveChanges();
        }
        public void RemovePodcast(int id)
        {
            PodcastRecord? podToDelete = Podcasts.FirstOrDefault(p => p.ID == id);
            if (podToDelete == null) return;
            Podcasts.Remove(podToDelete);
            SaveChanges();
        }
        public void RemoveRangePodcasts(ICollection<PodcastRecord>? recordsToDelete)
        {
            if (recordsToDelete == null) return;
            Podcasts.RemoveRange(recordsToDelete);
            SaveChanges();
        }


        public void AddEpisode(Episode episode, int podcastId)
        {
            if(episode == null) return;
            PodcastRecord? recordToUpdate = Podcasts.FirstOrDefault(p => p.ID == podcastId);
            if (recordToUpdate == null) return;
            recordToUpdate.Episodes.Add(episode);
            SaveChanges();
        }
        public void AddRangeEpisodes(ICollection<Episode>? episodes, int podcastId)
        {
            if (episodes == null) return;
            PodcastRecord? recordToUpdate = Podcasts.FirstOrDefault(p => p.ID == podcastId);
            if(recordToUpdate == null) return;
            foreach (var episode in episodes)
            {
                recordToUpdate.Episodes.Add(episode);
                SaveChanges();
            }
        }
        public void RemoveEpisode(int id)
        {
            Episode episodeToDelete = Episodes.FirstOrDefault(ep => ep.ID == id);
            if (episodeToDelete == null) return;
            Episodes.Remove(episodeToDelete);
            SaveChanges();
        }
        public void RemoveRangeEpisodes(ICollection<Episode>? episodesToDelete)
        {
            if (episodesToDelete == null) return;
            Episodes.RemoveRange(episodesToDelete);
            SaveChanges();
        }
    }
}
