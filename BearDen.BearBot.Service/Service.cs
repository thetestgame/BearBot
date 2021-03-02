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
    /// Represents root service specific configuration options
    /// </summary>
    public class ServiceConfiguration
    {
        /// <summary>
        /// Represents the total number of Discord client shards to spawn.
        /// it's recommended to have 1 shard per 1500-2000 guilds the bot is in.
        /// </summary>
        public int TotalShards { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int MaxCacheSize { get; set; }
    }

    /// <summary>
    /// Primary service object for the BearBot application. Handles the management 
    /// and instantiation of the required components and services
    /// </summary>
    public class Service
    {
        public IConfigurationRoot Configuration { get; }

        public Service(string[] args)
        {
            // Load our application configuration file
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("config.json");
            this.Configuration = builder.Build();
        }

        /// <summary>
        /// Starts teh application service
        /// </summary>
        /// <param name="args">Command line startup arguments</param>
        public static async Task RunAsync(string[] args)
        {
            var startup = new Service(args);
            await startup.RunAsync();
        }

        /// <summary>
        /// Runs the servic until stopped
        /// </summary>
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
        /// Configures the application ServiceCollection with required services
        /// </summary>
        /// <param name="services">ServiceCollection to configure</param>
        private void ConfigureServices(IServiceCollection services)
        {
            // Add the base Discord socket client service to the collection
            ServiceConfiguration config = this.Configuration.GetSection("Service").Get<ServiceConfiguration>();
            services.AddSingleton(new DiscordShardedClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose,
                MessageCacheSize = config.MaxCacheSize,
                TotalShards = config.TotalShards
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
            .AddSingleton<StartupService>()                                         // Add the startup service to the collection    
            .AddSingleton<LoggingService>()                                         // Add the logging service to the collection

            .AddSingleton<Random>()                                                 // Add Random to the collection
            .AddSingleton(Configuration);                                           // Add the configuration to the collection
        }
    }
}
