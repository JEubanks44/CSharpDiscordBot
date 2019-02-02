using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord.Commands;
using Discord;
using System.Reflection;
namespace ChaunceyDiscordBot
{
    class CommandHandler : ModuleBase<SocketCommandContext>
    {
        DiscordSocketClient _client;
        EmbedBuilder embed = new EmbedBuilder();
        CommandService _service;
        DataStorage ds = new DataStorage();
        public async Task InitializeAsync(DiscordSocketClient client)
        {
            _client = client;
            _service = new CommandService();

            await _service.AddModulesAsync(Assembly.GetEntryAssembly());
            
            _client.MessageReceived += HandleCommandAsync;
        }

        private async Task HandleCommandAsync(SocketMessage sockMessage)
        {
            var message = sockMessage as SocketUserMessage;
            var context = new SocketCommandContext(_client, message);

            if(message == null)
            {
                return;
            }
            else
            {
                //ds.addPoints(message.Author.Id.ToString(), 10);
            }

            if (context.User.IsBot) return;
            ds.addPoints(message.Author.Id.ToString(), 5);
            if(ds.checkLevelUp(message.Author.Id.ToString()))
            {
                embed.WithTitle("LEVEL UP!");
                embed.WithDescription(message.Author.Username.ToString() + " is now level " + ds.getLevel(message.Author.Id.ToString()));
                embed.WithColor(255, 0, 0);
                embed.WithThumbnailUrl(message.Author.GetAvatarUrl());
                await context.Channel.SendMessageAsync("", false, embed);
            }
            Console.WriteLine(message.Author.Id.ToString());

            int argPos = 0;
            
            if(message.HasStringPrefix(Config.bot.cmdPrefix, ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                var result = await _service.ExecuteAsync(context, argPos);

                if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                {
                    Console.WriteLine(result.ErrorReason);
                }
            }

            
        }
    }
}
