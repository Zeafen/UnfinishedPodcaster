using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using VlogManager_Client.Models;

namespace VlogManager_Client.Comands
{
    static class RSSProcessing
    {
        private static Regex TimeRegex = new Regex(@"^\d{2}:\d{2}:\d{2}\b");
        private static Regex MinSecRegex = new Regex(@"^\d{2}:\d{2}\b");
        private static Regex SecRegex = new Regex(@"^[0-7]{1}[0-9]{4}\b|^[8]{1}[0-5]{1}[0-9]{3}\b|^[8]{1}[6]{1}[0-3]{1}[0-9]{2}\b");


        public static List<PodcastRecord>? WorkWithOpml(XmlDocument doc)
        {
            try
            {
                List<PodcastRecord> records = new List<PodcastRecord>();
                XmlElement mainRoot = doc.DocumentElement;
                foreach (XmlNode outline in mainRoot["body"]?.SelectNodes("outline"))
                {
                    PodcastRecord? rec = WorkWithRss(outline?.Attributes["xmlUrl"]?.InnerText);
                    if (rec != null)
                        records.Add(rec);
                }
                return records;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static PodcastRecord? WorkWithRss(XmlDocument doc)
        {
            try
            {
                var root = doc.GetElementsByTagName("channel")[0];
                PodcastRecord record = new(root["title"]?.InnerText, ConvertFromHtmlToPlain(root["description"]?.InnerText), root["link"]?.InnerText, new ObservableCollection<Episode>());
                foreach (XmlNode item in root.SelectNodes("item"))
                {
                    if (item == null) break;
                    record.Episodes.Add(new Episode(item["title"]?.InnerText, ConvertFromHtmlToPlain(item["description"]?.InnerText), (item["enclosure"]?.Attributes["type"]?.InnerText.Contains("audio") ?? true) ? EpisodeType.Audio : EpisodeType.Video, Convert.ToDateTime(item["pubDate"]?.InnerText), ConvertFromStrToTimeSpan(item["itunes:duration"]?.InnerText ?? "00:00:00"), item["enclosure"]?.Attributes["url"]?.InnerText));
                }
                record.ImageUrl = DownLoadImage(root["itunes:image"]?.Attributes["href"]?.InnerText ?? string.Empty, record.Name);
                record.DirectoryPath = Application.Current.Resources["DataDirectory"] as string + @$"\{record.Name}";
                return record;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
        public static PodcastRecord? WorkWithRss(string xmlUrl)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(XmlReader.Create(xmlUrl));
                var root = doc.GetElementsByTagName("channel")[0];
                PodcastRecord record = new(root["title"]?.InnerText, ConvertFromHtmlToPlain(root["description"]?.InnerText), root["link"]?.InnerText, new ObservableCollection<Episode>()) { XmlUrl = xmlUrl };
                foreach (XmlNode item in root.SelectNodes("item"))
                {
                    record.Episodes.Add(new Episode(item["title"]?.InnerText, ConvertFromHtmlToPlain(item["description"]?.InnerText), (item["enclosure"]?.Attributes["type"]?.InnerText.Contains("audio") ?? true) ? EpisodeType.Audio : EpisodeType.Video, Convert.ToDateTime(item["pubDate"]?.InnerText), ConvertFromStrToTimeSpan(item["itunes:duration"]?.InnerText ?? "00:00:00"), item["enclosure"]?.Attributes["url"]?.InnerText));
                }
                record.ImageUrl = DownLoadImage(root["itunes:image"]?.Attributes["href"]?.InnerText ?? string.Empty, record.Name);
                record.DirectoryPath = Application.Current.Resources["DataDirectory"] as string + @$"\{record.Name}";
                return record;
            }
            catch (Exception)
            {

                return null;
            }
        }

        public static string ConvertFromHtmlToPlain(string htmlText)
        {
            if (htmlText == null) return "";
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(htmlText);

            string text = htmlDocument.DocumentNode.InnerText;
            return text;
        }

        private static TimeSpan ConvertFromStrToTimeSpan(string time)
        {
            if (TimeRegex.IsMatch(time)) return TimeSpan.Parse(time);
            else if (MinSecRegex.IsMatch(time)) return TimeSpan.Parse("00:" + time);
            else if (SecRegex.IsMatch(time)) return TimeSpan.FromSeconds(Convert.ToDouble(time));
            else return TimeSpan.Parse("00:00:00");
        }

        private static string DownLoadImage(string ImageUrl, string podcastName)
        {
            if (string.IsNullOrEmpty(ImageUrl)) return string.Empty;
            try
            {
                string path = (Application.Current.Resources["DataDirectory"] as string) + @$"\DownLoads\{podcastName}";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                path += @"\logo.png";
                using (var client = new WebClient())
                {
                    client.DownloadFile(ImageUrl, path);
                }
                return path;
            }
            catch(Exception ex)
            {
                return string.Empty;
            }
        }
    }
}
