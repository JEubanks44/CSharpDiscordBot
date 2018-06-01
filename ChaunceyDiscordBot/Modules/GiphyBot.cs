using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Cookie.Giphy.Models;
using Cookie.Giphy;
using Cookie;
namespace ChaunceyDiscordBot.Modules
{
    public class GiphyBot : ModuleBase<SocketCommandContext>
    {
        DataStorage ds = new DataStorage();
        EmbedBuilder embed = new EmbedBuilder();
        CookieClient cookieClient = new CookieClient(new CookieConfig()
        {
            GiphyKey = Utilities.GetAlert("GIPHY_API_KEY"),
            
        });

        public async Task checkLevel(string ID)
        {
            bool levelUp = ds.checkLevelUp(ID);

            if (levelUp)
            {
                embed.WithTitle("LEVEL UP!");
                embed.WithDescription(Context.User.Username.ToString() + " is now level " + ds.getLevel(ID));
                embed.WithColor(255, 0, 0);
                embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                await Context.Channel.SendMessageAsync("", false, embed);
            }
            else
            {
                return;
            }

        }

        [Alias("gif", "gifkey", "gkey", "gifsearch", "giphy")]
        [Command("gif")]
        public async Task getGifKeyword([Remainder] string search)
        {
            Random rand = new Random();
            int i = 0;
            var gifSearch = await cookieClient.Giphy.SearchAsync(search, 25, Rating.Unrated).ConfigureAwait(false); //This is broke?? :>(
            if (gifSearch.Datum[0].EmbedURL != null)
            {
                i = rand.Next(gifSearch.Pagination.Count);
                Console.WriteLine(gifSearch.Pagination.Count);
                Console.WriteLine(gifSearch.Datum[i].EmbedURL);
                //embed.WithColor(255, 0, 0);
                //embed.WithTitle("GIF: " + search);
                //embed.WithThumbnailUrl(gifSearch.Datum[i].EmbedURL);
                var roles = Context.Guild.Roles;
                await Context.Channel.SendMessageAsync(gifSearch.Datum[i].EmbedURL);
                
            }
        }

        public async Task<string> getGifURL(string search)
        {
            var gifSearch = await cookieClient.Giphy.SearchAsync(search, 10, Rating.Unrated).ConfigureAwait(false);
            string embedURL = gifSearch.Datum[0].EmbedURL;
            return embedURL;
            
        }

       
    }
}
