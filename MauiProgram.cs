using Microsoft.Maui.LifecycleEvents;
using Microsoft.UI.Windowing;

namespace Lab2OOP;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()

            .ConfigureLifecycleEvents(events =>
            {
#if WINDOWS
                events.AddWindows(windows => windows
                    .OnWindowCreated(window =>
                    {
                        AppWindow appWindow = window.AppWindow;

                        appWindow.Closing += async (sender, args) =>
                        {
                            args.Cancel = true;

                            bool answer = await Application.Current.MainPage.DisplayAlert(
                                "Підтвердження",
                                "Ви дійсно хочете вийти з програми?",
                                "Так", "Ні");

                            if (answer)
                            {
                                Application.Current.Quit();
                            }
                        };
                    }));
#endif
            })

            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddSingleton<MainPage>();

        return builder.Build();
    }
}