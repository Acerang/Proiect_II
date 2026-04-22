using System.Collections.ObjectModel;

namespace testare;

public partial class MonitoringPage : ContentPage
{
    public MonitoringPage(ObservableCollection<AppLimitItem> apps)
    {
        InitializeComponent();
        MonitoringListDisplay.ItemsSource = apps;
    }

    private async void OnAddNewAppClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AppSelectionPage());
    }

    private async void OnGoToWaterClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MainPage());
    }

    private async void OnResetSettingsClicked(object sender, EventArgs e)
    {
        bool answer = await DisplayAlert("Reset", "Delete all limits?", "Yes", "No");
        if (answer)
        {
            Preferences.Default.Remove("MonitoredApps");
            Application.Current.MainPage = new NavigationPage(new AppSelectionPage());
        }
    }

    private async void OnProfileClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ProfilePage());
    }
}