using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VlogManager_Client.Models;
using VlogManager_Client.ViewModel;

namespace VlogManager_Client
{
    /// <summary>
    /// Interaction logic for PodcastAdditionWindow.xaml
    /// </summary>
    public partial class PodcastAdditionWindow : Window
    {
        public PodcastAdditionWindow()
        {
            InitializeComponent();
        }

        private void OnPodcastSuggestions_CheckedChanged(object sender, RoutedEventArgs e)
        {
            var ctx = (this.DataContext as PodcastAddingVM);
            CheckBox OriginalSender = (CheckBox)sender;
            if(OriginalSender != null)
            {
                if (OriginalSender != null && OriginalSender.IsChecked == true)
                {
                    if (OriginalSender.DataContext is PodcastRecord recToAdd)
                    {
                        ctx.SignedRecords.Add(recToAdd);
                    }
                }
                else if (OriginalSender != null && OriginalSender.IsChecked == false)
                {
                    if (OriginalSender.DataContext is PodcastRecord recToRemove)
                    {
                        ctx.SignedRecords.Remove(recToRemove);
                    }
                }
            }
        }

        private void AddPodcasts(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).Name.Contains("reference", StringComparison.OrdinalIgnoreCase))
            {
                (this.DataContext as PodcastAddingVM).GetPodcast(ReferenceInput.Text);
            }
            else if (((Button)sender).Name.Contains("suggests", StringComparison.OrdinalIgnoreCase))
            {
                (DataContext as PodcastAddingVM).GetSuggests();
            }
            DialogResult = true;
        }
    }
}
