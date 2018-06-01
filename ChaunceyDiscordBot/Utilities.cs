using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
namespace ChaunceyDiscordBot
{
    class Utilities
    {
        private static Dictionary<string, string> alerts;

        static Utilities()
        {
            string json = File.ReadAllText("SystemLang/alerts.json"); //stores json data in a string
            var data = JsonConvert.DeserializeObject<dynamic>(json); //Converts from json to data structure
            alerts = data.ToObject<Dictionary<string,string>>();
        }

        public static string GetAlert(string key)
        {
            if (alerts.ContainsKey(key))
            {
                return alerts[key];
            }
            else
            {
                return "";
            }
        }

        public static string getFormattedAlert(string key, params object[] param)
        {
            if (alerts.ContainsKey(key))
            {
                return String.Format(alerts[key], param);
            }
            else
            {
                return "";
            }
        }

        public static string getFormattedAlert(string key, object param)
        {
            return getFormattedAlert(key, new object[] { param });
        }
    }
}
