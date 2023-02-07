using System.ComponentModel;
using System.Windows.Input;

namespace MauiSeekBar;

public class MainViewModel : INotifyPropertyChanged
{
    public Action? Play { get; set; }
    public Action? Pause { get; set; }

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

    private TimeSpan position;
    public TimeSpan Position
    {
        get => position;
        set
        {
            position = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Position)));
        }
    }

    private TimeSpan duration;
    public TimeSpan Duration
    {
        get => duration;
        set
        {
            duration = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Duration)));
        }
    }

    private bool isPlaying;
    public bool IsPlaying
    {
        get => isPlaying;
        set
        {
            isPlaying = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsPlaying)));
        }
    }

    public ICommand? PlayOrPauseCommand { get; set; }

    public MainViewModel()
    {
        PlayOrPauseCommand = new Command(PlayOrPause);
    }

    private void PlayOrPause()
    {
        if (IsPlaying)
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
