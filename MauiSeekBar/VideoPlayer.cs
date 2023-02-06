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

    private WebView? webView;

    private readonly string videoPlayerHtmlFileUri;
    private readonly IDispatcherTimer timer;

    public VideoPlayer()
    {
        videoPlayerHtmlFileUri = new Uri(Path.Combine(AppContext.BaseDirectory, "video-player.html")).AbsoluteUri;
        timer = Dispatcher.CreateTimer();
        timer.Interval = new TimeSpan(0, 0, 1);
        timer.Tick += (sender, e) => Position = Position.Add(new TimeSpan(0, 0, 1));
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
        }
    }

    public void Play()
    {
        timer.Start();
        IsPlaying = true;
        webView?.EvaluateJavaScriptAsync($"play()");
    }

    public void Pause()
    {
        timer.Stop();
        IsPlaying = false;
        webView?.EvaluateJavaScriptAsync($"pause()");
    }
}
