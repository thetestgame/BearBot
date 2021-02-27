// Copyright (c) Jordan Maxwell. All rights reserved.
// See LICENSE file in the project root for full license information.

using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BearDen.BearBot.Service.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class LoggingService
    {
        private readonly DiscordSocketClient discord;
        private readonly CommandService commands;

        private string logDirectory { get; }
        private string logFile => Path.Combine(logDirectory, $"{DateTime.UtcNow.ToString("yyyy-MM-dd")}.txt");

        // DiscordSocketClient and CommandService are injected automatically from the IServiceProvider
        public LoggingService(DiscordSocketClient discord, CommandService commands)
        {
            this.logDirectory = Path.Combine(AppContext.BaseDirectory, "logs");

            this.discord = discord;
            this.commands = commands;

            this.discord.Log += OnLogAsync;
            this.commands.Log += OnLogAsync;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        private Task OnLogAsync(LogMessage msg)
        {
            if (!Directory.Exists(this.logDirectory))     // Create the log directory if it doesn't exist
                Directory.CreateDirectory(this.logDirectory);
            if (!File.Exists(this.logFile))               // Create today's log file if it doesn't exist
                File.Create(this.logFile).Dispose();

            string logText = $"{DateTime.UtcNow.ToString("hh:mm:ss")} [{msg.Severity}] {msg.Source}: {msg.Exception?.ToString() ?? msg.Message}";
            //File.AppendAllText(this.logFile, logText + "\n");     // Write the log text to a file

            return Console.Out.WriteLineAsync(logText);       // Write the log text to the console
        }
    }
}
