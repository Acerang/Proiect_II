using System.Collections.ObjectModel;
using System.Text.Json;

namespace testare;

public partial class SplashScreen : ContentPage
{
    public SplashScreen()
    {
        InitializeComponent();
        StartTimer();
    }

    private async void StartTimer()
    {
        await Task.Delay(3000);
        // se verifica datele
        string savedApps = Preferences.Default.Get("MonitoredApps", string.Empty);

        if (!string.IsNullOrEmpty(savedApps))
        {
            try
            {
                var appsList = JsonSerializer.Deserialize<ObservableCollection<AppLimitItem>>(savedApps);
                Application.Current.MainPage = new NavigationPage(new MonitoringPage(appsList));
            }
            catch
            {
                Application.Current.MainPage = new NavigationPage(new AppSelectionPage());
            }
        }
        else
        {
            Application.Current.MainPage = new NavigationPage(new AppSelectionPage());
        }
    }
}