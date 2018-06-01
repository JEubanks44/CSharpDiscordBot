using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord.Commands;
using Discord;
using Discord.WebSocket;
using ChaunceyDiscordBot;
using System.Net.NetworkInformation;
using System.Speech.Synthesis;
using System.Speech.Recognition;
using Discord.Audio;
namespace ChaunceyDiscordBot.Modules
{
    public class Misc : ModuleBase<SocketCommandContext>  //Inherits from the modulbase discord class
    {

        private EmbedBuilder embed = new EmbedBuilder();
        private Color red = new Color(255, 0, 0);
        DiscordSocketClient bot = Program.getClient();
        DataStorage ds = new DataStorage();
        SpeechSynthesizer synth = new SpeechSynthesizer();

        public async Task checkLevel(string ID)
        {
            bool levelUp = ds.checkLevelUp(ID);

            if(levelUp)
            {
                embed.WithTitle("LEVEL UP!");
                embed.WithDescription(Context.User.Username.ToString() + " is now level " + ds.getLevel(ID));
                embed.WithColor(red);
                embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                await Context.Channel.SendMessageAsync("", false, embed);
            }
            else
            {
                return;
            }
            
        }
        
        [Command("date")]
        public async Task Date()
        {  
            String date = DateTime.Now.ToString();
            embed.WithTitle("The Data and Time is: ");
            embed.WithDescription(date);
            embed.WithColor(red);
            await Context.Channel.SendMessageAsync("", false, embed);
            ds.addPoints(Context.User.Id.ToString(), 2);
            await checkLevel(Context.User.Id.ToString());
        }

        [Command("pick")]
        public async Task Pick([Remainder]string message)
        {
            string[] choices = message.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);




            Random rand = new Random();
            int r2 = rand.Next(0, choices.Length);

            embed.WithTitle("I pick this: ");
            embed.WithDescription(choices[r2]);
            embed.WithColor(red);
            embed.WithThumbnailUrl(Context.User.GetAvatarUrl());

            await Context.Channel.SendMessageAsync("", false, embed);
        }


        [Command("help")]
        public async Task Help()
        {
            string totalList = "";
            string echoDesc = "<!echo>: Repeats the message that follows it";
            string dateDesc = "<!date>: Retrieves the Date and Time";
            string pickDesc = "<!pick>: Randomly chooses one object from a list (Format: Option 1 | Option 2 | Option 3 | ...)";
            string ytDesc = "<!yt>: Returns a YouTube video based on the query that follows it";
            string spamDesc = "<!spam>: The bot repeats the message the requested amount of times (Format: message times)";
            string setIDDesc = "<!setID>: Adds users ID to the database";
            string setRealNameDesc = "<!setRealName>: Adds users real name to the database (Allows use of the !ping command with that name)";
            string pingDesc = "<!ping>: pings the users using real names set with !setRealName";
            string steamDesc = "<!steam>: Retrieves your steam profile info (Only if you have set Steam ID with <!setSteamID>)";

            List<string> helpList = new List<string>();
            helpList.Add(echoDesc);
            helpList.Add(dateDesc);
            helpList.Add(pickDesc);
            helpList.Add(ytDesc);
            helpList.Add(spamDesc);
            helpList.Add(setIDDesc);
            helpList.Add(setRealNameDesc);
            helpList.Add(pingDesc);
            foreach (string item in helpList)
            {
                totalList += item + "\n \n";
            }

            embed.WithTitle("Commands: ");
            embed.WithDescription(totalList);
            embed.WithColor(red);
            await Context.Channel.SendMessageAsync("", false, embed);
        }

        [Command("spam")]
        public async Task Spam([Remainder]string message)
        {
            Regex regex = new Regex(@"(.*?) (\d+)");
            Match match = regex.Match(message);
            string text = "";
            int times = 0;
            if (match.Success)
            {
                text = match.Groups[1].Value;
                Console.WriteLine("Text: " + text);
                times = Int32.Parse(match.Groups[2].Value);
                Console.WriteLine("Times: " + times);
            }
            if (times > 5)
            {
                await Context.Channel.SendMessageAsync("Nice Try Asshole: Spam set to 1");
                times = 1;
            }
            if (times <= 5)
            {
                for (int i = 0; i < times; i++)
                {
                    await Context.Channel.SendMessageAsync(text);
                }
            }

        }

        [Command("Ping")]
        public async Task Ping()
        {
            var messageTime = Context.Message.Timestamp;
            var botMessage = await Context.Channel.SendMessageAsync("Pinging...");
            var ping = (botMessage.Timestamp - messageTime).TotalMilliseconds;
           
            var speech = await Context.Channel.SendMessageAsync("Ping: " + ping.ToString() + " milliseconds");
            synth.Speak(speech.Content);
        }   

        [Command("Talk")]
        public async Task Talk()
        {
            synth.SelectVoiceByHints(VoiceGender.Male);
            synth.Speak("Bot Speech Test, Hello");
        }

    }
}
