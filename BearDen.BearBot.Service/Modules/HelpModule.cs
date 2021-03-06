﻿// Copyright (c) Jordan Maxwell. All rights reserved.
// See LICENSE file in the project root for full license information.

using Discord;
using Discord.Commands;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace BearDen.BearBot.Service.Modules
{
    [Name("Help")]
    [Summary("Provides helpful information about the bot and available commands")]
    public class HelpModule : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _service;
        private readonly IConfigurationRoot _config;

        public HelpModule(CommandService service, IConfigurationRoot config)
        {
            _service = service;
            _config = config;
        }

        [Command("bot")]
        [Summary("Provides information about the bot")]
        public async Task Bot()
        {
            string message = string.Empty;
            message += "BearBot is a custom C# Discord bot for the BearDen community server.\n";

            var builder = new EmbedBuilder()
            {
                Color = new Color(114, 137, 218),
            };
            builder.AddField("Source", "https://github.com/thetestgame/BearBot");
            builder.AddField("Developers", "thetestgame");
            builder.AddField("Version", Constants.BotVersion);

            await ReplyAsync(message, false, builder.Build());
        }

        [Command("help")]
        [Summary("Provides helpful information about the available commands")]
        public async Task Help()
        {
            var builder = new EmbedBuilder()
            {
                Color = new Color(114, 137, 218),
            };

            foreach (var module in _service.Modules)
            {
                string description = null;
                foreach (var cmd in module.Commands)
                {
                    var result = await cmd.CheckPreconditionsAsync(Context);
                    if (result.IsSuccess)
                    {
                        if (cmd.Parameters.Count > 0)
                            description += $"!{cmd.Aliases.First()} <{string.Join(", ", cmd.Parameters.Select(p => p.Name))}> - {cmd.Summary}\n";
                        else
                            description += $"!{cmd.Aliases.First()} - {cmd.Summary}\n";
                    }
                }

                if (!string.IsNullOrWhiteSpace(description))
                {
                    builder.AddField(x =>
                    {
                        x.Name = $"{module.Name} - {module.Summary}";
                        x.Value = description;
                        x.IsInline = false;
                    });
                }
            }

            await ReplyAsync("", false, builder.Build());
        }

        [Command("help")]
        [Summary("Provides helpful information about a specific command")]
        public async Task Help(string command)
        {
            var result = _service.Search(Context, command);

            if (!result.IsSuccess)
            {
                await ReplyAsync($"Sorry, I couldn't find a command like **{command}**.");
                return;
            }

            var builder = new EmbedBuilder()
            {
                Color = new Color(114, 137, 218),
                Description = $"Here are some commands like **{command}**"
            };

            foreach (var match in result.Commands)
            {
                var cmd = match.Command;

                builder.AddField(x =>
                {
                    x.Name = string.Join(", ", cmd.Aliases);
                    x.Value = $"Parameters: {string.Join(", ", cmd.Parameters.Select(p => p.Name))}\n" +
                              $"Summary: {cmd.Summary}";
                    x.IsInline = false;
                });
            }

            await ReplyAsync("", false, builder.Build());
        }
    }
}
