namespace testare;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(UsernameEntry.Text))
        {
            // save the username to indicate the user is logged in
            Preferences.Default.Set("UserNickname", UsernameEntry.Text);

            await DisplayAlert("Success", $"Welcome, {UsernameEntry.Text}!", "OK");

            // go back to the previous page (ProfilePage)
            await Navigation.PopAsync();
        }
        else
        {
            await DisplayAlert("Error", "Please enter a username", "OK");
        }
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}