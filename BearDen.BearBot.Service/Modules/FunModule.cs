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

        [Command("hug")]
        [Summary("Hugs another user in chat")]
        public async Task Hug([Remainder] SocketGuildUser user)
        {
            string[] possibleImages = new string[] {
                "https://media1.tenor.com/images/08de7ad3dcac4e10d27b2c203841a99f/tenor.gif?itemid=4885268",
                "https://tenor.com/view/dog-hug-bff-bestfriend-friend-gif-9512793",
                "https://tenor.com/view/red-panda-tackle-surprised-hug-cute-gif-12661024",
                "https://tenor.com/view/milk-and-mocha-hug-cute-kawaii-love-gif-12535134",
                "https://tenor.com/view/friends-hug-joey-jump-catch-gif-5381990",
                "https://tenor.com/view/warm-hug-gif-10592083",
                "https://tenor.com/view/milk-and-mocha-bear-couple-line-hug-cant-breathe-gif-12687187",
                "https://tenor.com/view/bearhugs-hugs-cute-gif-4104176"
            };

            string imageUrl = possibleImages[RandomService.Next(possibleImages.Length)];
            await ReplyAsync($"{Context.User.Mention} hugs {user.Mention}\n{imageUrl}");
        }

        [Command("punt")]
        [Summary("Punts another user in chat.")]
        public async Task Punt([Remainder] SocketGuildUser user)
        {
            // Pick a random image prior to "punting" the user
            string[] possibleImages = new string[] {
                "https://tenor.com/view/punt-kick-baby-grandma-gif-8217719",
                "https://tenor.com/view/call-me-kevin-kick-the-baby-kick-kicking-punting-gif-18237653",
                "https://tenor.com/view/austin-powers-minime-mini-me-gif-6077759",
            };
            string imageUrl = possibleImages[RandomService.Next(possibleImages.Length)];

            if (user == Context.User || user == null) // User -> Punts -> User A Invocation
            {
                await ReplyAsync($"{Context.User.Mention} punts themselves.\n{imageUrl}");
            }
            else if (user.Id == Context.Client.CurrentUser.Id) // User A -> Punts -> Discord Bot Invocation
            {
                string[] possibleMessages = new string[] {
                    "*Is punted*",
                    "Weeeeee",
                    "*Tumbles through the air*",
                };

                // Pick a random message and send it
                string message = possibleMessages[RandomService.Next(possibleMessages.Length)];
                await ReplyAsync($"{message}\n{imageUrl}");
            }
            else // Normal User A -> Punts -> User B Invocation
            {
                await ReplyAsync($"{Context.User.Mention} punts {user.Mention}.\n{imageUrl}");
            }
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
