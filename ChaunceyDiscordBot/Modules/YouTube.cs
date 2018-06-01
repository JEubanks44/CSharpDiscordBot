using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Discord.Commands;
using Discord;
using Discord.WebSocket;
namespace ChaunceyDiscordBot.Modules
{
    public class YouTube : ModuleBase<SocketCommandContext>
    {
        
        DataStorage ds = new DataStorage();
        EmbedBuilder embed = new EmbedBuilder();
        private YouTubeService youtubeService;
        public YouTube()
        {

        }

        public async Task checkLevel(string ID)
        {
            bool levelUp = ds.checkLevelUp(ID);

            if (levelUp)
            {
                embed.WithTitle("LEVEL UP!");
                embed.WithDescription(Context.User.Username.ToString() + " is now level " + ds.getLevel(ID));
                embed.WithColor(255, 0 , 0);
                embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                await Context.Channel.SendMessageAsync("", false, embed);
            }
            else
            {
                return;
            }

        }

        public YouTubeService getService()
        {
            return youtubeService;
        }

        [Alias("youtube", "tube", "ytkey")]
        [Command("yt")]
        public async Task searchKeyword([Remainder]string message)
        {

            youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = Utilities.GetAlert("YOUTUBE_API_KEY"),
                ApplicationName = this.GetType().ToString()
            });

            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.Q = message;
            searchListRequest.MaxResults = 10;

            var searchListResponse = await searchListRequest.ExecuteAsync();

            List<string> videos = new List<string>();

            foreach (var result in searchListResponse.Items)
            {
                videos.Add(result.Id.VideoId);
                Console.WriteLine(result.Id.VideoId);
            }
            String URL = "https://www.youtube.com/watch?v=";
            if (videos[0] != null)
            {
                URL += videos[0];
                Console.WriteLine("videos[0]");
            }
            else
            {
                URL += videos[1];
                Console.WriteLine("videos[1]");
            }
            
            
            await Context.Channel.SendMessageAsync(URL);
            string userID = Context.User.Id.ToString();
            ds.addPoints(userID, 10);
          
            await checkLevel(userID);
            
        }

        

    }
}
