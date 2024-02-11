using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml;
using VlogManager_Client.Comands;
using VlogManager_Client.DataBase;
using VlogManager_Client.Models;

namespace VlogManager_Client.ViewModel
{
    public class ClientWork
    {

        //String to order podcasts
        public string PodcastOrderingString { get; set; } = string.Empty;

        //string to order episodes
        public string EpisodeOrderingString { get; set; } = string.Empty;

        //client to download files
        public WebClient DownloadClient { get; private set; } = new WebClient();

        //database context
        public PodcastsContext Context { get; set; } = new PodcastsContext();

        //selected by user episodes
        public List<Episode> EpisodesToOperate { get; private set; } = new List<Episode>();


        //existing podcasts
        public ObservableCollection<PodcastRecord> Podcasts { get; set; } = new ObservableCollection<PodcastRecord>();

        //Episodes to download
        public ObservableCollection<Episode> DownLoadQueue { get; set; } = new ObservableCollection<Episode>();

        //variables, which holds the delete and update commands of podcasts
        private RelayComand<ICollection<PodcastRecord>> _updateRangePodcastsCommand = null;
        private RelayComand<ICollection<PodcastRecord>> _deleteRangePodcastsCommand = null;
        private RelayComand<PodcastRecord> _deletePodcastCommand = null;
        private RelayComand<PodcastRecord> _updatePodcastCommand = null;

        //variables, which holds the delete and update commands of episodes
        private RelayComand<ICollection<Episode>> _downloadRangeEpisodesCommand = null;
        private RelayComand<ICollection<Episode>> _deleteRangeEpisodesCommand = null;
        private RelayComand<ICollection<Episode>> _markAsOldCommand = null;

        ////Commands, which work with delete and update operates of podcasts
        public RelayComand<ICollection<PodcastRecord>> UpdateRangePodcastsCommand => _updateRangePodcastsCommand ?? (_updateRangePodcastsCommand = new RelayComand<ICollection<PodcastRecord>>(UpdateRangePodcasts, (ICollection<PodcastRecord> podcasts) => podcasts != null));
        public RelayComand<ICollection<PodcastRecord>> DeleteRangePodcastsCommand => _deleteRangePodcastsCommand ?? (_deleteRangePodcastsCommand = new RelayComand<ICollection<PodcastRecord>>(DeleteRangePodcasts, (ICollection<PodcastRecord> colPod) => colPod != null));
        public RelayComand<PodcastRecord> DeletePodcastCommand => _deletePodcastCommand ?? (_deletePodcastCommand = new RelayComand<PodcastRecord>(DeletePodcast, (PodcastRecord record) => record!=null));
        public RelayComand<PodcastRecord> UpdatePodcastCommand => _updatePodcastCommand ?? (_updatePodcastCommand = new RelayComand<PodcastRecord> (UpdatePodcast, (PodcastRecord record) => record != null));

        ////Commands, which work with delete and update operates of episodes
        public RelayComand<ICollection<Episode>> MarkAsOldCommand => _markAsOldCommand ?? (_markAsOldCommand = new RelayComand<ICollection<Episode>>(MarkAsOld, (ICollection<Episode> episodes) => episodes != null));
        public RelayComand<ICollection<Episode>> DownloadRangeEpisodesCommand => _downloadRangeEpisodesCommand ?? (_downloadRangeEpisodesCommand = new RelayComand<ICollection<Episode>>(DownloadRangeEpisodes, (ICollection<Episode> episodes) => episodes != null));
        public RelayComand<ICollection<Episode>> DeleteRangeEpisodesCommand => _deleteRangeEpisodesCommand ?? (_deleteRangeEpisodesCommand = new RelayComand<ICollection<Episode>>(DeleteRangeEpisodes, (ICollection<Episode> colPEp) => colPEp != null));

        public ClientWork()
        {
            object lockDownloads = new object();
            BindingOperations.EnableCollectionSynchronization(DownLoadQueue, lockDownloads);
            DownloadClient.DownloadFileCompleted += OnDownloadFileCompleted;
            Podcasts = Context.Podcasts.Local.ToObservableCollection();
        }

        ~ClientWork(){
            DownloadClient.Dispose();
        }

        private void OnDownloadFileCompleted(object? sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (DownLoadQueue.Count > 0)
            {
                DownLoadQueue.RemoveAt(0);
                if(DownLoadQueue.Count > 0)
                    ProcessDownLoad(DownLoadQueue[0]);
            }
        }

        /// <summary>
        /// Gets all required information about selected podcast
        /// </summary>
        /// <param name="xmlUrl"> Rss link of podcast</param>
        public void GetPodcasts(string xmlUrl)
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(XmlReader.Create(xmlUrl));
                XmlElement root = doc.DocumentElement;
                if (root.Name == "opml")
                    Context.AddRangePodcast(RSSProcessing.WorkWithOpml(doc));
                else if (root.Name == "rss")
                {
                    PodcastRecord rec = RSSProcessing.WorkWithRss(doc);
                    if (rec != null)
                    {
                        rec.XmlUrl = xmlUrl;
                        Context.AddPodcast(rec);
                    }
                }
                    
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Invalid reference or {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Adds podcast to existing ones
        /// </summary>
        /// <param name="podToAdd">Podcast, which should be added</param>
        public void AddPodcast(PodcastRecord? podToAdd)
        {
            Context.AddPodcast(podToAdd);
        }

        /// <summary>
        /// Adds podcasts collection to existing ones
        /// </summary>
        /// <param name="podsToAdd">Some podcasts, which should be added</param>
        public void AddRangePodcasts(ICollection<PodcastRecord>? podsToAdd)
        {
            Context.AddRangePodcast(podsToAdd);
        }

        /// <summary>
        /// Finds out if there is any new episodes in existing podcast and then adds them
        /// </summary>
        /// <param name="podcastToUpdate">Existing podcast, which should be updated</param>
        public void UpdatePodcast(PodcastRecord podcastToUpdate)
        {
            if (podcastToUpdate == null) return;
            PodcastRecord? updatedRecord = RSSProcessing.WorkWithRss(podcastToUpdate.XmlUrl);
            if (updatedRecord == null) return;
            List<Episode> newEpisodes = CompareByName(podcastToUpdate.Episodes, updatedRecord.Episodes).ToList();
            if (newEpisodes.Count == 0)
                return;
            Context.AddRangeEpisodes(newEpisodes, podcastToUpdate.ID);

        }

        /// <summary>
        /// Finds out if there is any new episodes in a range of existing podcasts and then adds them
        /// </summary>
        /// <param name="podcastsToUpdate">Some existing podcasts, which should be updated</param>
        public void UpdateRangePodcasts(ICollection<PodcastRecord> podcastsToUpdate)
        {
            foreach (var podcast in podcastsToUpdate)
            {
                UpdatePodcast(podcast);
            }
        }

        /// <summary>
        ///  Deletes specific episode from the existing podcasts
        /// </summary>
        /// <param name="recordToDelete">selected episode, which should be deleted</param>
        public void DeletePodcast(PodcastRecord recordToDelete)
        {
            if (recordToDelete == null) return;
            if(!Podcasts.Contains(recordToDelete)) return;
            Context.RemovePodcast(recordToDelete.ID);
        }

        /// <summary>
        ///  Deletes a collection of podcasts from the existing podcasts
        /// </summary>
        /// <param name="recordsToDelete">selected episodes, which should be deleted</param>
        public void DeleteRangePodcasts(ICollection<PodcastRecord> recordsToDelete)
        {
            if(recordsToDelete == null || recordsToDelete.Count == 0) return;
            Context.RemoveRangePodcasts(recordsToDelete.ToList());
        }

        /// <summary>
        /// Deletes specific episode from the existing episodes
        /// </summary>
        /// <param name="episodeToDelete">selected episode, which should be deleted</param
        public void DeleteEpisode(Episode episodeToDelete)
        {
            if (episodeToDelete == null) return;
            Context.RemoveEpisode(episodeToDelete.ID);
        }

        /// <summary>
        ///  Deletes a collection of episodes from the existing episode
        /// </summary>
        /// <param name="episodesToDelete"> selected episodes, which should be deleted</param>
        public void DeleteRangeEpisodes(ICollection<Episode> episodesToDelete)
        {
            if(episodesToDelete == null) return;
            Context.RemoveRangeEpisodes(episodesToDelete.ToList());
        }

        /// <summary>
        /// Marks episode as old one, actually useless function yet
        /// </summary>
        /// <param name="episodes"></param>
        public void MarkAsOld(ICollection<Episode> episodes)
        {
            if (episodes.Count() == 0) return;
            bool isNew = episodes.First().IsNew = !episodes.First().IsNew;
            foreach (Episode episode in episodes)
                episode.IsNew = isNew;
        }

        /// <summary>
        /// Begins to download episode
        /// </summary>
        /// <param name="episodeToDownload">Episode, which should be downloaded</param>
        public void DownloadEpisode(Episode episodeToDownload)
        {
            DownLoadQueue.Add(episodeToDownload);
            if(!DownloadClient.IsBusy)
                ProcessDownLoad(episodeToDownload);
        }
        /// <summary>
        ///  Begins to download episodes collection
        /// </summary>
        /// <param name="episodesToDownload">Collection of episodes to download</param>
        public void DownloadRangeEpisodes(ICollection<Episode> episodesToDownload)
        {
            foreach(var ep in  episodesToDownload)
                DownLoadQueue.Add(ep);
            if (!DownloadClient.IsBusy)
                ProcessDownLoad(DownLoadQueue[0]);

        }

        /// <summary>
        /// Cancels the downloading of a specific episode
        /// </summary>
        /// <param name="episodeToCancel">episode, which downloading should be cancelled</param>
        public void CancelDownload(Episode? episodeToCancel)
        {
            if(episodeToCancel == null) return;
            DownLoadQueue.Remove(episodeToCancel);
            if (episodeToCancel.Name.Equals(DownLoadQueue[0].Name))
            {
                DownloadClient.CancelAsync();
                File.Delete(Application.Current.Resources["DataDirectory"] as string + $@"\Downloads\{episodeToCancel.PodcastRecord.Name}\{episodeToCancel.Name}\{(episodeToCancel.Format == EpisodeType.Audio ? ".mp3" : ".mp4")}");
            }
        }

        /// <summary>
        /// Starts the episodes downloading
        /// </summary>
        /// <param name="episodeToDownload"> Episode to download</param>
        private void ProcessDownLoad(Episode episodeToDownload)
        {
            if (episodeToDownload is not null)
            {
                try
                {
                string filePath = Application.Current.Resources["DataDirectory"] as string + $@"\Downloads\{episodeToDownload.PodcastRecord.Name}\{(episodeToDownload.Format == EpisodeType.Audio ? ".mp3" : ".mp4")}";
                if (string.IsNullOrEmpty(episodeToDownload.DownloadUri)) return;
                DownloadClient.DownloadFileAsync(new Uri(episodeToDownload.DownloadUri), filePath);
                }
                catch (Exception)
                {
                    MessageBox.Show("There has been a problem while trying to download episode, please, check if the source is right", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    DownLoadQueue.RemoveAt(0);
                    ProcessDownLoad(DownLoadQueue[0]);
                }
            }
        }

        //Applies filter of podcasts or episodes
        public void UseFilterForPodcasts()
        {
            var collection = CollectionViewSource.GetDefaultView(Podcasts);
            if (collection.Filter != null) return;
            collection.Filter = PodcastNameDescriptionFilter;
        }
        public void UseFilterForEpisodes(PodcastRecord? podcastRecord)
        {
            if(podcastRecord == null) return;
            var collection = CollectionViewSource.GetDefaultView(podcastRecord.Episodes);
            if (collection.Filter != null) return;
            collection.Filter = EpisodeNameDescriptionFilter;
        }

        //Disables filter of podcasts or episodes
        public void DisableFilterForPodcasts()
        {
            var collection = CollectionViewSource.GetDefaultView(Podcasts);
            if (collection.Filter == null) return;
            collection.Filter = null;
        }
        public void DisableFilterForEpisodes(PodcastRecord? podcastRecord)
        {
            if (podcastRecord == null) return;
            var collection = CollectionViewSource.GetDefaultView(podcastRecord?.Episodes);
            if(collection.Filter == null) return;
            collection.Filter = null;
        }

        /// <summary>
        /// filters the episodes and finds ones, which contains EpisodeOrderingString in their name or description
        /// </summary>
        /// <param name="episode">Episode record to filter</param>
        /// <returns>if record contains EpisodeOrderingString in its name or description </returns>
        private bool EpisodeNameDescriptionFilter(object episode)
        {
            if (episode == null || !(episode is PodcastRecord)) return false;
            if (((PodcastRecord)episode).Name.Contains(EpisodeOrderingString, StringComparison.OrdinalIgnoreCase) || ((PodcastRecord)episode).Description.Contains(EpisodeOrderingString, StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }
        /// <summary>
        /// filters the podcasts and finds ones, which contains PodcastOrderingString in their name or description
        /// </summary>
        /// <param name="podcast">Podcast record to filter</param>
        /// <returns>if record contains EpisodeOrderingString in its name or description</returns>
        private bool PodcastNameDescriptionFilter(object podcast)
        {
            if (podcast == null || !(podcast is PodcastRecord)) return false;
            if(((PodcastRecord)podcast).Name.Contains(PodcastOrderingString, StringComparison.OrdinalIgnoreCase) || ((PodcastRecord)podcast).Description.Contains(PodcastOrderingString, StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }

        /// <summary>
        /// Compares podcasts by name with existing ones
        /// </summary>
        /// <param name="source">Existing podcasts, which you should compare</param>
        /// <param name="episodesToCompare">New podcasts, to which we should compare</param>
        /// <returns>collection, that contains difference between podcast collections</returns>
        private ICollection<Episode>? CompareByName(ICollection<Episode> source, ICollection<Episode> episodesToCompare)
        {
            if (episodesToCompare == null || source == null || episodesToCompare.Count == 0) return null;
            List<Episode> result = new List<Episode>();
            foreach(Episode episode in episodesToCompare)
                if(source.FirstOrDefault(ep => ep.Name == episode.Name || ep.Description == ep.Description) != null) result.Add(episode);
            return result;
        }
    }
}