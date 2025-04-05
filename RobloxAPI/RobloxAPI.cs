using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using Newtonsoft.Json.Linq;

namespace RobloxAPI
{
    public class RobloxApi
    {
        public static string GetUsername()
        {
            string userName = Environment.UserName;
            string path = "C:\\\\Users\\\\" + userName + "\\\\AppData\\\\Local\\\\Roblox\\\\LocalStorage\\\\appStorage.json";
            bool flag = !File.Exists(path);
            string result;
            if (flag)
            {
                result = null;
            }
            else
            {
                try
                {
                    string text = File.ReadAllText(path);
                    JObject jobject = JObject.Parse(text);
                    bool flag2 = jobject.ContainsKey("Username");
                    if (flag2)
                    {
                        JToken jtoken = jobject["Username"];
                        return (jtoken != null) ? jtoken.ToString() : null;
                    }
                }
                catch (Exception ex)
                {
                    
                }
                result = null;
            }
            CatchError();
            return result;
        }

        public static string CheckStatus(string link)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    string key = client.DownloadString(link)?.Trim();

                    if (string.IsNullOrEmpty(key))
                    {
                        return "Unknown";
                    }
                    if (key.Equals("Working"))
                    {
                        return "Working";
                    }
                    if (key.Equals("Not Working"))
                    {
                        return "Not Working";
                    }
                    if (key.Equals("Update"))
                    {
                        return "Outdated Update To The Lastet Version";
                    }
                    return "Unknown";
                }
            }
            catch (Exception ex)
            {
                CatchError();
                return $"Could Not Check Status";
            }
        }

        public static void CatchError(string error = null, string customMessage = null, string app = null)
        {
            if (!string.IsNullOrEmpty(customMessage))
            {
                MessageBox.Show(customMessage, app ?? "Error");
            }
            else if (!string.IsNullOrEmpty(error))
            {
                if (error == "user")
                {
                    MessageBox.Show("There's An Error Getting Your Roblox Username! Please Login Before Opening The App!", "RobloxApi");
                }
                else if (error == "status")
                {
                    MessageBox.Show("There's An Error Getting The App's Status. Please Connect To The Internet Before Opening The App!", "RobloxApi");
                }
            }
        }
    }
}
