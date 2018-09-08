using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord;
using Discord.WebSocket;
using DotLastFm.Api;
using DotLastFm.Models;

//This class is bot commands to communicate with the Last.FM API
namespace ChaunceyDiscordBot.Modules
{
    class LastFM : ModuleBase<SocketCommandContext>
    {
        DataStorage ds = new DataStorage();
        EmbedBuilder embed = new EmbedBuilder();
        LastFM lastFM = new LastFM();
        
        [Command("lastfm")]
        public async Task lastFMInfo()
        {
            
        }
    }
}
