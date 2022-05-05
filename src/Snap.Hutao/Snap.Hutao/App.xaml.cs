﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Snap.Hutao.Core;
using Snap.Hutao.Web.Request;

namespace Snap.Hutao;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    private Window? mainWindow;

    /// <summary>
    /// Initializes the singleton application object.
    /// This is the first line of authored code executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        // load app resource
        InitializeComponent();

        InitializeDependencyInjection();
    }

    /// <summary>
    /// Invoked when the application is launched normally by the end user.
    /// Other entry points will be used such as when the application is launched to open a specific file.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        mainWindow = Ioc.Default.GetRequiredService<MainWindow>();
        mainWindow.Activate();
    }

    private static void InitializeDependencyInjection()
    {
        // prepare DI
        IServiceProvider services = new ServiceCollection()
            .AddLogging(builder => builder.AddDebug())

            // http json
            .AddHttpClient<HttpJson>()
            .ConfigureHttpClient(client =>
            {
                client.Timeout = Timeout.InfiniteTimeSpan;
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) Snap Hutao");
            })
            .Services

            // requester & auth reuqester
            .AddHttpClient<Requester>(nameof(Requester))
            .AddTypedClient<AuthRequester>()
            .ConfigureHttpClient(client => client.Timeout = Timeout.InfiniteTimeSpan)
            .Services

            // inject app wide services
            .AddInjections(typeof(App))
            .BuildServiceProvider();

        Ioc.Default.ConfigureServices(services);
    }
}