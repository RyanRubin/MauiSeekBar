using System.ComponentModel;
using System.Windows.Input;

namespace MauiSeekBar;

public class MainViewModel : INotifyPropertyChanged
{
    public Action? Play { get; set; }
    public Action? Pause { get; set; }
    public Func<bool>? GetIsPlaying { get; set; }

    private string? videoFile;
    public string? VideoFile
    {
        get => videoFile;
        set
        {
            videoFile = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(VideoFile)));
        }
    }

    public ICommand? PlayOrPauseCommand { get; set; }

    public MainViewModel()
    {
        PlayOrPauseCommand = new Command(PlayOrPause);
    }

    private void PlayOrPause()
    {
        if (GetIsPlaying?.Invoke() == true)
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
