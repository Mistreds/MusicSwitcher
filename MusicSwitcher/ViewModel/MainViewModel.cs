using MusicSwitcher.Model;
using ReactiveUI;
using System.Reactive;
using MusicSwitcher.Services;

namespace MusicSwitcher.ViewModel;

public class MainViewModel:ReactiveObject
{
    public MusicModel MusicModel { get; set; }

    private readonly IMusicServices _musicServices;
    public MainViewModel(MusicModel _musicModel, IMusicServices musicServices)
    {
       
        this.MusicModel = _musicModel;
        this._musicServices = musicServices;
        
    }

    public ReactiveCommand<Unit, Task> StartStop => ReactiveCommand.Create(async () =>
    {
        await _musicServices.StartStop();
    });
    public ReactiveCommand<Unit, Task> Next => ReactiveCommand.Create(async () =>
    {
        await  _musicServices.NextButton();
    });
    public ReactiveCommand<Unit, Task> Back => ReactiveCommand.Create(async () =>
    {
        await _musicServices.BackButton();
    });
}