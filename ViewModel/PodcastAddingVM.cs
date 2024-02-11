using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using VlogManager_Client.Comands;
using VlogManager_Client.Models;

namespace VlogManager_Client.ViewModel
{
    public class PodcastAddingVM
    {


        public List<PodcastRecord> SuggestedPodcasts { get; set; } = new List<PodcastRecord>();
        public List<PodcastRecord> SignedRecords { get; set; } = new List<PodcastRecord> ();

        public List<PodcastRecord> PodcastsToAdd { get; private set; } = new List<PodcastRecord> () { };
        public PodcastAddingVM()
        {
            List<string> suggests = Application.Current.Resources["SuggestedPodcasts"] as List<string> ?? new List<string>();
            foreach (string suggest in suggests)
            {
                var pod = GetPodcastInfo(suggest);
                if(pod != null) SuggestedPodcasts.Add(pod);
            }
        }

        private PodcastRecord? GetPodcastInfo(string xmlUrl)
        {
            PodcastRecord podRec;
            XmlDocument doc = new();
            try
            {
                doc.Load(XmlReader.Create(xmlUrl));
                var root = doc.GetElementsByTagName("channel")[0];
                podRec = new(root["title"]?.InnerText, RSSProcessing.ConvertFromHtmlToPlain(root["description"]?.InnerText), root["link"]?.InnerText) { XmlUrl = xmlUrl };
                return podRec;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Incorrect reference or {ex.Message}");
                return null;
            }
        }

        public void GetPodcast(string xmlUrl)
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(XmlReader.Create(xmlUrl));
                XmlElement root = doc.DocumentElement;
                if (root.Name == "opml")
                    PodcastsToAdd.AddRange(RSSProcessing.WorkWithOpml(doc));
                else if (root.Name == "rss")
                {
                    PodcastRecord rec = RSSProcessing.WorkWithRss(doc);
                    if (rec != null)
                    {
                        rec.XmlUrl = xmlUrl;
                        PodcastsToAdd.Add(rec);
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Invalid reference or {ex.Message}");
                return;
            }
        }
        
        public void GetSuggests()
        {
            foreach(var pod in SignedRecords)
            {
                GetPodcast(pod.XmlUrl);
            }
        }
    }
}
