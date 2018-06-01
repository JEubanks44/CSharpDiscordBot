using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Discord.Commands;
using Discord;
namespace ChaunceyDiscordBot.Modules
{
    public class Mention : ModuleBase<SocketCommandContext>
    {
        private EmbedBuilder embed = new EmbedBuilder();
        private Color red = new Color(255, 0, 0);
        static SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["connectionString"]);
        [Command("ping")]
        public async Task pingName([Remainder]string message)
        {
            conn.Open();
            SqlCommand getPingIDs;
            string userPings = "Pinging: ";
            string summonedUsers = "";
            string extraText = "";
            string[] summons = message.Split(' ');
            foreach (string sum in summons)
            {
                
                getPingIDs = new SqlCommand("SELECT userID FROM UserInfo WHERE realName = @REAL");
                getPingIDs.Connection = conn;
                try
                {
                    getPingIDs.Parameters.AddWithValue("@REAL", sum);
                    string pingID = getPingIDs.ExecuteScalar().ToString().Trim();
                    string formatSummon = "<@" + pingID + "> ";
                    summonedUsers += formatSummon;
                }
                catch (Exception ex)
                {
                    extraText += sum + " ";
                    Console.WriteLine(ex.Message);
                }



                
            }

            conn.Close();
            await Context.Channel.SendMessageAsync(userPings + summonedUsers);
            await Context.Channel.SendMessageAsync(extraText);
            
        }

        [Command("gam")]
        public async Task pingGamPlayers()
        {
            conn.Open();
            SqlCommand getGamIDs = new SqlCommand("SELECT * FROM UserInfo WHERE gam = @param1");
            getGamIDs.Connection = conn;
            getGamIDs.Parameters.AddWithValue("@param1", 1);
            string gamMessage = "Time to Gam: ";
            string pingedUsers = "";
            List<string> pings = new List<string>();

            using (SqlDataReader reader = getGamIDs.ExecuteReader())
            {
                while (reader.Read())
                {
                    pings.Add(reader["userID"].ToString().Trim());
                }
               
            }

            foreach (string ping in pings)
            {
                pingedUsers += "<@" + ping + "> ";
                    
            }

            await Context.Channel.SendMessageAsync(gamMessage + pingedUsers);

            conn.Close();
        }

        /*
        [Command("setPFP")]
        public async Task setNewProfilePicture([Remainder]string message)
        {
            Image img;
            System.Drawing.Image image = null;
            try
            {
                System.Net.HttpWebRequest webRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(message);
                webRequest.AllowWriteStreamBuffering = true;
                webRequest.Timeout = 30000;

                System.Net.WebResponse webResponse = webRequest.GetResponse();

                System.IO.Stream stream = webResponse.GetResponseStream();

                image = System.Drawing.Image.FromStream(stream);
                img = new Image(stream);
                webResponse.Close();

            }
            catch(Exception e0)
            {
                Console.WriteLine(e0.Message);
            }
            Console.WriteLine(img.Stream.ToString());
            
            
            await Context.Client.CurrentUser.ModifyAsync(x => x.Avatar = img);


        }
        */
    }
}
