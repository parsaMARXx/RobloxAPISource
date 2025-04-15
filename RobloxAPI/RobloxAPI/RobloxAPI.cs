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
                    result = "Unknown User";
                }
                result = null;
            }
            CatchError();
            return result;
        }

        private static string GetUserIdFromUsername(string username)
        {
            string address = "https://users.roblox.com/v1/usernames/users";
            string data = "{\"usernames\": [\"" + username + "\"]}";
            try
            {
                WebClient webClient = new WebClient();
                webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
                string json = webClient.UploadString(address, "POST", data);
                JObject jObject = JObject.Parse(json);
                if (jObject["data"] != null && jObject["data"].HasValues)
                {
                    return jObject["data"][0]["id"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting user ID for " + username + ": " + ex.Message, "CloudyApi");
            }

            return null;
        }

        public static BitmapImage GetAvatar(string username)
        {
            string userIdFromUsername = GetUserIdFromUsername(username);
            if (string.IsNullOrEmpty(userIdFromUsername))
            {
                return null;
            }

            string address = "https://thumbnails.roblox.com/v1/users/avatar-headshot?userIds=" + userIdFromUsername + "&size=420x420&format=png";
            try
            {
                WebClient webClient = new WebClient();
                string json = webClient.DownloadString(address);
                JObject jObject = JObject.Parse(json);
                if (jObject["data"] != null && jObject["data"].HasValues)
                {
                    string uriString = jObject["data"][0]["imageUrl"].ToString();
                    return new BitmapImage(new Uri(uriString));
                }
            }
            catch (Exception ex)
            {
                CatchError();
            }

            return null;
        }

        public async void OpenRoblox()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string url = "https://whatexecutorsare.online/roblox-version";
                    string json = await client.GetStringAsync(url);
                    string version = JObject.Parse(json)["version"].ToString();
                    string robloxPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Roblox", "Versions", version, "RobloxPlayerBeta.exe");
                    if (File.Exists(robloxPath))
                    {
                        System.Diagnostics.Process.Start(robloxPath);
                        Thread.Sleep(5000);
                    }
                    else
                    {
                        MessageBox.Show("RobloxPlayerBeta.exe Was Not Found", "RobloxApi");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could Not Open RobloxPlayerBeta.exe", "RobloxApi");
            }
        }

        public async void OpenBloxstrap()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string bloxFolder = "Bloxstrap";
                    string bloxpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), bloxFolder, "Bloxstrap.exe");
                    if (File.Exists(bloxpath))
                    {
                        System.Diagnostics.Process.Start(bloxpath);
                        Thread.Sleep(5000);
                    }
                    else
                    {
                        MessageBox.Show("Bloxstrap.exe Not Found", "RobloxApi");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could Not Open Bloxstrap.exe", "RobloxApi");
            }
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
                        client.DownloadString(link)?.Trim();
                        return "Unknown";
                    }
                    if (key.Equals("Working"))
                    {
                        client.DownloadString(link)?.Trim();
                        return "Working";
                    }
                    if (key.Equals("Not Working"))
                    {
                        client.DownloadString(link)?.Trim();
                        return "Not Working";
                    }
                    if (key.Equals("Update"))
                    {
                        client.DownloadString(link)?.Trim();
                        return "Outdated Update To The Lastet Version";
                    }
                    client.DownloadString(link)?.Trim();
                    return "Unknown";
                }
            }
            catch (Exception ex)
            {
                CatchError();
                return $"Could Not Check Status";
            }
        }

        public static void CatchError(string customMessage = null, string app = null, string error = null)
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
                else if (error == "profile")
                {
                    MessageBox.Show("Theres An Error Getting The Profile Picture Please Connect To The Internet", "RobloxApi");
                }
            }
        }
    }
}
