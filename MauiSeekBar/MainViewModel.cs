using System.ComponentModel;
using System.Windows.Input;

namespace MauiSeekBar;

public class MainViewModel : INotifyPropertyChanged
{
    public Action? Play { get; set; }
    public Action? Pause { get; set; }
    public bool IsStartFocused { get; set; }
    public bool IsEndFocused { get; set; }

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

    private TimeSpan start;
    public TimeSpan Start
    {
        get => start;
        set
        {
            start = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Start)));

            if (!IsStartFocused)
            {
                startText = Start.ToString("hh\\:mm\\:ss\\.fff");
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StartText)));
            }
        }
    }

    private TimeSpan end;
    public TimeSpan End
    {
        get => end;
        set
        {
            end = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(End)));

            if (!IsEndFocused)
            {
                endText = End.ToString("hh\\:mm\\:ss\\.fff");
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EndText)));
            }
        }
    }

    private string? startText;
    public string? StartText
    {
        get => startText;
        set
        {
            startText = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StartText)));

            if (TimeSpan.TryParse(StartText, out TimeSpan start))
            {
                this.start = start;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Start)));
            }
        }
    }

    private string? endText;
    public string? EndText
    {
        get => endText;
        set
        {
            endText = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EndText)));

            if (TimeSpan.TryParse(EndText, out TimeSpan end))
            {
                this.end = end;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(End)));
            }
        }
    }

    public ICommand PlayOrPauseCommand { get; set; }
    public ICommand Rewind10SecCommand { get; set; }
    public ICommand Rewind1SecCommand { get; set; }
    public ICommand Forward1SecCommand { get; set; }
    public ICommand Forward10SecCommand { get; set; }
    public ICommand StartCommand { get; set; }
    public ICommand EndCommand { get; set; }

    public MainViewModel()
    {
        Start = default;

        PlayOrPauseCommand = new Command(() => {
            if (IsPlaying)
            {
                Pause?.Invoke();
            }
            else
            {
                Play?.Invoke();
            }
        });
        Rewind10SecCommand = new Command(() => {
            Position = Position.Add(new TimeSpan(0, 0, -10));
        });
        Rewind1SecCommand = new Command(() => {
            Position = Position.Add(new TimeSpan(0, 0, -1));
        });
        Forward1SecCommand = new Command(() => {
            Position = Position.Add(new TimeSpan(0, 0, 1));
        });
        Forward10SecCommand = new Command(() => {
            Position = Position.Add(new TimeSpan(0, 0, 10));
        });
        StartCommand = new Command(() => {
            Start = Position;
        });
        EndCommand = new Command(() => {
            End = Position;
        });
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
