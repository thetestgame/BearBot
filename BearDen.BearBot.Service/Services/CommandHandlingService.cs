// Copyright (c) Jordan Maxwell. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace BearDen.BearBot.Service.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class CommandHandlingService
    {
        private readonly CommandService commands;
        private readonly DiscordShardedClient discord;
        private readonly IServiceProvider services;

        public CommandHandlingService(IServiceProvider services)
        {
            this.commands = services.GetRequiredService<CommandService>();
            this.discord = services.GetRequiredService<DiscordShardedClient>();
            this.services = services;

            // Hook CommandExecuted to handle post-command-execution logic.
            this.commands.CommandExecuted += CommandExecutedAsync;
            // Hook MessageReceived so we can process each message to see
            // if it qualifies as a command.
            this.discord.MessageReceived += MessageReceivedAsync;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task InitializeAsync()
        {
            // Register modules that are public and inherit ModuleBase<T>.
            await this.commands.AddModulesAsync(Assembly.GetEntryAssembly(), this.services);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rawMessage"></param>
        /// <returns></returns>
        public async Task MessageReceivedAsync(SocketMessage rawMessage)
        {
            // Ignore system messages, or messages from other bots
            if (!(rawMessage is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;


            // Perform a basic check to verify the incoming message is a command invocation 
            var argPos = 0;
            if (!message.HasCharPrefix('!', ref argPos) && (!message.HasMentionPrefix(this.discord.CurrentUser, ref argPos))) return;

            var context = new ShardedCommandContext(this.discord, message);
            await this.commands.ExecuteAsync(context, argPos, this.services);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="context"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            // command is unspecified when there was a search failure (command not found); we don't care about these errors
            if (!command.IsSpecified)
                return;

            // the command was successful, we don't care about this result, unless we want to log that a command succeeded.
            if (result.IsSuccess)
                return;

            // the command failed, let's notify the user that something happened.
            await context.Channel.SendMessageAsync($"error: {result}");
        }
    }
}
