using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using MusicSwitcher.Model;
using Windows.Media.Control;
using MusicSwitcher.Services;

namespace MusicSwitcher.WorkerServices
{
    public class MusicBackgroundServices:BackgroundService
    {

        private readonly IMusicServices _musicServices;
        public MusicBackgroundServices(IMusicServices _musicServices)
        {
           this._musicServices= _musicServices;

        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                try
                {
                    await Task.Delay(TimeSpan.FromMinutes(1));
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                    continue;
                }

            }
           
        }
    }
}
