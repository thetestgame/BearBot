// Copyright (c) Jordan Maxwell. All rights reserved.
// See LICENSE file in the project root for full license information.

using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace BearDen.BearBot.Service.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class StartupService
    {
        private readonly IServiceProvider provider;
        private readonly DiscordSocketClient discord;
        private readonly CommandService commands;
        private readonly IConfigurationRoot config;

        // DiscordSocketClient, CommandService, and IConfigurationRoot are injected automatically from the IServiceProvider
        public StartupService(
            IServiceProvider provider,
            DiscordSocketClient discord,
            CommandService commands,
            IConfigurationRoot config)
        {
            this.provider = provider;
            this.config = config;
            this.discord = discord;
            this.commands = commands;
        }

        public async Task StartAsync()
        {
            string discordToken = Environment.GetEnvironmentVariable(Constants.BotTokenEnvironmentKey); // Get the discord token from the environment variable
            await this.discord.LoginAsync(TokenType.Bot, discordToken);                                 // Login to discord
            await this.discord.StartAsync();                                                            // Connect to the websocket

            await this.commands.AddModulesAsync(Assembly.GetEntryAssembly(), this.provider);            // Load commands and modules into the command service
        }
    }
}
