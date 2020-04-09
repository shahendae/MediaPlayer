using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using System.Windows.Threading;

namespace MediaPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool DragSlider;
        string[] songs;
        int currentSongIndex;
        public MainWindow()
        {
            InitializeComponent();
            DragSlider = false;
            currentSongIndex = -1;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            playlist.Visibility = Visibility.Hidden;
            sliderMedia.Visibility = Visibility.Hidden;
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            dispatcherTimer.Tick += Timer_Tick;
            dispatcherTimer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if((Media.Source != null) && Media.NaturalDuration.HasTimeSpan && !DragSlider)
            {
                sliderMedia.Minimum = 0;
                sliderMedia.Maximum = Media.NaturalDuration.TimeSpan.TotalSeconds;
                sliderMedia.Value = Media.Position.TotalSeconds;
                TimeText.Text =  String.Format("{0} / {1}", Media.Position.ToString(@"hh\:mm\:ss"), Media.NaturalDuration.TimeSpan.ToString(@"hh\:mm\:ss"));
            }
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            if(Media.Source != null)
                Media.Play();
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            if(Media.Source != null)
                Media.Stop();
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            if(Media.CanPause)
                Media.Pause();
        }

        private void Forward_Click(object sender, RoutedEventArgs e)
        {
            Media.Position = Media.Position.Add(TimeSpan.FromSeconds(10));
        }

        private void Backward_Click(object sender, RoutedEventArgs e)
        {
            Media.Position = Media.Position - TimeSpan.FromSeconds(10);
        }

        private void SliderMedia_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            DragSlider = true;
        }

        private void SliderMedia_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            DragSlider = false;
            Media.Position = TimeSpan.FromSeconds(sliderMedia.Value);
        }

        private void SliderVolume_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            Media.Volume = SliderVolume.Value;
        }

        private void Playlist_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Media.Source = new Uri(playlist.SelectedItem.ToString());
            this.Title = System.IO.Path.GetFileName(playlist.SelectedItem.ToString());
            Media.Play();
            currentSongIndex = playlist.SelectedIndex;
           // Media.MediaEnded += Media_MediaEnded;

        }

        private void Media_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (currentSongIndex == -1)
            {
                currentSongIndex = playlist.SelectedIndex;
            }
            currentSongIndex++;
            if(currentSongIndex < playlist.Items.Count)
            {
                Media.Source = new Uri(playlist.Items[currentSongIndex].ToString());
                this.Title = System.IO.Path.GetFileName(playlist.Items[currentSongIndex].ToString());
                playlist.SelectedIndex = currentSongIndex;
                //Media.Play();
            }

        }

        private void Volume_Click(object sender, RoutedEventArgs e)
        {
            Media.IsMuted = !Media.IsMuted;
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
                Media.Source = new Uri(openFileDialog.FileName);
            this.Title = System.IO.Path.GetFileName(openFileDialog.FileName);
            Media.Play();
        }

        private void Playlist_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All files (*.*)|*.*";
            openFileDialog.Multiselect = true;
            openFileDialog.ShowDialog();
            songs = openFileDialog.FileNames;
            foreach (string song in songs)
            {
                playlist.Items.Add(song);
            }
            if(playlist.Items.Count != 0)
                playlist.Visibility = Visibility.Visible;
        }

        private void Media_MediaOpened(object sender, RoutedEventArgs e)
        {
            sliderMedia.Visibility = Visibility.Visible;
            MediaTypeIcon();
        }
        private void MediaTypeIcon()
        {
            if (Media.HasVideo)
            {
                TaskbarIconChange.Overlay = (ImageSource)Resources["Video"];
            }
            else if (Media.HasAudio)
            {
                TaskbarIconChange.Overlay = (ImageSource)Resources["Sound"];
            }
            else
            {
                TaskbarIconChange.Overlay = null;
            }
        }
    }

}
