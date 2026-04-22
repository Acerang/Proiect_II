using System.Collections.ObjectModel;
using System.Text.Json;

namespace testare;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // se verifica ce date sunt salvate
        MainPage = new SplashScreen();
    }
}