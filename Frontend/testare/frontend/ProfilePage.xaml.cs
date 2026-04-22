namespace testare;

public partial class ProfilePage : ContentPage
{
    public ProfilePage()
    {
        InitializeComponent();
        UpdateUIState();
    }

    private void UpdateUIState()
    {
        //se verifica daca userul este logat
        bool isLoggedIn = Preferences.Default.ContainsKey("UserNickname");

        if (isLoggedIn)
        {
            // USER LOGAT
            GuestView.IsVisible = false;
            UserNameLabel.Text = Preferences.Default.Get("UserNickname", "User");

            // se blocheaza butonul de edit
            EditProfileButton.IsEnabled = false;
            EditProfileButton.Text = "Editing Locked (Logged In)";
            EditProfileButton.BackgroundColor = Colors.LightGray;

            HydrationBar.Progress = 0.6;
            StatusMessage.Text = "Welcome back! Your stats are updated.";
        }
        else
        {
            // USER NOU / GUEST
            GuestView.IsVisible = true;
            UserNameLabel.Text = "Guest Mode";

            // butonul de edit este intr un guest mode
            EditProfileButton.IsEnabled = true;
            EditProfileButton.Text = "Edit Profile";
            EditProfileButton.BackgroundColor = Color.FromArgb("#607D8B");

            HydrationBar.Progress = 0.1;
            StatusMessage.Text = "Create an account to start tracking.";
        }
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new LoginPage());
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegisterPage());
    }

    private async void OnEditProfileClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Profile", "Please Register or Login to enable full profile editing.", "Got it");
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        UpdateUIState();
    }
}