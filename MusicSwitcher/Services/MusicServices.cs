using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Control;

namespace MusicSwitcher.Services
{
    public interface IMusicServices
    {

        public Task NextButton();
        public Task BackButton();
        public Task StartStop();
    }
    public class MusicServices:IMusicServices
    {
        private GlobalSystemMediaTransportControlsSessionManager gsmtcsm;
        private GlobalSystemMediaTransportControlsSessionMediaProperties mediaProperties;
        public MusicServices()
        {

            GetGSMT();
        }

        public async Task NextButton()
        {
            try
            {
                var CurrSession = gsmtcsm.GetCurrentSession();
                await CurrSession.TrySkipNextAsync();
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
                var CurrSession = gsmtcsm.GetCurrentSession();
                await CurrSession.TrySkipPreviousAsync();
            }
            catch
            {
                
            }
        }
        public async Task StartStop()
        {
            try
            {
                var CurrSession = gsmtcsm.GetCurrentSession();
                var play_back = CurrSession.GetPlaybackInfo();
                if (play_back.PlaybackStatus.ToString() == "Paused")
                {
                    await  CurrSession.TryPlayAsync();
                }
                else
                {
                    await CurrSession.TryPauseAsync();
                }
            }
            catch { }
        }
        private async void GetGSMT() => gsmtcsm = await GetSystemMediaTransportControlsSessionManager();
        private async Task<GlobalSystemMediaTransportControlsSessionManager> GetSystemMediaTransportControlsSessionManager() =>
            await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();

        private async Task<GlobalSystemMediaTransportControlsSessionMediaProperties> GetMediaProperties(GlobalSystemMediaTransportControlsSession session)
            => await session.TryGetMediaPropertiesAsync();
    }
}
