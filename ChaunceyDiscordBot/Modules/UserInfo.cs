using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord.Commands;
using Discord;
using Discord.WebSocket;
//This Class Provides Bot Commands to allow setting of information in the database
namespace ChaunceyDiscordBot.Modules
{
    public class UserInfo : ModuleBase<SocketCommandContext> 
    {
        DataStorage ds = new DataStorage();
        [Command("setID")]
        public async Task setUserID()
        {
            await Context.Channel.SendMessageAsync("Testing ID Database");
            string userID = Context.User.Id.ToString();
            ds.setID(userID);
            
            
        }

       [Command("setRealName")]
       public async Task setRealName([Remainder]string message)
       {
            string userID = Context.User.Id.ToString();
            ds.setRealName(userID, message);
            await Context.Channel.SendMessageAsync("Real Name: " + message + " Added to database");
           
       }

       [Command("setNickName")]
       public async Task setNickName()
        {
            string userID = Context.User.Id.ToString();
            string nickName = Context.Guild.GetUser(Context.User.Id).Nickname;
            if (nickName == null || nickName == "")
            {
                nickName = Context.User.Username;
            }
            ds.setNickName(userID, nickName);
            await Context.Channel.SendMessageAsync("Nick Name: " + nickName + "Added to database");
        }

        [Command("setSteamID")]
        public async Task setSteamID([Remainder]string steamID)
        {
            string userID = Context.User.Id.ToString();
            ds.setSteamID(userID, steamID);
            await Context.Channel.SendMessageAsync("Steam ID set");
        }

        [Alias("setlfm", "setLFM")]
        [Command("setLastFM")]
        public async Task setLastFM([Remainder] string userName)
        {
            string userID = Context.User.Id.ToString();
            ds.setLastFM(userID, userName);
            await Context.Channel.SendMessageAsync("Last FM ID set");
        }
        
    }
}
