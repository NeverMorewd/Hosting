using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;

namespace Nito.Hosting.AvaloniauiDesktop
{
    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    [SupportedOSPlatform("macos")]
    public static class AvaloniauiApplicationLifetimeExtensions
    {
        /// <summary>
        /// Configures the host to use avaloniaui application lifetime.
        /// Also configures the <typeparamref name="TApplication"/> as a singleton.
        /// </summary>
        /// <typeparam name="TApplication">The type of avaloniaui application <see cref="Application"/> to manage.</typeparam>
        /// <param name="appBuilderConfiger"><see cref="AppBuilder.Configure{TApplication}()"/></param>

        public static IServiceCollection AddAvaloniauiDesktopApplication<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TApplication>(this IServiceCollection services,
            Func<AppBuilder, AppBuilder> appBuilderConfiger)
            where TApplication : Application, new()
        {
            return services
                    .AddSingleton<TApplication>()
                    .AddSingleton(provider =>
                    {
                        var appBuilder = AppBuilder.Configure(() =>
                        {
                            return provider.GetRequiredService<TApplication>();
                        });

                        appBuilder = appBuilderConfiger(appBuilder);

                        return appBuilder;
                    })
                    .AddSingleton<IHostLifetime, AvaloniauiApplicationLifetime<TApplication>>();
        }


        /// <summary>
        /// Runs the avaloniaui application along with the .NET generic host.
        /// </summary>
        /// <typeparam name="TApplication">The type of the avaloniaui application <see cref="Application"/> to run.</typeparam>
        /// <param name="commandArgs">commmandline args</param>
        /// <param name="cancellationToken">cancellationToken</param>
        public static Task RunAvaliauiApplication<TApplication>(this IHost host,
             string[] commandArgs,
            CancellationToken cancellationToken = default)
            where TApplication : Application
        {
            _ = host ?? throw new ArgumentNullException(nameof(host));
            var builder = host.Services.GetRequiredService<AppBuilder>();
            builder = builder.SetupWithLifetime(new ClassicDesktopStyleApplicationLifetime
            {
                Args = commandArgs,
                // can be reset in OnFrameworkInitializationCompleted
                ShutdownMode = Avalonia.Controls.ShutdownMode.OnMainWindowClose,
            });

            if (builder.Instance == null)
            {
                throw new NotImplementedException("No application initialized!");
            }

            var hostTask = host.RunAsync(token: cancellationToken);
            if (builder.Instance.ApplicationLifetime is ClassicDesktopStyleApplicationLifetime classicDesktop)
            {
                Environment.ExitCode = classicDesktop.Start(classicDesktop.Args ?? Array.Empty<string>());
            }
            else
            {
                throw new NotImplementedException("Genric host support classic desktop only!");
            }
            return hostTask;
        }
    }
}
