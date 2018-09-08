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

//This class contains the timer based system that constantly polls for changes in key statuses for individual users
namespace ChaunceyDiscordBot
{
    
    public static class RepeatedTimer
    {
        private static Timer loopingTimer; //Timer
        private static SocketTextChannel channel; //Channel in which the general channel will be stored if found
        private static SteamUser steamInterface = new SteamUser(Utilities.GetAlert("STEAM_API_KEY")); //Steam API Profile Interface
        private static SteamStore steamStoreIntferace = new SteamStore(); //Steam API Store Interface
       
        
        private static DataStorage ds = new DataStorage();
        private static EmbedBuilder embed = new EmbedBuilder();
        private static CookieClient cookieClient = new CookieClient(new CookieConfig()
            {
                SteamKey = Utilities.GetAlert("STEAM_API_KEY")

            });
        private static Dictionary<string, string> dict = new Dictionary<string, string> ();
        private static int count = 0;
        public static Task StartTimer() //Task to begin the timer upon application start
        {
            loopingTimer = new Timer()
            {
                Interval = 2000, //Timer checks for changes in status of all users every 2 seconds
                AutoReset = true,
                Enabled = true,
            };
            loopingTimer.Elapsed += OnTimerTicked;
            Console.WriteLine("Timer Started");
            return Task.CompletedTask;
        }

        
        private static async void OnTimerTicked(object sender, ElapsedEventArgs e)
        {

            if (Global.Client == null) //Ensures the Discord Client is running before the timer begins
            {
                
                Console.WriteLine("Timer Ticked before Client was ready");
                return;
            }
            //Console.WriteLine("Timer Cycle Complete");
            try
            {
                if (count == 0) //On the first cycle of the timer get critical information and store it in the database
                {
                    foreach (var guild in Global.Client.Guilds) //For every server currently running the bot
                    {
                        foreach (var user in guild.Users) //For every user in each server
                        {
                            string userID = user.Id.ToString(); //Retrieve userID
                            string steamID = null;
                            while (steamID == null && !user.IsBot)
                            {
                                steamID = DataStorage.getSteamID(userID).Result; //Use userID to access users steam ID in the database
                            }

                            if (!user.IsBot && userID != null) //If the user is not a bot and the user exists
                            {
                                var gameResponse = steamInterface.GetPlayerSummaryAsync(Convert.ToUInt64(steamID));
                                string game = gameResponse.Result.Data.PlayingGameName;
                                if (game == null)
                                {
                                    game = "none";
                                }
                                DataStorage.setNowPlaying(userID, game); //Set the nowPlaying value in the database for this user to the game received from the Steam API
                            }
                        }
                    }
                }
                else
                {
                    foreach (var guild in Global.Client.Guilds) //For each server running the bot
                    {
                        foreach (var chan in guild.TextChannels) //For each channel in each server
                        {
                            if (chan.Name == "general") //Find the channel named general and assign its ID to a variable channel
                            {
                                channel = chan;
                            }
                        }
                        foreach (var user in guild.Users) //For each user in the server
                        {
                            //Retrieve the users steam ID from the database
                            string userID = user.Id.ToString();
                            string steamID = null;
                            while (steamID == null && !user.IsBot)
                            {
                                steamID = DataStorage.getSteamID(userID).Result;
                            }
                            
                            //If the user is not a bot
                            if (!user.IsBot)
                            {
                                //Get the data from the Steam API for the game that the current Steam ID is playing
                                var gameResponse = steamInterface.GetPlayerSummaryAsync(Convert.ToUInt64(steamID));
                                string game = gameResponse.Result.Data.PlayingGameName;
                                string gameID = gameResponse.Result.Data.PlayingGameId;
                                var store = steamStoreIntferace.GetStoreAppDetailsAsync(Convert.ToUInt32(gameID));
                                string imageURL = store.Result.HeaderImage;
                                
                      
                                if (game == null)
                                {
                                    game = "none";

                                }
                                string checkGame = DataStorage.getNowPlaying(userID); //Gets the nowPlaying value from the database
                                
                                //If the game in the current users row for nowPlaying in the database does not match the report from the Steam API
                                //A change has occured and the user is now playing a game so create an embed and post it to the channel as an alert.
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
                count = 1; //Increments the count variable to ensure the initial check does not happen twice.
            }
            catch(Exception exc)
            {
                
            }

        }
    }
    
}
