using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord;
using Discord.WebSocket;
using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Helpers;
using System.Globalization;
//This class is bot commands to communicate with the Last.FM API
namespace ChaunceyDiscordBot.Modules
{
    public class LastFM : ModuleBase<SocketCommandContext>
    {
        DataStorage ds = new DataStorage();
        LastfmClient lfmClient = new LastfmClient(Utilities.GetAlert("LASTFM_API_KEY"), Utilities.GetAlert("LASTFM_API_SECRET"));
        EmbedBuilder embed = new EmbedBuilder();
        TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
        [Command("album")]
        public async Task lastFMInfo([Remainder]string info)
        {
            try //Prints the error message to Discord if last fm client returns null
            { 
                string[] infoArray = info.Split('|');
                string artist = textInfo.ToTitleCase(infoArray[0].Trim());
                string album = textInfo.ToTitleCase(infoArray[1].Trim());
                string userName = ds.getLastFM(Context.User.Id.ToString());
                var lastFMResults = lfmClient.Album.GetInfoAsync(artist, album).Result.Content;
                string textToSend = "\nTrack List: \n";
                foreach(var name in lastFMResults.Tracks)
                {
                    textToSend += name.Name + "\n";
                }
                textToSend += "\nLastFM: " + lastFMResults.Url + "\n";
                
            
            


                embed.WithTitle("Info for " + artist + " - " + album);
                embed.WithImageUrl(lastFMResults.Images.ExtraLarge.AbsoluteUri);
                embed.WithDescription(textToSend);


                await Context.Channel.SendMessageAsync("", false, embed);
            }
            catch(Exception e)
            {
                await Context.Channel.SendMessageAsync("Album not found!!!");
            }
        }

        [Command("artist")]
        public async Task lastFMArtist([Remainder]string artist)
        {
            try
            {
                artist = textInfo.ToTitleCase(artist);
                Console.WriteLine(artist);
                string textToSend = "Bio: \n";
                var lastFMResults = lfmClient.Artist.GetInfoAsync(artist).Result.Content;
                textToSend += lastFMResults.Bio.Summary.Substring(0, lastFMResults.Bio.Summary.LastIndexOf("<a"));
                textToSend += "\n\nLastFM: " + lastFMResults.Url;
                embed.WithTitle("Info for " + artist);
                embed.WithImageUrl(lastFMResults.MainImage.ExtraLarge.AbsoluteUri);
                embed.WithDescription(textToSend);
                await Context.Channel.SendMessageAsync("", false, embed);
            }
            catch(Exception e)
            {
                await Context.Channel.SendMessageAsync("Artist not Found");
            }
        }

        [Command("lastfm")]
        public async Task lastFM()
        {
            try
            {
                string userName = ds.getLastFM(Context.User.Id.ToString());
                string textToSend = "";
                var recentScrobbles = lfmClient.User.GetRecentScrobbles(userName).Result.Content;
                var topArtists = lfmClient.User.GetTopArtists(userName, 0).Result.Content;
                var userInfo = lfmClient.User.GetInfoAsync(userName).Result.Content;
                textToSend += "Recent Scrobbles\n";
                foreach(var title in recentScrobbles)
                {
                    textToSend += title.ArtistName + " - " + title.Name +"\n";
                }
                textToSend += "\n\nTop 5 Artists:\n";
                foreach (var title in topArtists)
                {
                    textToSend += title.Name + "\n";
                }
                embed.WithTitle("Info for: " + userInfo.FullName);
                embed.WithImageUrl(userInfo.Avatar.ExtraLarge.AbsoluteUri);
                embed.WithDescription(textToSend);
                await Context.Channel.SendMessageAsync("", false, embed);
            }
            catch(Exception e)
            {

            }
        }
    }
}
