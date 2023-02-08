﻿using System.ComponentModel;
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
    public ICommand? Rewind10SecCommand { get; set; }
    public ICommand? Rewind1SecCommand { get; set; }
    public ICommand? Forward1SecCommand { get; set; }
    public ICommand? Forward10SecCommand { get; set; }

    public MainViewModel()
    {
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
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
