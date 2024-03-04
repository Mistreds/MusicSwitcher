using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using MusicSwitcher.Model;
using Windows.Media.Control;
namespace MusicSwitcher.WorkerServices
{
    public class MusicBackgroundServices:BackgroundService
    {

        private MusicModel _musicModel { get; set; }
        private GlobalSystemMediaTransportControlsSessionManager gsmtcsm;
        private GlobalSystemMediaTransportControlsSessionMediaProperties mediaProperties;

        public MusicBackgroundServices(MusicModel _musicModel)
        {
            GetGSMT();
            this._musicModel = _musicModel;
        }
        private async void GetGSMT() => gsmtcsm = await GetSystemMediaTransportControlsSessionManager();
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                try
                {
                    gsmtcsm = await GetSystemMediaTransportControlsSessionManager();
                    if (gsmtcsm.GetCurrentSession() == null)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(3));
                        continue;
                    }
                    mediaProperties = await GetMediaProperties(gsmtcsm.GetCurrentSession());
                    var currSession = gsmtcsm.GetCurrentSession();
                    var playBack = currSession.GetPlaybackInfo();
                    if (_musicModel.AlbumName == mediaProperties.AlbumTitle &&
                        _musicModel.SingName == mediaProperties.Title && _musicModel.Status== playBack.PlaybackStatus.ToString())
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(1500));
                        continue;
                    }
                    var stream = await mediaProperties.Thumbnail.OpenReadAsync();
                    var bytes = default(byte[]);
                    using (StreamReader sr = new StreamReader(stream.AsStreamForRead()))
                    {
                        using (var memstream = new MemoryStream())
                        {
                            var buffer = new byte[512];
                            var bytesRead = default(int);
                            while ((bytesRead = sr.BaseStream.Read(buffer, 0, buffer.Length)) > 0)
                                memstream.Write(buffer, 0, bytesRead);
                            bytes = memstream.ToArray();

                        }
                    }

                    await  _musicModel.UpdateMusic(mediaProperties.Title, mediaProperties.AlbumTitle, mediaProperties.Artist,
                        playBack.PlaybackStatus.ToString(), bytes);
                    await Task.Delay(TimeSpan.FromMilliseconds(1500));
                }
                catch (Exception e)
                {
                    await Task.Delay(TimeSpan.FromSeconds(3));
                    continue;
                }
              
            }
           
        }
        private async Task<GlobalSystemMediaTransportControlsSessionManager> GetSystemMediaTransportControlsSessionManager() =>
            await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();

        private async Task<GlobalSystemMediaTransportControlsSessionMediaProperties> GetMediaProperties(GlobalSystemMediaTransportControlsSession session)
            => await session.TryGetMediaPropertiesAsync();
    }
}
