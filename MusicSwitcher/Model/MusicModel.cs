using System.Drawing;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Hardcodet.Wpf.TaskbarNotification;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.IO;
using Brush = System.Windows.Media.Brush;
using static System.Net.Mime.MediaTypeNames;
using Color = System.Windows.Media.Color;
using System.Drawing.Imaging;

namespace MusicSwitcher.Model;

public class MusicModel:ReactiveObject
{
    /// <summary>
    /// Название песни
    /// </summary>
    [Reactive]
    public string SingName { get; set; } = "";
    /// <summary>
    /// Название альбома
    /// </summary>

    [Reactive] public string AlbumName { get; set; } = "";
    /// <summary>
    /// Название исполнителя
    /// </summary>
    [Reactive] public string SingerName { get; set; } = "";

    [Reactive] public string Status { get; set; } = "Stopped";
    [Reactive] public BitmapImage Icon { get; set; } = new BitmapImage(new Uri("pack://application:,,,/MusicSwitcher;component/Resources/play-button.ico"));

    [Reactive] public object Content { get; set; } = new View.Start();

    [Reactive] public BitmapImage AlbumImage { get; set; }
    [Reactive] public Brush WindowColor { get; set; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFAABBCC")!;
    private static readonly BrushConverter Converter = new ();
    public async Task UpdateMusic(string singName, string albumName, string singerName, string status, byte[] file)
    {
        await Task.Run(async () =>
        {

            this.SingName=singName;
            this.AlbumName=albumName;
            this.SingerName=singerName;
            this.Status = status;
            await App.Current.Dispatcher.Invoke(async delegate
            {
                this.AlbumImage = ConvertImage.ToBitmapImage(file);
                WindowColor= await ConvertImage.GetColor(AlbumImage);
                if (status == "Paused")
                {
                    Icon = play;
                    Content = new View.Start();
                }

                if (status == "Playing")
                {
                    Icon = pause;
                    Content = new View.Stop();

                }
            });
        });
    }

    private BitmapImage play = new BitmapImage(new Uri("pack://application:,,,/MusicSwitcher;component/Resources/play-button.ico"));

    private BitmapImage pause = new BitmapImage(new Uri("pack://application:,,,/MusicSwitcher;component/Resources/video-pause-button.ico"));

   

}

public static class ConvertImage
{
    public static BitmapImage ToBitmapImage(byte[] array)//Делаем из потока байтов картинку
    {

        using var ms = new System.IO.MemoryStream(array);
        try
        {
            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad; // here
            image.StreamSource = ms;
            image.Rotation = Rotation.Rotate0;
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
            image.EndInit();
            return image;
        }
        catch
        {
            return null;
        }



    }
    private static Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
    {
        // BitmapImage bitmapImage = new BitmapImage(new Uri("../Images/test.png", UriKind.Relative));

        using (MemoryStream outStream = new MemoryStream())
        {
            BitmapEncoder enc = new BmpBitmapEncoder();
            enc.Frames.Add(BitmapFrame.Create(bitmapImage));
            enc.Save(outStream);
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

            return new Bitmap(bitmap);
        }
    }
    public static async Task<System.Windows.Media.SolidColorBrush> GetColor(BitmapImage image)
    {
        var bitmap = BitmapImage2Bitmap(image);
        var pixh = image.PixelHeight;
        var pixw = image.PixelWidth;
        System.Windows.Media.Color color = new System.Windows.Media.Color();
        await Task.Run(() => {

            int r = 0;
            int g = 0;
            int b = 0;
            for (int i = 0; i < pixw; i++)
            {
                for (int j = 0; j < pixh; j++)
                {
                    r += bitmap.GetPixel(i, j).R;
                    g += bitmap.GetPixel(i, j).G;
                    b += bitmap.GetPixel(i, j).B;
                }
            }

            r = r / (pixh * pixw);
            g = g / (pixh * pixw);
            b = b / (pixh * pixw);
            //Color s=Color.FromArgb(r,g,b);
            color = System.Windows.Media.Color.FromArgb(150, (byte)r, (byte)g, (byte)b);
            GC.Collect();
        });
        return new System.Windows.Media.SolidColorBrush(color);
    }
}