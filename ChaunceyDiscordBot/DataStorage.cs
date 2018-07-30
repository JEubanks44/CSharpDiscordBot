using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using System.Configuration;
using System.Data.SqlClient;

using Discord.Commands;
using Discord;
using Discord.WebSocket;
namespace ChaunceyDiscordBot
{
    //Not Yet Implemented or Completed
    public class  DataStorage : ModuleBase<SocketCommandContext>
    {
        static SqlConnection conn = new SqlConnection("Data Source=JOSEPHSPC\\JESERVER;Initial Catalog=DiscordBot;Integrated Security=True");
        public DataStorage()
        {
            
            

            if(conn == null)
            {
                Console.WriteLine("Connection to Database Failed");
            }
            else
            {
                Console.WriteLine("Connection to Database Successful");
                Console.WriteLine("Connection String: " + conn.ConnectionString);
                Console.WriteLine("Database: " + conn.Database);
                Console.WriteLine("Data Source: " + conn.DataSource);
            }
            
            /*
            string provider = ConfigurationManager.AppSettings["provider"];
            string connectionString = ConfigurationManager.AppSettings["connectionString"];
            DbProviderFactory factory = DbProviderFactories.GetFactory(provider);

            using (DbConnection connection = factory.CreateConnection())
            {
                if (connection == null)
                {
                    Console.WriteLine("ConnectionError");
                    Console.ReadLine();
                }

                connection.ConnectionString = connectionString;

                connection.Open();

                DbCommand command = factory.CreateCommand();

                if (command == null)
                {
                    Console.WriteLine("Command Error");
                    Console.ReadLine();
                }

                command.Connection = connection;
                command.CommandText = "Select * From UserInfo";

                using (DbDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        Console.WriteLine($"{dataReader["realName"]}");
                    }
                }
                */
                conn.Close();
             
        }

        public bool checkIDExists(string ID)
        {

            conn.Close();
            conn.Open();
            SqlCommand checkID = new SqlCommand("SELECT COUNT(*) FROM UserInfo WHERE ([userID] = @userID)");
            
            checkID.Connection = conn;
            checkID.Parameters.AddWithValue("@userID", ID);
            int check = (int)checkID.ExecuteScalar();
            checkID.Connection.Close();
            if(check > 0)
            {

                Console.WriteLine("User Already Exists");
                return true;

            }
            else
            {
                
                return false;
                
            }
            

            
        }

        public void setID(string ID)
        {
            if (checkIDExists(ID))
            {

            }
            else
            { 
                conn.Open();
                SqlCommand insertNewUserID = new SqlCommand("INSERT INTO UserInfo(userID,points,realName,nickName) VALUES(@ID, @POINTS,@REAL,@NICK)");
                insertNewUserID.Connection = conn;

                insertNewUserID.Parameters.AddWithValue("@ID", ID);
                insertNewUserID.Parameters.AddWithValue("@POINTS", 0);
                insertNewUserID.Parameters.AddWithValue("@REAL", "");
                insertNewUserID.Parameters.AddWithValue("@NICK", "");

                Console.WriteLine("Adding New User ID: User ID added to database");
                insertNewUserID.ExecuteNonQuery();
                conn.Close();
                
            }

            

        }

        public void setRealName(string ID, string realName)
        {
            conn.Open();
            SqlCommand updateRealName = new SqlCommand("UPDATE UserInfo SET realName = @REAL WHERE userID = @ID");
            updateRealName.Connection = conn;
            SqlCommand insertNewUserID = new SqlCommand("INSERT INTO UserInfo(userID,points,realName,nickName) VALUES(@ID, @POINTS,@REAL,@NICK)");
            insertNewUserID.Connection = conn;
            if (checkIDExists(ID))
            {
                updateRealName.Parameters.AddWithValue("@ID", ID);
                updateRealName.Parameters.AddWithValue("@REAL", realName);
                Console.WriteLine("Updated UserID: " + ID + "'s realName to: " + realName);
                updateRealName.ExecuteNonQuery();
            }
            else
            {
                insertNewUserID.Parameters.AddWithValue("@ID", ID);
                insertNewUserID.Parameters.AddWithValue("@POINTS", 0);
                insertNewUserID.Parameters.AddWithValue("@REAL", realName);
                insertNewUserID.Parameters.AddWithValue("@NICK", "");

                Console.WriteLine("Adding New User ID: User ID and Real Name added to database");
                insertNewUserID.ExecuteNonQuery();
            }
            conn.Close();
        }

        public void setNickName(string ID, string nickName)
        {
            conn.Open();
            SqlCommand updateNickName = new SqlCommand("UPDATE UserInfo SET nickName = @NICK WHERE userID = @ID");
            updateNickName.Connection = conn;
            SqlCommand insertNewUserID = new SqlCommand("INSERT INTO UserInfo(userID,points,realName,nickName) VALUES(@ID, @POINTS,@REAL,@NICK)");
            insertNewUserID.Connection = conn;

            if (checkIDExists(ID))
            {
                updateNickName.Parameters.AddWithValue("@ID", ID);
                updateNickName.Parameters.AddWithValue("@NICK", nickName);
                Console.WriteLine("Updates UserID: " + ID + "'s nickName to: " + nickName);
                updateNickName.ExecuteNonQuery();
            }
            else
            {
                insertNewUserID.Parameters.AddWithValue("@ID", ID);
                insertNewUserID.Parameters.AddWithValue("@POINTS", 0);
                insertNewUserID.Parameters.AddWithValue("@REAL", "");
                insertNewUserID.Parameters.AddWithValue("@NICK", nickName);

                Console.WriteLine("Adding New User ID: User ID and nickName added to database");
                insertNewUserID.ExecuteNonQuery();
            }
            conn.Close();
        }

        public string getNickName(string ID)
        {
            conn.Open();
            SqlCommand getNick = new SqlCommand("SELECT nickName FROM UserInfo WHERE userID = @ID");
            getNick.Connection = conn;
            getNick.Parameters.AddWithValue("@ID", ID);
            string nickName = (string)getNick.ExecuteScalar();
            conn.Close();
            return nickName;
        }

        public void addPoints(string ID, int points)
        {
            int userPoints = getPoints(ID);
            conn.Open();
            SqlCommand addpoint = new SqlCommand("UPDATE UserInfo SET points = @POINTS WHERE userID = @ID");
            addpoint.Connection = conn;
            
            int updatedPoints = userPoints + points;
            addpoint.Parameters.AddWithValue("@POINTS", updatedPoints);
            addpoint.Parameters.AddWithValue("@ID", ID);
            addpoint.ExecuteNonQuery();
            conn.Close();
        }

        public int getPoints(string ID)
        {
            if (!checkIDExists(ID))
            {
                setID(ID);
            }
            conn.Open();
            SqlCommand points = new SqlCommand("SELECT points FROM UserInfo WHERE userID = @ID");
            points.Connection = conn;
            points.Parameters.AddWithValue("@ID", ID);
            int XP = (int)points.ExecuteScalar();
            conn.Close();
            return XP;  
        }

        public int getLevel(string ID)
        {
            if (!checkIDExists(ID))
            {
                setID(ID);
            }
            conn.Open();
            SqlCommand levels = new SqlCommand("SELECT level FROM UserInfo WHERE userID = @ID");
            levels.Connection = conn;
            levels.Parameters.AddWithValue("@ID", ID);
            SqlDataReader dr = levels.ExecuteReader();
            dr.Read();
            int level = dr.GetInt32(0);
            conn.Close();
            return level;
        }

        public bool checkLevelUp(string ID)
        {
            if(!checkIDExists(ID))
            {
                setID(ID);
            }
            int userPoints = getPoints(ID);
            int userLevel = getLevel(ID);
            double pointRequirement = (((userLevel - 1) * 20) + ((userLevel - 1) * 20 * 1.10 ));
            
            if(userPoints >= pointRequirement)
            {
                levelUp(ID);
                return true;
            }
            else
            {
                return false;
            }
            
        }

        public void levelUp(string ID)
        {
            int levelNew = getLevel(ID) + 1;
            conn.Open();
            SqlCommand updateLevel = new SqlCommand("UPDATE UserInfo SET level = @LEVEL WHERE userID = @ID");
            SqlCommand updatePoints = new SqlCommand("UPDATE UserInfo SET points = @POINTS WHERE userID = @ID");
            updateLevel.Connection = conn;
            updatePoints.Connection = conn;

            updateLevel.Parameters.AddWithValue("@LEVEL", levelNew);
            updateLevel.Parameters.AddWithValue("@ID", ID);

            updatePoints.Parameters.AddWithValue("@POINTS", 0);
            updatePoints.Parameters.AddWithValue("@ID", ID);

            updateLevel.ExecuteNonQuery();
            updatePoints.ExecuteNonQuery();
            conn.Close();
        }

        public void setSteamID(string ID, string steamID)
        {
            if (conn.State == System.Data.ConnectionState.Open)
            {
                conn.Close();
            }
            conn.Open();
            SqlCommand updateSteamID = new SqlCommand("UPDATE UserInfo SET steamID = @STEAMID WHERE userID = @ID");
            updateSteamID.Connection = conn;
            updateSteamID.Parameters.AddWithValue("@STEAMID", steamID);
            updateSteamID.Parameters.AddWithValue("@ID", ID);
            updateSteamID.ExecuteNonQuery();
            conn.Close();
        }

        public static async Task<string> getSteamID(string ID)
        {
            if (conn.State == System.Data.ConnectionState.Open)
            {
                conn.Close();
            }
            else { await conn.OpenAsync(); };
            SqlCommand getSteamID = new SqlCommand("SELECT steamID FROM UserInfo WHERE userID = @ID");
            getSteamID.Connection = conn;
            getSteamID.Parameters.AddWithValue("@ID", ID);
            var id = await getSteamID.ExecuteScalarAsync();
            conn.Close();
            return id.ToString();
            
        }

        public static void setNowPlaying(string ID, string game)
        {
            if (conn.State == System.Data.ConnectionState.Open)
            {
                conn.Close();
            }
            conn.Open();
            SqlCommand setNowPlaying = new SqlCommand("UPDATE UserInfo SET nowPlaying = @GAME WHERE userID = @ID");
            setNowPlaying.Connection = conn;
            setNowPlaying.Parameters.AddWithValue("@GAME", game);
            setNowPlaying.Parameters.AddWithValue("@ID", ID);
            setNowPlaying.ExecuteNonQuery();
            conn.Close();
        }

        public static string getNowPlaying(string ID)
        {
            if (conn.State == System.Data.ConnectionState.Open)
            {
                conn.Close();
            }
            conn.Open();
            SqlCommand getNowPlaying = new SqlCommand("SELECT nowPlaying FROM UserInfo WHERE userID = @ID");
            getNowPlaying.Connection = conn;
            getNowPlaying.Parameters.AddWithValue("@ID", ID);
            string game = (string)getNowPlaying.ExecuteScalar();
            conn.Close();
            return game;
        }

        
    }
}
