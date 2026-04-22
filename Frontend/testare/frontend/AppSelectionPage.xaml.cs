using System.Collections.ObjectModel;
using System.Text.Json;
using System.Linq;

namespace testare;

public partial class AppSelectionPage : ContentPage
{
    public ObservableCollection<AppLimitItem> AppLimits { get; set; } = new ObservableCollection<AppLimitItem>();

    public AppSelectionPage()
    {
        InitializeComponent();
        AppsListDisplay.ItemsSource = AppLimits;
        LoadInstalledApps();
    }

    private void LoadInstalledApps()
    {
        var appNames = new List<string>();
#if ANDROID
        var context = Android.App.Application.Context;
        var apps = context.PackageManager.GetInstalledApplications(Android.Content.PM.PackageInfoFlags.MatchAll);
        foreach (var app in apps)
        {
            if (context.PackageManager.GetLaunchIntentForPackage(app.PackageName) != null)
            {
                appNames.Add(app.LoadLabel(context.PackageManager).ToString());
            }
        }
#else
        appNames.AddRange(new[] { "Chrome", "YouTube", "TikTok", "Instagram" });
#endif
        AppPicker.ItemsSource = appNames.OrderBy(a => a).ToList();
    }

    private void OnAddAppClicked(object sender, EventArgs e)
    {
        if (AppPicker.SelectedIndex != -1 && !string.IsNullOrWhiteSpace(EntryAppLimit.Text))
        {
            // cautam numele aplicatiei
            string selectedName = AppPicker.SelectedItem.ToString();

            AppLimits.Add(new AppLimitItem
            {
                Name = selectedName,
                PackageName = selectedName, // serviciul va cauta acest string
                Limit = int.Parse(EntryAppLimit.Text)
            });

            EntryAppLimit.Text = string.Empty;
            AppPicker.SelectedIndex = -1;
        }
    }

    private async void OnSaveAndStartClicked(object sender, EventArgs e)
    {
        if (AppLimits.Count == 0) return;
        string serializedList = JsonSerializer.Serialize(AppLimits.ToList());
        Preferences.Default.Set("MonitoredApps", serializedList);

#if ANDROID
        var intent = new Android.Content.Intent(Android.App.Application.Context, typeof(WaterGuardianService));
        Android.App.Application.Context.StartService(intent);
#endif

        Application.Current.MainPage = new NavigationPage(new MonitoringPage(AppLimits));
    }
}

public class AppLimitItem
{
    public string Name { get; set; }
    public string PackageName { get; set; }
    public int Limit { get; set; }
}