// Copyright (c) Jordan Maxwell. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace BearDen.BearBot.Service.Modules
{
    /// <summary>
    /// 
    /// </summary>
    [Name("Fun")]
    [Summary("Silly Extra Commands")]
    [RequireContext(ContextType.Guild)]
    public class FunModule : ModuleBase<SocketCommandContext>
    {
        // These values are automatically filled in by the Dependency Injection system
        public Random RandomService { get; set; }

        [Command("say"), Alias("s")]
        [Summary("Makes BearBot say something")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public Task Say([Remainder] string text)
            => ReplyAsync(text);

        [Command("roll")]
        [Summary("Rolls a random number between a and b")]
        public async Task Roll(int a, int b) 
        {
            int output = RandomService.Next(a, b);
            await ReplyAsync($"You rolled: {output}");
        }

        #region Yeet Command
        [Command("yeet"), Priority(1)]
        [Summary("Yeets another user in chat.")]
        public async Task Yeet([Remainder] SocketGuildUser user)
        {
            if (user == Context.User || user == null) // User -> Yeets -> User A Invocation
            {
                await ReplyAsync($"{Context.User.Mention} yeets themselves.\nhttps://tenor.com/view/yeet-lion-king-simba-rafiki-throw-gif-16194362");
            }
            else if (user.Id == Context.Client.CurrentUser.Id) // User A -> Yeets -> Discord Bot Invocation
            {
                string[] possibleMessages = new string[] {
                    "*Is yeeted*",
                    "Weeeeee",
                    "*Tumbles through the air*",
                    "https://tenor.com/view/yeet-lion-king-simba-rafiki-throw-gif-16194362"
                };

                // Pick a random message and send it
                string message = possibleMessages[RandomService.Next(possibleMessages.Length)];
                await ReplyAsync(message);
            }
            else // Normal User A -> Yeets -> User B Invocation
            {
                await ReplyAsync($"{Context.User.Mention} yeets {user.Mention}.\nhttps://tenor.com/view/yeet-lion-king-simba-rafiki-throw-gif-16194362");
            }
        }

        [Command("yeet"), Priority(0)]
        [Summary("Yeets your self across chat.")]
        public async Task Yeet()
            => await this.Yeet(null);
        #endregion
    }
}
