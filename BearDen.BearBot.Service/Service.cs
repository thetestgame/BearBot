// Copyright (c) Jordan Maxwell. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using BearDen.BearBot.Service.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BearDen.BearBot.Service
{
    /// <summary>
    /// 
    /// </summary>
    public class Service
    {
        public IConfigurationRoot Configuration { get; }

        public Service(string[] args)
        {
            // Load our application configuration file
            var builder = new ConfigurationBuilder();

            this.Configuration = builder.Build();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static async Task RunAsync(string[] args)
        {
            var startup = new Service(args);
            await startup.RunAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task RunAsync()
        {
            var services = new ServiceCollection();                             // Create a new services collection and configure it
            this.ConfigureServices(services);

            var provider = services.BuildServiceProvider();                     // Create our service provider instance
            provider.GetRequiredService<LoggingService>();                      // Start the required logging service
            provider.GetRequiredService<CommandHandlingService>();              // Start the required command handling service

            await provider.GetRequiredService<StartupService>().StartAsync();   // Start the required startup service
            await Task.Delay(Timeout.Infinite);                                 // Keep the application alive
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        private void ConfigureServices(IServiceCollection services)
        {
            // Add the base Discord socket client service to the collection
            services.AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose,
                MessageCacheSize = 1000
            }))

            // Add the command handling services to the collection
            .AddSingleton(new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Verbose,
                DefaultRunMode = RunMode.Async
            }))
            .AddSingleton<CommandHandlingService>()

            // Add misc other required services and base startup service to 
            // the collection instance
            .AddSingleton<StartupService>()             // Add the startup service to the collection    
            .AddSingleton<LoggingService>()             // Add the logging service to the collection
            
            .AddSingleton<Random>()                     // Add Random to the collection
            .AddSingleton(this.Configuration);          // Add the configuration to the collection
        }
    }
}
