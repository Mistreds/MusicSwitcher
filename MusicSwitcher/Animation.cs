using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace MusicSwitcher
{
    public interface IAnimation
    {
        public void UpdateTrackAnimation();

    }

    public class Animation:IAnimation

    {
    public DoubleAnimation OnShow { get; set; }
    public DoubleAnimation OnHide { get; set; }
    public DoubleAnimation StartFirstTrack { get; set; }
    public DoubleAnimation StartSecondTrack { get; set; }
    private MainWindow Window;

    public Animation(MainWindow window)
    {

        Window = window;
        var primaryMonitorArea = SystemParameters.WorkArea;
        OnShow = new DoubleAnimation();
        OnHide = new DoubleAnimation();
        OnShow.From = primaryMonitorArea.Bottom;
        OnShow.To = primaryMonitorArea.Bottom - Window.Height - 5;
        OnShow.AutoReverse = false;
        OnShow.Duration = new Duration(TimeSpan.FromMilliseconds(300d));
        OnShow.Completed += OnShow_Completed;
        OnHide = new DoubleAnimation();
        OnHide.From = primaryMonitorArea.Bottom - Window.Height - 5;
        OnHide.To = primaryMonitorArea.Bottom;
        OnHide.AutoReverse = false;
        OnHide.Duration = new Duration(TimeSpan.FromMilliseconds(300d));
        OnHide.Completed += OnHide_Completed;


    }

    public void UpdateTrackAnimation()
    {
        StartFirstTrack = new DoubleAnimation();

        StartFirstTrack.From = 0;
        StartFirstTrack.To = -Window.Track.ActualWidth - Window.Track.ActualWidth * 0.25;
        StartFirstTrack.Completed += StartFistTrack_Completed;
        StartFirstTrack.Duration = new Duration(TimeSpan.FromSeconds(Window.Track.Text.Length * 0.25));
        StartSecondTrack = new DoubleAnimation();
        StartSecondTrack.From = Window.Track1.ActualWidth + Window.Track.ActualWidth * 0.25;
        StartSecondTrack.To = -Window.Track1.ActualWidth - Window.Track.ActualWidth * 0.25;
        StartSecondTrack.RepeatBehavior = RepeatBehavior.Forever;
        StartSecondTrack.Duration = new Duration(TimeSpan.FromSeconds(Window.Track.Text.Length * 0.50));

    }

    private void StartFistTrack_Completed(object sender, EventArgs e)
    {

        Window.Track.BeginAnimation(Canvas.LeftProperty, StartSecondTrack);
    }

    private void OnHide_Completed(object sender, EventArgs e)
    {
        Window.Hide();

    }

    private void OnShow_Completed(object sender, EventArgs e)
    {
        Window.Topmost = true;
    }
    }
}
