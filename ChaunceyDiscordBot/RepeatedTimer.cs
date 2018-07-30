using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Discord.Commands;
using Discord;
using Steam;
using SteamWebAPI2;
using Discord.WebSocket;
using SteamWebAPI2.Models;
using SteamWebAPI2.Interfaces;
using Cookie.Steam;
using Cookie.Steam.Models;
using Cookie;
using System.Text.RegularExpressions;
namespace ChaunceyDiscordBot
{
    
    public static class RepeatedTimer
    {
        private static Timer loopingTimer;
        private static SocketTextChannel channel;
        private static SteamUser steamInterface = new SteamUser(Utilities.GetAlert("STEAM_API_KEY"));
        private static SteamStore steamStoreIntferace = new SteamStore();
       
        
        private static DataStorage ds = new DataStorage();
        private static EmbedBuilder embed = new EmbedBuilder();
        private static CookieClient cookieClient = new CookieClient(new CookieConfig()
            {
                SteamKey = Utilities.GetAlert("STEAM_API_KEY")

            });
        private static Dictionary<string, string> dict = new Dictionary<string, string> ();
        private static int count = 0;
        public static Task StartTimer()
        {
            loopingTimer = new Timer()
            {
                Interval = 2000,
                AutoReset = true,
                Enabled = true,
            };
            loopingTimer.Elapsed += OnTimerTicked;
            Console.WriteLine("Timer Started");
            return Task.CompletedTask;
        }

        
        private static async void OnTimerTicked(object sender, ElapsedEventArgs e)
        {

            if (Global.Client == null)
            {
                
                Console.WriteLine("Timer Ticked before Client was ready");
                return;
            }
            //Console.WriteLine("Timer Cycle Complete");
            try
            {


                if (count == 0)
                {
                    foreach (var guild in Global.Client.Guilds)
                    {
                        foreach (var user in guild.Users)
                        {
                            string userID = user.Id.ToString();
                            string steamID = null;
                            while (steamID == null && !user.IsBot)
                            {
                                steamID = DataStorage.getSteamID(userID).Result;
                            }

                            if (!user.IsBot && userID != null)
                            {
                                var gameResponse = steamInterface.GetPlayerSummaryAsync(Convert.ToUInt64(steamID));
                                string game = gameResponse.Result.Data.PlayingGameName;
                                if (game == null)
                                {
                                    game = "none";
                                }
                                DataStorage.setNowPlaying(userID, game);
                            }
                        }
                    }
                }
                else
                {
                    foreach (var guild in Global.Client.Guilds)
                    {
                        foreach (var chan in guild.TextChannels)
                        {
                            if (chan.Name == "general")
                            {
                                channel = chan;
                            }
                        }
                        foreach (var user in guild.Users)
                        {
                            string userID = user.Id.ToString();
                            string steamID = null;
                            while (steamID == null && !user.IsBot)
                            {
                                steamID = DataStorage.getSteamID(userID).Result;
                            }
                            
                            if (!user.IsBot)
                            {
                                
                                var gameResponse = steamInterface.GetPlayerSummaryAsync(Convert.ToUInt64(steamID));
                                string game = gameResponse.Result.Data.PlayingGameName;
                                string gameID = gameResponse.Result.Data.PlayingGameId;
                                var store = steamStoreIntferace.GetStoreAppDetailsAsync(Convert.ToUInt32(gameID));
                                string imageURL = store.Result.HeaderImage;
                                
                      
                                if (game == null)
                                {
                                    game = "none";

                                }
                                string checkGame = DataStorage.getNowPlaying(userID);
                                
                                if (game.Trim() != checkGame.Trim())
                                {
                                    
                                    embed.WithTitle(user.Username + " Is Now Playing: " + game);
                                    embed.WithColor(0, 173, 238);
                                    
                                    if (imageURL == null)
                                    {
                                        game = "none";
                                        gameID = "none";
                                        imageURL = "none";
                                        
                                    }
                                    DataStorage.setNowPlaying(userID, game);

                                    if(game != "none")
                                    {
                                        embed.WithImageUrl(imageURL);
                                        
                                        await channel.SendMessageAsync("", false, embed);
                                    }
                                    

                                }
                            }
                        }
                    }
                }
                count = 1;
            }
            catch(Exception exc)
            {
                
            }

        }
    }
    
}
