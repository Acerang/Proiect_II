namespace testare;

public partial class MainPage : ContentPage
{
    int currentGlasses = 0;
    const int targetGlasses = 8;
    int snoozeCount = 0;
    const int maxSnoozes = 3;
    bool isLockedUntilTomorrow = false;

    public MainPage()
    {
        InitializeComponent();

        // load data for a new day
        CheckDateAndLoadData();

        // start the timer for the countdown
        StartGlobalTimer();
    }

    private void CheckDateAndLoadData()
    {
        string today = DateTime.Now.ToString("ddMMyyyy");
        string lastSavedDate = Preferences.Default.Get("LastSavedDate", "");

        if (lastSavedDate != today)
        {
            // new day -> Reset everything
            currentGlasses = 0;
            snoozeCount = 0;
            isLockedUntilTomorrow = false;

            Preferences.Default.Set("LastSavedDate", today);
            Preferences.Default.Set("WaterCount", 0);
            Preferences.Default.Set("SnoozeCount", 0);
            Preferences.Default.Set("IsLocked", false);
        }
        else
        {
            // same day ->: Load saved progress
            currentGlasses = Preferences.Default.Get("WaterCount", 0);
            snoozeCount = Preferences.Default.Get("SnoozeCount", 0);
            isLockedUntilTomorrow = Preferences.Default.Get("IsLocked", false);
        }

        UpdateInterface();
    }

    private void StartGlobalTimer()
    {
        IDispatcherTimer timer = Dispatcher.CreateTimer();
        timer.Interval = TimeSpan.FromSeconds(1);
        timer.Tick += (s, e) => {
            if (isLockedUntilTomorrow)
            {
                UpdateCountdown();
            }
        };
        timer.Start();
    }

    private void UpdateCountdown()
    {
        DateTime now = DateTime.Now;
        DateTime midnight = DateTime.Today.AddDays(1);
        TimeSpan timeLeft = midnight - now;

        CountdownLabel.Text = timeLeft.ToString(@"hh\:mm\:ss");

        MainUI.IsVisible = false;
        LockdownUI.IsVisible = true;
    }

    private async void OnDrinkWaterClicked(object sender, EventArgs e)
    {
        if (currentGlasses < targetGlasses)
        {
            currentGlasses++;
            Preferences.Default.Set("WaterCount", currentGlasses);
            UpdateInterface();

            if (currentGlasses == targetGlasses)
            {
                await DisplayAlert("Success!", "You have reached your hydration goal! App unlocked.", "Great!");
            }
        }
    }

    private void UpdateInterface()
    {
        // update labels and progress bar
        GlassesLabel.Text = $"{currentGlasses} / {targetGlasses}";
        WaterProgressBar.Progress = (double)currentGlasses / targetGlasses;

        // update snooze button status
        int remainingSnoozes = maxSnoozes - snoozeCount;
        SnoozeButton.Text = $"I need 5 more minutes ({remainingSnoozes} left)";

        if (remainingSnoozes <= 0)
        {
            SnoozeButton.IsEnabled = false;
            SnoozeButton.Text = "No more snoozes today";
        }

        // handle UI states
        if (isLockedUntilTomorrow)
        {
            MainUI.IsVisible = false;
            LockdownUI.IsVisible = true;
        }
        else
        {
            MainUI.IsVisible = true;
            LockdownUI.IsVisible = false;
        }
    }

    private async void OnSnoozeClicked(object sender, EventArgs e)
    {
        if (snoozeCount < maxSnoozes)
        {
            int remaining = maxSnoozes - (snoozeCount + 1);
            bool confirm = await DisplayAlert("Warning",
                $"You have {remaining + 1} snoozes left for today. If you postpone now, you will be locked out until tomorrow!",
                "I'll take the risk", "Cancel");

            if (confirm)
            {
                snoozeCount++;
                Preferences.Default.Set("SnoozeCount", snoozeCount);

                isLockedUntilTomorrow = true;
                Preferences.Default.Set("IsLocked", true);

                UpdateInterface();
            }
        }
        else
        {
            await DisplayAlert("Limit Reached", "You have used all your snoozes for today. You must drink water!", "OK");
        }
    }

    private async void OnEditLimitsClicked(object sender, EventArgs e)
    {
        // se opreste serverul
        #if ANDROID
        var intent = new Android.Content.Intent(Android.App.Application.Context, typeof(WaterGuardianService));
        Android.App.Application.Context.StopService(intent);
        #endif

        // ne intoarcem la pagina de configurare
        Application.Current.MainPage = new NavigationPage(new AppSelectionPage());
    }
}