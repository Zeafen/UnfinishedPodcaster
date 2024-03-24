using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VlogManager_Client.Models;

namespace VlogManager_Client.ViewModel
{
    /// <summary>
    /// Interaction logic for PodcastConfigWindow.xaml
    /// </summary>
    public partial class PodcastConfigWindow : Window
    {
        public PodcastConfigWindow(PodcastRecord selectedPod)
        {
            DataContext = new PodcastConfiguringVM(selectedPod);
            InitializeComponent();
        }

        private void OnPodcastLink_Click(object sender, MouseButtonEventArgs e)
        {
            Process.Start(((TextBlock)sender).Text);
        }

        private void OnStopEditingName_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button b)
            {
                Storyboard sb = FindResource("ChangeColumnsToView") as Storyboard;
                sb?.Begin();
            }
        }

        private void OnBeginEditing_Click(object sender, RoutedEventArgs e)
        {
            Storyboard sb = FindResource("ChangeColumnsToEdit") as Storyboard;
            sb?.Begin();
        }

        private void OnGoToDirectory_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", (DataContext as PodcastConfiguringVM)?.RecordToConfig.DirectoryPath);
        }

        private void OnChangeIcon_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as PodcastConfiguringVM).SelectAnotherImage();
        }

        private void OnGroupChanged(object sender, SelectionChangedEventArgs e)
        {
            var group = (sender as ComboBox).SelectedItem as string;
            if (!string.IsNullOrEmpty(group))
                (DataContext as PodcastConfiguringVM)?.ChangePodcastGroup(group);
        }
    }
}
