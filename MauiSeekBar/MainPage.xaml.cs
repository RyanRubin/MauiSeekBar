namespace MauiSeekBar;

public partial class MainPage : ContentPage
{
    private readonly MainViewModel vm;

    public MainPage()
    {
        InitializeComponent();
        vm = new MainViewModel();
        NavigatedTo += MainPage_NavigatedTo;
    }

    private void MainPage_NavigatedTo(object? sender, NavigatedToEventArgs e)
    {
        vm.Play += () => media.Play();
        vm.Pause += () => media.Pause();
        BindingContext = vm;
    }

    private void StartEntry_Focused(object sender, FocusEventArgs e) => vm.IsStartFocused = true;

    private void StartEntry_Unfocused(object sender, FocusEventArgs e) => vm.IsStartFocused = false;

    private void EndEntry_Focused(object sender, FocusEventArgs e) => vm.IsEndFocused = true;

    private void EndEntry_Unfocused(object sender, FocusEventArgs e) => vm.IsEndFocused = false;
}
