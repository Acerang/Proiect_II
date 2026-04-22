namespace testare.frontend;

public partial class LockScreenPage : ContentPage
{
    public LockScreenPage()
    {
        InitializeComponent();
    }

    private async void OnUnlockClicked(object sender, EventArgs e)
    {
        // se simuleaza succesul
        await DisplayAlert("Unlocked!", "Good job! You have 15 minutes of access.", "Thanks!");
    }

    private async void OnGoToAppClicked(object sender, EventArgs e)
    {
        // utilizatorul merge la aplicatia principala
        Application.Current.MainPage = new NavigationPage(new MainPage());
    }
}