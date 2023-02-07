﻿using System.Diagnostics;

namespace MauiSeekBar;

public class VideoPlayer : ContentView
{
    public static readonly BindableProperty SourceProperty = BindableProperty.Create(nameof(Source), typeof(string), typeof(SeekBar), default, BindingMode.TwoWay, propertyChanged: OnSourceChanged);
    public string Source { get => (string)GetValue(SourceProperty); set => SetValue(SourceProperty, value); }
    private static void OnSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var videoPlayer = (VideoPlayer)bindable;
        string videoFile = (string)newValue;
        videoPlayer.LoadVideo(videoFile);
    }

    public static readonly BindableProperty IsPlayingProperty = BindableProperty.Create(nameof(IsPlaying), typeof(bool), typeof(SeekBar), default, BindingMode.TwoWay);
    public bool IsPlaying { get => (bool)GetValue(IsPlayingProperty); set => SetValue(IsPlayingProperty, value); }

    public static readonly BindableProperty PositionProperty = BindableProperty.Create(nameof(Position), typeof(TimeSpan), typeof(SeekBar), default, BindingMode.TwoWay);
    public TimeSpan Position { get => (TimeSpan)GetValue(PositionProperty); set => SetValue(PositionProperty, value); }

    public static readonly BindableProperty DurationProperty = BindableProperty.Create(nameof(Duration), typeof(TimeSpan), typeof(SeekBar), default, BindingMode.TwoWay);
    public TimeSpan Duration { get => (TimeSpan)GetValue(DurationProperty); set => SetValue(DurationProperty, value); }

    private WebView? webView;

    private readonly string videoPlayerHtmlFileUri;
    private readonly Stopwatch stopwatch;
    private readonly IDispatcherTimer timer;

    public VideoPlayer()
    {
        videoPlayerHtmlFileUri = new Uri(Path.Combine(AppContext.BaseDirectory, "video-player.html")).AbsoluteUri;
        stopwatch = new Stopwatch();
        timer = Dispatcher.CreateTimer();
        timer.Interval = new TimeSpan(0, 0, 0, 0, 25);
        timer.Tick += (sender, e) => {
            Position = stopwatch.Elapsed;
            if (Position >= Duration)
            {
                Pause();
                Position = Duration;
            }
        };
    }

    private void LoadVideo(string videoFile)
    {
        if (string.IsNullOrEmpty(videoFile))
        {
            Content = null;
        }
        else
        {
            webView = new WebView { Source = videoPlayerHtmlFileUri };
            webView.Navigated += WebView_Navigated;
            Content = webView;
        }
    }

    private async void WebView_Navigated(object? sender, WebNavigatedEventArgs e)
    {
        if (webView != null && string.Equals(e.Url, videoPlayerHtmlFileUri, StringComparison.InvariantCultureIgnoreCase))
        {
            await webView.EvaluateJavaScriptAsync($"loadVideo('{new Uri(Source).AbsoluteUri}')");
            string retVal;
            do
            {
                await Task.Delay(100);
                retVal = await webView.EvaluateJavaScriptAsync($"getDuration()");
            } while (string.IsNullOrEmpty(retVal));
            double millis = double.Parse(retVal) * 1000;
            Duration = new TimeSpan(0, 0, 0, 0, (int)millis);
        }
    }

    public void Play()
    {
        if (Position >= Duration)
        {
            Position = default;
        }
        stopwatch.Start();
        timer.Start();
        IsPlaying = true;
        webView?.EvaluateJavaScriptAsync($"play()");
    }

    public void Pause()
    {
        stopwatch.Start();
        timer.Stop();
        IsPlaying = false;
        webView?.EvaluateJavaScriptAsync($"pause()");
    }
}
