using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord;
using Discord.Commands;
using Discord.Audio;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using ChaunceyDiscordBot.Services;
using System.Net.Http;
using ChaunceyDiscordBot.Modules;
using ChaunceyDiscordBot;
namespace ChaunceyDiscordBot
{
    class Program
    {
        private static DiscordSocketClient _client;
        CommandHandler _handler;

        public static DiscordSocketClient getClient()
        {
            return _client;
        }
        static void Main(string[] args)
        => new Program().StartAsync().GetAwaiter().GetResult(); //Start Async funtion instead of main

        public async Task StartAsync() //Begins the main loop of the program
        {
            var services = ConfigureServices();
            
            if (Config.bot.token == "" || Config.bot.token == null)
            {
                return;
            }

            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose
            });

            _client.Log += Log;
            _client.Ready += RepeatedTimer.StartTimer; //Starts the timer polling class
            await _client.LoginAsync(TokenType.Bot, Config.bot.token);  //Logs in bot
            await _client.StartAsync(); //Starts bot
            Global.Client = _client;
            _handler = new CommandHandler();

            await _handler.InitializeAsync(_client);

            
           
            

            await Task.Delay(-1);
        }

        private async Task Log(LogMessage message)
        {
            Console.WriteLine(message.Message.ToString());
            
        }

        private IServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<AudioService>().BuildServiceProvider();
        }



    }
}
