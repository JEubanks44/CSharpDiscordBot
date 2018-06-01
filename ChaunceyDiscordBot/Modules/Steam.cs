using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord;
using Discord.WebSocket;
using Cookie;
using Cookie.Steam.Models;
using Cookie.Steam;
using SteamWebAPI2;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Models;
using SteamWebAPI2.Exceptions;
using SteamWebAPI2.Utilities;
namespace ChaunceyDiscordBot.Modules
{
    public class Steam : ModuleBase<SocketCommandContext>
    {
        private System.Threading.Timer timer;
        DataStorage ds = new DataStorage();
        SteamUser steamInterface = new SteamUser(Utilities.GetAlert("STEAM_API_KEY"));
        SteamStore steamStoreInterface = new SteamStore();
        EmbedBuilder embed = new EmbedBuilder();
        CookieClient cookieClient = new CookieClient(new CookieConfig()
        {
            SteamKey = Utilities.GetAlert("STEAM_API_KEY")

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

        [Alias("steamp", "profilesteam")]
        [Command("steam")]
        public async Task SteamInfo()
        {
            
            string userID = Context.User.Id.ToString();
            string steamID = await DataStorage.getSteamID(userID);

            var playerSummaryResponse = await steamInterface.GetPlayerSummaryAsync(Convert.ToUInt64(steamID));
            var playerSummaryData = playerSummaryResponse.Data;
            

            string nickName = playerSummaryData.Nickname;
            string avatar = playerSummaryData.AvatarFullUrl;
            string gamePlaying = playerSummaryData.PlayingGameName;
            if (gamePlaying == null)
            {
                gamePlaying = "Nothing";
            }
            string profile = playerSummaryData.ProfileUrl;
            string userStatus = playerSummaryData.UserStatus.ToString();
            string createdDate = playerSummaryData.AccountCreatedDate.ToString();

            string description = "";
            description += "Currently Playing: " + gamePlaying + "\n";
            description += "User Status: " + userStatus + "\n";
            description += "Account Created: " + createdDate + "\n";
            embed.WithTitle(nickName);
            embed.WithDescription(description);
            embed.WithUrl(profile);
            embed.WithThumbnailUrl(avatar);
            embed.WithColor(0, 173, 238);
            await Context.Channel.SendMessageAsync("", false, embed);
            ds.addPoints(userID, 5);
            await checkLevel(userID);
        }

        [Command("Playing")]
        public async Task CurrentlyPlaying()
        {
            string userID = Context.User.Id.ToString();
            string steamID = await DataStorage.getSteamID(userID);
            Console.WriteLine(steamID);
            
            
            var playerSummaryResponse = await steamInterface.GetPlayerSummaryAsync(Convert.ToUInt64(steamID)).ConfigureAwait(false);
            var playerSummaryData = playerSummaryResponse.Data;
            string gameID = playerSummaryData.PlayingGameId;
            var cookieSteam = cookieClient.Steam.RecentGamesAsync(steamID);
            var gameList = cookieSteam.Result.RecentGames.GamesList;
            string gameLogo = "";
            foreach (var game in gameList)
            {
                if (game.AppId.ToString() == gameID)
                {
                   gameLogo = game.LogoUrl;
                }
            }
            
            string gameName = playerSummaryData.PlayingGameName;
            if (gameName == null)
            {
                gameName = "Nothing";
            }
            if (gameID == null)
            {
                gameID = "";
            }
            Console.WriteLine(gameID);
            
            string gameImage = "https://steamcdn-a.akamaihd.net/steamcommunity/public/images/apps/" + gameID.Trim() + "/" + gameLogo.Trim() + ".jpg";
            Console.WriteLine(gameImage);
            string name = ds.getNickName(userID);
            embed.WithImageUrl(gameImage);
            embed.WithTitle(name + " Is Playing: ");
            embed.WithDescription(gameName);
            embed.WithColor(0, 173, 238);
            await Context.Channel.SendMessageAsync("", false, embed);
            
        }


        [Command("recent")]
        public async Task RecentGames()
        {
            string userID = Context.User.Id.ToString();
            string steamID = await DataStorage.getSteamID(userID);

            var cookieSteam = cookieClient.Steam.RecentGamesAsync(steamID);
            var gameList = cookieSteam.Result.RecentGames.GamesList;
            for (int i = 0; i < 5; i++)
            {
                string gameID = gameList[i].AppId.ToString();
                string gameLogo = gameList[i].LogoUrl;
                string gameTitle = gameList[i].Name;
                string gameImage = "https://steamcdn-a.akamaihd.net/steamcommunity/public/images/apps/" + gameID.Trim() + "/" + gameLogo.Trim() + ".jpg";
                string playTimeTotal = "Total Playtime: " + (gameList[i].TotalPlaytime / 60).ToString() + " Hours";
                string playTimeWeeks = "Playtime Last 2 Weeks: " + (gameList[i].Playtime2Weeks / 60).ToString() + " Hours";

                embed.WithTitle(gameTitle);
                embed.WithColor(0, 173, 238);
                embed.WithImageUrl(gameImage);
                embed.WithDescription(playTimeTotal + "\n" + playTimeWeeks);

                await Context.Channel.SendMessageAsync("", false, embed);
            }
        }

        [Alias("steamfg")]
        [Command("steamFeaturedGames")]
        public async Task StoreFeaturedGames()
        {
            string userID = Context.User.Id.ToString();
            string steamID = await DataStorage.getSteamID(userID);

            var featured = steamStoreInterface.GetStoreFeaturedProductsAsync().Result;
            var featuredGame = featured.FeaturedWin;
            for (int i = 0; i < 5; i++)
            {
                string gameName = featuredGame[i].Name;
                string gameID = featuredGame[i].Id.ToString();
                double gamePrice = (double)featuredGame[i].FinalPrice / 100;
                string gameImage = featuredGame[i].LargeCapsuleImage;
                string gameDiscount = featuredGame[i].DiscountPercent.ToString();
                double gamePriceOriginal = (double)featuredGame[i].OriginalPrice / 100;

                string url = "https://store.steampowered.com/app/" + gameID;
                string description = "";
                description += "Original Price: $" + gamePriceOriginal + "\n";
                description += "Discount Price: $" + gamePrice + "\n";
                description += "Discount Percentage: " + gameDiscount + "\n";
                embed.WithTitle(gameName);
                embed.WithDescription(description);
                embed.WithUrl(url);
                embed.WithColor(0, 173, 238);
                embed.WithImageUrl(gameImage);

                await Context.Channel.SendMessageAsync("", false, embed);

            }
        } 
        
        
    }
}
