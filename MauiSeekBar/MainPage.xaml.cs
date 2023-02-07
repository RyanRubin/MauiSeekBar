namespace MauiSeekBar;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        NavigatedTo += MainPage_NavigatedTo;
    }

    private void MainPage_NavigatedTo(object? sender, NavigatedToEventArgs e)
    {
        var vm = new MainViewModel();
        vm.Play += () => media.Play();
        vm.Pause += () => media.Pause();
        BindingContext = vm;
    }
}
