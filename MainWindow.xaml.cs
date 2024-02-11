using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VlogManager_Client.Models;
using VlogManager_Client.ViewModel;

namespace VlogManager_Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ClientWork ClientProcessing {  get; set; }
        public MainWindow()
        {
            ClientProcessing = new ClientWork();
            SizeToContent = SizeToContent.WidthAndHeight;
            InitializeComponent();
            ClientProcessing.DownloadClient.DownloadProgressChanged += OnDownloadProgressChanged;
        }

        private void OnDownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            DownloadProgress.Value = e.ProgressPercentage;
        }

        private void ShowDetails_Click(object sender, RoutedEventArgs e)
        {
            if (EpisodesField.RowDefinitions[1].Height.Equals(new GridLength(0, GridUnitType.Pixel)) && EpisodesField.RowDefinitions[2].Height.Equals(new GridLength(0, GridUnitType.Pixel)))
            {
                EpisodesField.RowDefinitions[0].Height = new GridLength(5, GridUnitType.Star);
                EpisodesField.RowDefinitions[1].Height = new GridLength(10, GridUnitType.Pixel);
                EpisodesField.RowDefinitions[2].Height = new GridLength(1, GridUnitType.Star);
            }
            else
                EpisodesField.RowDefinitions[2].Height = EpisodesField.RowDefinitions[1].Height = new GridLength(0, GridUnitType.Pixel);
        }

        private void OnSelectedPodcastChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count>0 && e.RemovedItems[0] != null)
                ClientProcessing?.DisableFilterForEpisodes((PodcastRecord)e.RemovedItems[0]);

        }
        private void OnSelectedEpisodeChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count > 0)
                foreach (var itemToRemove in e.RemovedItems)
                    if (itemToRemove is Episode terToRemove && ClientProcessing.EpisodesToOperate.Contains(terToRemove))
                        ClientProcessing.EpisodesToOperate.Remove(terToRemove);
            if (e.AddedItems.Count > 0)
                foreach (var itemToAdd in e.AddedItems)
                    if (itemToAdd is Episode terToAdd && !ClientProcessing.EpisodesToOperate.Contains(terToAdd))
                        ClientProcessing.EpisodesToOperate.Add(terToAdd);
        }

        private void OnOrderingStringChanged(object sender, TextChangedEventArgs e)
        {
            if (((TextBox)e.OriginalSource).Name.Contains("podcast", StringComparison.CurrentCultureIgnoreCase))
                CollectionViewSource.GetDefaultView(PodcastsView.ItemsSource).Refresh();
            else if (((TextBox)e.OriginalSource).Name.Contains("episode", StringComparison.CurrentCultureIgnoreCase))
                CollectionViewSource.GetDefaultView(EpisodesView.ItemsSource).Refresh();
        }

        private void DisablePodcastFilter_Click(object sender, RoutedEventArgs e)
        {
            ClientProcessing.DisableFilterForPodcasts();
            PodcastsField.RowDefinitions[0].Height = new GridLength(0, GridUnitType.Pixel);
        }

        private void FindPodcast_Click(object sender, RoutedEventArgs e)
        {
            PodcastsField.RowDefinitions[0].Height = new GridLength(25, GridUnitType.Pixel);
            ClientProcessing.UseFilterForPodcasts();
            PodcastOrderingString.Focus();
        }

        private void FindEpisode_Click(object sender, RoutedEventArgs e)
        {
            EpisodeViewField.RowDefinitions[1].Height = new GridLength(25, GridUnitType.Pixel);
            ClientProcessing.UseFilterForEpisodes(PodcastsView.SelectedItem as PodcastRecord);
            EpisodeOrderingString.Focus();
        }

        private void DisableEpisodeFilter_Click(object sender, RoutedEventArgs e)
        {
            ClientProcessing.DisableFilterForEpisodes(PodcastsView.SelectedItem as PodcastRecord);
            EpisodeViewField.RowDefinitions[1].Height = new GridLength(0, GridUnitType.Pixel);
        }

        private void OnAddPodcasts_Click(object sender, RoutedEventArgs e)
        {
            PodcastAddingVM addingVM = new PodcastAddingVM();
            var dlg = new PodcastAdditionWindow() { DataContext = addingVM };
            dlg.ShowDialog();
            ClientProcessing.AddRangePodcasts(addingVM.PodcastsToAdd);
        }

        private void OnGoToDownloads_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("explorer.exe", Application.Current.Resources["DataDirectory"] as string + @"\Downloads");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void OnConfigurePodcast_Click(object sender, RoutedEventArgs e)
        {
            if(PodcastsView.SelectedItem == null)
                return;
            var dlg = new PodcastConfigWindow(PodcastsView.SelectedItem as PodcastRecord);
            dlg.ShowDialog();
        }

        private void CancelDownload(object sender, RoutedEventArgs e)
        {
            ClientProcessing.CancelDownload(DownloadingEpisodes.SelectedItems as Episode ?? null);
        }
    }
}