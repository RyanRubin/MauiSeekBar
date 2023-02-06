namespace MauiSeekBar;

public class VideoPlayer : ContentView
{
    public static readonly BindableProperty SourceProperty = BindableProperty.Create(nameof(Source), typeof(string), typeof(SeekBar), string.Empty, propertyChanged: OnSourceChanged);
    public string Source
    {
        get => (string)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }
    private static void OnSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var videoPlayer = (VideoPlayer)bindable;
        string videoFile = (string)newValue;
        videoPlayer.LoadVideo(videoFile);
    }

    public static readonly BindableProperty IsPlayingProperty = BindableProperty.Create(nameof(IsPlaying), typeof(bool), typeof(SeekBar), false);
    public bool IsPlaying
    {
        get => (bool)GetValue(IsPlayingProperty);
        set => SetValue(IsPlayingProperty, value);
    }

    private WebView? webView;

    private readonly string videoPlayerHtmlFileUri;

    public VideoPlayer()
    {
        videoPlayerHtmlFileUri = new Uri(Path.Combine(AppContext.BaseDirectory, "video-player.html")).AbsoluteUri;
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
        IsPlaying = true;
        webView?.EvaluateJavaScriptAsync($"play()");
    }

    public void Pause()
    {
        IsPlaying = false;
        webView?.EvaluateJavaScriptAsync($"pause()");
    }
}
