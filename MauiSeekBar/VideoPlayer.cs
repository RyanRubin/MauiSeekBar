using System.Diagnostics;

namespace MauiSeekBar;

public class VideoPlayer : ContentView
{
    public static readonly BindableProperty SourceProperty = BindableProperty.Create(nameof(Source), typeof(string), typeof(VideoPlayer), default, BindingMode.TwoWay, propertyChanged: OnSourceChanged);
    public string Source { get => (string)GetValue(SourceProperty); set => SetValue(SourceProperty, value); }
    private static void OnSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var videoPlayer = (VideoPlayer)bindable;
        string source = (string)newValue;
        videoPlayer.LoadVideo(source);
    }

    public static readonly BindableProperty IsPlayingProperty = BindableProperty.Create(nameof(IsPlaying), typeof(bool), typeof(VideoPlayer), default, BindingMode.TwoWay, propertyChanged: OnIsPlayingChanged);
    public bool IsPlaying { get => (bool)GetValue(IsPlayingProperty); set => SetValue(IsPlayingProperty, value); }
    private static void OnIsPlayingChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var videoPlayer = (VideoPlayer)bindable;
        bool isPlaying = (bool)newValue;
        if (isPlaying)
        {
            videoPlayer.Play();
        }
        else
        {
            videoPlayer.Pause();
        }
    }

    public static readonly BindableProperty PositionProperty = BindableProperty.Create(nameof(Position), typeof(TimeSpan), typeof(VideoPlayer), default, BindingMode.TwoWay, propertyChanged: OnPositionChanged);
    public TimeSpan Position { get => (TimeSpan)GetValue(PositionProperty); set => SetValue(PositionProperty, value); }
    private static async void OnPositionChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var videoPlayer = (VideoPlayer)bindable;
        var position = (TimeSpan)newValue;
        if (position < default(TimeSpan))
        {
            videoPlayer.Position = default;
        }
        else if (position > videoPlayer.Duration)
        {
            videoPlayer.Position = videoPlayer.Duration;
        }
        await videoPlayer.SetCurrentTimeUsingPosition();
    }

    public static readonly BindableProperty DurationProperty = BindableProperty.Create(nameof(Duration), typeof(TimeSpan), typeof(VideoPlayer), default, BindingMode.TwoWay);
    public TimeSpan Duration { get => (TimeSpan)GetValue(DurationProperty); set => SetValue(DurationProperty, value); }

    private WebView? webView;
    private bool isSkipSetCurrentTimeUsingPosition;

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
            isSkipSetCurrentTimeUsingPosition = true;
            Position = Position.Add(stopwatch.Elapsed);
            isSkipSetCurrentTimeUsingPosition = false;
            stopwatch.Restart();
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
                retVal = await webView.EvaluateJavaScriptAsync("getDuration()");
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
            webView?.EvaluateJavaScriptAsync("setCurrentTime(0)");
        }
        timer.Start();
        IsPlaying = true;
        webView?.EvaluateJavaScriptAsync("play()");
    }

    public void Pause()
    {
        stopwatch.Reset();
        timer.Stop();
        IsPlaying = false;
        webView?.EvaluateJavaScriptAsync("pause()");
    }

    private async Task SetCurrentTimeUsingPosition()
    {
        if (isSkipSetCurrentTimeUsingPosition)
        {
            return;
        }
        float positionMillis = (float)Position.TotalMilliseconds;
        if (webView != null)
        {
            await webView.EvaluateJavaScriptAsync($"setCurrentTime('{positionMillis / 1000}')");
        }
    }
}
