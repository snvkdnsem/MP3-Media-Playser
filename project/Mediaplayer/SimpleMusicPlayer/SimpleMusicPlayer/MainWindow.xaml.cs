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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Win32;

namespace SimpleMusicPlayer
{
    public partial class MainWindow : Window
    {
        ListBoxItem currentTrack; // 변수선언
        ListBoxItem PreviousTrack;
        DispatcherTimer timer;

        bool isDragging; // false or true

        public MainWindow()
        {
            InitializeComponent();

            currentTrack = null;
            //PreviousTrack = null;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1); // FromSeconds(1)의 의미 및 1은 무엇인가?
            timer.Tick += new EventHandler(timer_Tick); // timer_Tick을 왜 여기에 다시 넣나?

            isDragging = false; // 왜 여기서 false인가?
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (!isDragging) // musicslider 드래그에 사용
            {
                musicSlider.Value = media.Position.TotalSeconds;
            }
        }

        private void Previous(object sender, RoutedEventArgs e)
        {
            MoveToPrecedentTrack();
        }

        private void Pause(object sender, RoutedEventArgs e)
        {
            media.Pause();
        }

        private void Play(object sender, RoutedEventArgs e)
        {
            PlayTrack();
        }

        private void Stop(object sender, RoutedEventArgs e)
        {
            media.Stop();
        }

        private void Next(object sender, RoutedEventArgs e)
        {
            MoveToNextTrack();
        }

        private void Repeat(object sender, RoutedEventArgs e)
        {
            
        }

        // MediaElement에서 음량을 조절하는 slider를 binding하여 조절한다.
        private void VolumeOnOff(object sender, RoutedEventArgs e)
        {
            if (volumeControl.Value != 0) //media는 MediaElement의 name
            {
                volumeControl.Value = 0;
            }
            else
            {
                volumeControl.Value = 0.5;
            }
        }

        // 파일추가 코딩
        private void FileAdd(object sender, RoutedEventArgs e)
        {
            // OpenFileDialog은 using Micosoft.Win32; 가 필요하다
            OpenFileDialog myDialog = new OpenFileDialog();
            myDialog.Filter = "Image Files(*.mp3;*.wav;*.wma)|*.mp3;*.wav;*.wma|All files (*.*)|*.*";
            myDialog.CheckFileExists = true; // on,off로 생각하면 된다.
            myDialog.Multiselect = true;

            if (myDialog.ShowDialog() == true) // ShowDialog 예약어?
            {
                foreach(string file in myDialog.FileNames) // FileNames 예약어?
                {
                    ListBoxItem musicList = new ListBoxItem();
                    musicList.Content = System.IO.Path.GetFileNameWithoutExtension(file);
                    musicList.Tag = file;
                    lstFiles.Items.Add(musicList); // ListBoxItem에 내가 선택한 항목을 추가한다.
                }
            }
        }

        private void PlayTrack()
        {
            if (lstFiles.SelectedItem != currentTrack)
            {
                if (currentTrack != null)
                {
                    PreviousTrack = currentTrack;
                }
                currentTrack = (ListBoxItem)lstFiles.SelectedItem;
                media.Source = new Uri(currentTrack.Tag.ToString());
                musicSlider.Value = 0;
                media.Play();
            }
            else
            {
                media.Play();
            }

            TagLib.File file = TagLib.File.Create(currentTrack.Tag.ToString());

            string Album = file.Tag.Album;
            string Title = file.Tag.Title;
            string[] Singer = file.Tag.Artists;
            TimeSpan RunTime = file.Properties.Duration;

            album.Content = Album;
            title.Content = Title;
            singer.Content = Singer[0];
            runTime.Content = RunTime.Minutes + ":" + RunTime.Seconds;

            ////////////////////앨범 아트//////////////////////
            TagLib.IPicture album_cover = file.Tag.Pictures[0];
            System.IO.MemoryStream memoryStream = new System.IO.MemoryStream(album_cover.Data.Data);
            BitmapImage cover = new BitmapImage();
            cover.BeginInit();
            cover.StreamSource = memoryStream;
            cover.EndInit();

            albumart.Source = cover;
        }

        private void PlayMusic(object sender, RoutedEventArgs e)
        {
            if (media.NaturalDuration.HasTimeSpan)
            {
                TimeSpan ts = media.NaturalDuration.TimeSpan;
                musicSlider.Maximum = ts.TotalSeconds;
                musicSlider.SmallChange = 1;
            }
            timer.Start();
        }

        private void EndMusic(object sender, RoutedEventArgs e)
        {
            musicSlider.Value = 0; // musicSlider의 값이 0이면 아래 코드를 실행
            MoveToNextTrack(); // 다음곡 재생
        }

        // 다음곡 재생 메소드
        private void MoveToNextTrack()
        {
            if (lstFiles.Items.IndexOf(currentTrack) < lstFiles.Items.Count - 1)
            {
                lstFiles.SelectedIndex = lstFiles.Items.IndexOf(currentTrack) + 1;
                PlayTrack();
                return;
            }
            else if (lstFiles.Items.IndexOf(currentTrack) == lstFiles.Items.Count - 1)
            {
                lstFiles.SelectedIndex = 0;
                PlayTrack();
                return;
            }
        }

        // 이전곡 재생 메소드
        private void MoveToPrecedentTrack()
        {
            if (lstFiles.Items.IndexOf(currentTrack) > 0)
            {
                lstFiles.SelectedIndex = lstFiles.Items.IndexOf(currentTrack) - 1;
                PlayTrack();
            }
            else if (lstFiles.Items.IndexOf(currentTrack) == 0)
            {
                lstFiles.SelectedIndex = lstFiles.Items.Count - 1;
                PlayTrack();
            }
        }
    }
}
