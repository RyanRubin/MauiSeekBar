using CommunityToolkit.Maui.Core.Primitives;
using System.ComponentModel;
using System.Windows.Input;

namespace MauiSeekBar;

public class MainViewModel : INotifyPropertyChanged
{
    public Action? Play { get; set; }
    public Action? Pause { get; set; }
    public Func<MediaElementState>? GetCurrentState { get; set; }

    public ICommand? PlayOrPauseCommand { get; set; }

    public MainViewModel()
    {
        PlayOrPauseCommand = new Command(PlayOrPause);
    }

    private void PlayOrPause()
    {
        if (GetCurrentState?.Invoke() == MediaElementState.Playing)
        {
            Pause?.Invoke();
        }
        else
        {
            Play?.Invoke();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
