using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web;
using SpotifyAPI.Local;
using SpotifyAPI.Web.Models;
using SpotifyAPI.Local.Models;
using SpotifyAPI.Web.Enums;
using Discord;
using Discord.Commands;
namespace ChaunceyDiscordBot.Modules
{
    public class Spotify : ModuleBase<SocketCommandContext>
    {
        static ClientCredentialsAuth auth;

        
        WebAPIFactory webAPIFactory = new WebAPIFactory(
           "http://localhost",
           8080,
           "c649fe14327b4079aad22a7609a1f4ee",
           Scope.UserReadPrivate,
           TimeSpan.FromSeconds(20)
        );
       
        [Command("song")]
       public async Task getSong([Remainder] string query)
        {
            var spotify = await webAPIFactory.GetWebApi();
            var searchSpot = spotify.SearchItems(query, SearchType.Track, 10);
            var trackList = searchSpot.Tracks.Items;
            var track = trackList[0];
            Console.WriteLine(track.Name);
            await Context.Channel.SendMessageAsync(track.Href);
        }
        
    }
}
