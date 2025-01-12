using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using System.Net.Sockets;
using UdpEsp8266App.Services;
namespace UdpEsp8266App;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddSingleton<UdpClient>(provider =>
        {
            return new UdpClient();
        });
        
        builder.Services.AddSingleton<TcpPage>();
        builder.Services.AddTransient<SwitchControlPage>();
        builder.Services.AddSingleton<MqttService>();


        return builder.Build();
    }
}
