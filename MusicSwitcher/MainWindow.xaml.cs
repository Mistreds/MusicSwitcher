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
using MusicSwitcher.ViewModel;

namespace MusicSwitcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Animation Animation { get; set; }
        public MainWindow(MainViewModel viewModel)
        {
            var primaryMonitorArea = SystemParameters.WorkArea;
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            Left = primaryMonitorArea.Right - Width - 5;
            Top = primaryMonitorArea.Bottom - Height - 5; 
            DataContext = viewModel;
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

            Animation = new Animation(this);
            //Hide();
        }

        private void Track_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Canvas.SetLeft(Track, 0);
            Canvas.SetLeft(Track1, Track1.ActualWidth + 360);
            StartTrackAnimation();
        }

        /// <summary>
        /// Запуск или остановка анимации бегущей строки песни если она размер трека слишком большой
        /// </summary>
        private void StartTrackAnimation()
        {
            Track.BeginAnimation(Canvas.LeftProperty, null);
            Track1.BeginAnimation(Canvas.LeftProperty, null);
            if (Track.ActualWidth > 230)
            {
                Animation.UpdateTrackAnimation();
                Track.BeginAnimation(Canvas.LeftProperty, Animation.StartFirstTrack);
                Track1.BeginAnimation(Canvas.LeftProperty, Animation.StartSecondTrack);

            }
        }
        /// <summary> Открытие или скрыте формы через кнопку на панели </summary>
        private void TaskbarIcon_TrayLeftMouseDown(object sender, RoutedEventArgs e)
        {
            Activate();
            if (this.IsVisible)
            {
                Topmost = false;
                BeginAnimation(TopProperty, Animation.OnHide);
            }
            else
            {
                Topmost = false;
                this.Show();
                BeginAnimation(TopProperty, Animation.OnShow);
                WinApiLib.HideFromAltTab(new System.Windows.Interop.WindowInteropHelper(this).Handle);
            }

        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
        private void Restart_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("MusicSwitcher.exe");
            Process.GetCurrentProcess().Kill();
        }
    }
}