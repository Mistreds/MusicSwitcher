using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Control;
using MusicSwitcher.Model;

namespace MusicSwitcher.Services
{
    public interface IMusicServices
    {

        public Task NextButton();
        public Task BackButton();
        public Task StartStop();
        public Task UpdateMusic();
    }

    public class MusicServices : IMusicServices
    {
        private GlobalSystemMediaTransportControlsSessionManager gsmtcsm;
        private GlobalSystemMediaTransportControlsSessionMediaProperties mediaProperties;

        private MusicModel _musicModel;
        public MusicServices(MusicModel _musicModel)
        {
            GetGSMT();
            this._musicModel= _musicModel;
            
        }

        public async Task NextButton()
        {
            try
            {
                gsmtcsm = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
                var CurrSession = gsmtcsm.GetCurrentSession();
                await CurrSession.TrySkipNextAsync();
                await UpdateMusic();
            }
            catch
            {
                return;
            }
        }

        public async Task BackButton()
        {
            try
            {
                gsmtcsm = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
                var CurrSession = gsmtcsm.GetCurrentSession();
                await CurrSession.TrySkipPreviousAsync();
                await UpdateMusic();
            }
            catch
            {

            }
        }

        public async Task StartStop()
        {
            try
            {
                gsmtcsm = await GetSystemMediaTransportControlsSessionManager();
                var CurrSession = gsmtcsm.GetCurrentSession();
                var play_back = CurrSession.GetPlaybackInfo();
                if (play_back.PlaybackStatus.ToString() == "Paused")
                {
                    await CurrSession.TryPlayAsync();
                }
                else
                {
                    await CurrSession.TryPauseAsync();
                }
                await UpdateMusic();
            }
            catch
            {
            }
        }

        public async Task UpdateMusic()
        {
            try
            {
                gsmtcsm = await GetSystemMediaTransportControlsSessionManager();
                if (gsmtcsm.GetCurrentSession() == null)
                {
                    await _musicModel.SetDefault();
                    return;
                }

                mediaProperties = await GetMediaProperties(gsmtcsm.GetCurrentSession());
                var currSession = gsmtcsm.GetCurrentSession();
                var playBack = currSession.GetPlaybackInfo();
                if (_musicModel.AlbumName == mediaProperties.AlbumTitle &&
                    _musicModel.SingName == mediaProperties.Title &&
                    _musicModel.Status == playBack.PlaybackStatus.ToString())
                {
                    return;
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
                await _musicModel.UpdateMusic(mediaProperties.Title, mediaProperties.AlbumTitle, mediaProperties.Artist,
                    playBack.PlaybackStatus.ToString(), bytes);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
           
        }
    

        private async void GetGSMT() => gsmtcsm = await GetSystemMediaTransportControlsSessionManager();
        private async Task<GlobalSystemMediaTransportControlsSessionManager> GetSystemMediaTransportControlsSessionManager() =>
            await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();

        private async Task<GlobalSystemMediaTransportControlsSessionMediaProperties> GetMediaProperties(GlobalSystemMediaTransportControlsSession session)
            => await session.TryGetMediaPropertiesAsync();
    }
}
