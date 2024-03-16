using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Control;
using MusicSwitcher.Model;
using System.Security.Cryptography;
using System.Security.Policy;

namespace MusicSwitcher.Services
{
    public interface IMusicServices
    {
        /// <summary>Включение следующей песни </summary>
        public Task NextButton();

        /// <summary>Включение предыдущей песни </summary>
        public Task BackButton();
        /// <summary> Старт или остановка песни </summary>
        public Task StartStop();
        /// <summary>Обновление модели</summary>
        public Task UpdateMusic();
    }

    /// <summary>
    /// Сервис по переключению музыки
    /// </summary>
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

                }
                else
                {
                    await _musicModel.UpdateMusic(mediaProperties.Title, mediaProperties.AlbumTitle, mediaProperties.Artist,
                        playBack.PlaybackStatus.ToString());
                }
                var stream = await mediaProperties.Thumbnail.OpenReadAsync();
                using var md5 = MD5.Create();
                using StreamReader sr = new StreamReader(stream.AsStreamForRead());
                using var memstream = new MemoryStream();
                var buffer = new byte[512];
                var bytesRead = default(int);
                
                while ((bytesRead = sr.BaseStream.Read(buffer, 0, buffer.Length)) > 0)
                    memstream.Write(buffer, 0, bytesRead);
                var bytes = memstream.ToArray();
                var hash = string.Join("", md5.ComputeHash(bytes));
                if (_musicModel.HashImage != hash)
                    await _musicModel.UpdatePicture(bytes);
                
                GC.Collect();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
           
        }


        private async void GetGSMT() => gsmtcsm = await GetSystemMediaTransportControlsSessionManager();
        private async Task<GlobalSystemMediaTransportControlsSessionManager> GetSystemMediaTransportControlsSessionManager() =>
            await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();

        private async Task<GlobalSystemMediaTransportControlsSessionMediaProperties> GetMediaProperties(GlobalSystemMediaTransportControlsSession session)
            => await session.TryGetMediaPropertiesAsync();
    }
}
