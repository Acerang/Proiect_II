using Android.App;
using Android.Content;
using Android.OS;
using Android.App.Usage;
using System.Text.Json;

namespace testare;

[Service]
public class WaterGuardianService : Service
{
    private Timer _timer;
    private const int CheckIntervalSeconds = 5; // se verifica la fiecare 5 secunde

    public override IBinder OnBind(Intent intent) => null;

    public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
    {
        // se porneste cronometrul
        _timer = new Timer(CheckCurrentApp, null, 0, CheckIntervalSeconds * 1000);
        return StartCommandResult.Sticky;
    }

    private void CheckCurrentApp(object state)
    {
        var usageStatsManager = (UsageStatsManager)GetSystemService(Context.UsageStatsService);
        long endTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        long startTime = endTime - 10000;

        var stats = usageStatsManager.QueryUsageStats(UsageStatsInterval.Daily, startTime, endTime);

        if (stats != null)
        {
            var topApp = stats.OrderByDescending(s => s.LastTimeUsed).FirstOrDefault();
            if (topApp != null)
            {
                UpdateAppTime(topApp.PackageName);
            }
        }
    }

    private void UpdateAppTime(string currentPackageName)
    {
        // se citeste lista in Prefrences
        string savedApps = Preferences.Default.Get("MonitoredApps", string.Empty);
        if (string.IsNullOrEmpty(savedApps)) return;

        var monitoredList = JsonSerializer.Deserialize<List<AppLimitItem>>(savedApps);
        bool timeExpired = false;
        bool listChanged = false;

        // se cauta daca aplicatia este in lista
        var app = monitoredList.FirstOrDefault(a =>
            a.Name.ToLower() == currentPackageName.ToLower() ||
            a.PackageName == currentPackageName);

        if (app != null && app.Limit > 0)
        {
            // se scade intervalul de verificare din minutele ramase
            double minutesToSubtract = CheckIntervalSeconds / 60.0;
            app.Limit -= (int)Math.Ceiling(minutesToSubtract);

            if (app.Limit <= 0)
            {
                app.Limit = 0;
                timeExpired = true;
            }
            listChanged = true;
        }

        // daca timpul s a schimbat salvam iar lista
        if (listChanged)
        {
            string updatedJson = JsonSerializer.Serialize(monitoredList);
            Preferences.Default.Set("MonitoredApps", updatedJson);
        }

        // daca timpul expira -> blocare
        if (timeExpired)
        {
            TriggerLockdown();
        }
    }

    private void TriggerLockdown()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            // Deschidem aplicația noastră direct pe pagina cu apa
            var intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.NewTask | ActivityFlags.ClearTop | ActivityFlags.SingleTop);
            StartActivity(intent);

            Microsoft.Maui.Controls.Application.Current.MainPage = new NavigationPage(new MainPage());
        });
    }

    public override void OnDestroy()
    {
        _timer?.Dispose();
        base.OnDestroy();
    }
}