using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Nito.Hosting.AvaloniauiDesktop;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.Versioning;

namespace AvaloniaSample
{
    internal sealed class Program
    {
        [STAThread]
        [SupportedOSPlatform("windows")]
        [SupportedOSPlatform("linux")]
        [SupportedOSPlatform("macos")]
        [RequiresDynamicCode("Calls Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder()")]
        public static void Main(string[] args)
        {
            var hostBuilder = Host.CreateApplicationBuilder();

            // ioc configure
            hostBuilder.Configuration.AddCommandLine(args);

            // add avaloniaui application
            hostBuilder.Services.AddAvaloniauiDesktopApplication<App>(ConfigAvaloniaApp);

            // build host
            var appHost = hostBuilder.Build();

            // run app
            appHost.RunAvaliauiApplication<App>(args);
        }

        public static AppBuilder ConfigAvaloniaApp(AppBuilder appBuilder)
        {
            return appBuilder
                        .UsePlatformDetect()
                        .WithInterFont()
                        .LogToTrace()
                        .UseReactiveUI();
        }
    }
}
