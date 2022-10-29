using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Tweetinvi.Models;
using Tweetinvi;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Tweetinvi.Core.Extensions;

namespace TwitterFollows
{
    public partial class Form1 : Form
    {
        List<UserInfo> Following = new List<UserInfo>();
        List<UserInfo> Followers = new List<UserInfo>();
        string bearerToken;

        public Form1()
        {
            InitializeComponent();

            var consumerOnlyCredentials = new ConsumerOnlyCredentials("ydhwCu40L5Is2hV3sIfXKBZbp", "J0F1KENeeEqfxO0B2WbWdXrUi4hfjvRTJcRNTpcssD96FTY6Y5");
            var appClientWithoutBearer = new TwitterClient(consumerOnlyCredentials);

            bearerToken = appClientWithoutBearer.Auth.CreateBearerTokenAsync().Result;
            var appCredentials = new ConsumerOnlyCredentials("ydhwCu40L5Is2hV3sIfXKBZbp", "J0F1KENeeEqfxO0B2WbWdXrUi4hfjvRTJcRNTpcssD96FTY6Y5")
            {
                BearerToken = bearerToken
            };
        }

        private void buttonFetchFriends_Click(object sender, EventArgs e)
        {
            Following=new List<UserInfo>();
            
            FetchFriends();
            if (Following.Count > 0)
            {
                DumpToCsv(textBoxUserName.Text + "_Following.csv",Following);
            }
        }

        private void DumpToCsv(string strFileName, List<UserInfo> dataSet)
        {
            string strOutput="UserID, DisplayName, UserName"+Environment.NewLine;
            foreach (var user in dataSet)
            {
                strOutput+=user.id+","+user.name+","+user.username+Environment.NewLine;
            }
            string filePath=Path.Combine(textBoxOutputFolder.Text,strFileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }  
            File.WriteAllText(filePath,strOutput);

        }

        private void FetchFriends()
        {
            if (textBoxUserName.Text.IsNullOrEmpty())
            {
                MessageBox.Show("You must enter a user name to search for");
                return;
            }
            string id= GetIdFromUserName(textBoxUserName.Text);
            if (id.IsNullOrEmpty())
            {
                return;
            }

            string strNextPageToken=GetNextPageOfFriends(id,"");

            while (!strNextPageToken.IsNullOrEmpty())
            {
                strNextPageToken = GetNextPageOfFriends(id, strNextPageToken);
            }
            
        }

        string GetIdFromUserName(string userName)
        {
            var url = "https://api.twitter.com/2/users/by/username/" + userName;

            var httpRequest = (HttpWebRequest)WebRequest.Create(url);

            httpRequest.Headers["Authorization"] = "Bearer " + bearerToken;


            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            string result;
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }

            var user = JsonConvert.DeserializeObject<SingleUserFetch>(result);
            if (user == null)
            {
                MessageBox.Show("No user found by that name");
                return "";
            }

            return user.data.id;
        }

        string GetNextPageOfFriends(string userId, string nextPageId)
        {
            var url = "https://api.twitter.com/2/users/" + userId + "/following";

            if (!nextPageId.IsNullOrEmpty())
            {
                url=url+ "?pagination_token=" + nextPageId;
            }

            var httpRequest = (HttpWebRequest)WebRequest.Create(url);

            httpRequest.Headers["Authorization"] = "Bearer " + bearerToken;


            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            string result;
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }

            List<UserInfo> userInfos = new List<UserInfo>();
            var output = JsonConvert.DeserializeObject<Root>(result);

            foreach (UserInfo userInfo in output.data)
            {
                Following.Add(userInfo);
            }

            return output.meta.next_token;
        }

        string GetNextPageOfFollowers(string userId, string nextPageId)
        {
            var url = "https://api.twitter.com/2/users/" + userId + "/followers";

            if (!nextPageId.IsNullOrEmpty())
            {
                url = url + "?pagination_token=" + nextPageId;
            }

            var httpRequest = (HttpWebRequest)WebRequest.Create(url);

            httpRequest.Headers["Authorization"] = "Bearer " + bearerToken;


            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            string result;
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }

            List<UserInfo> userInfos = new List<UserInfo>();
            var output = JsonConvert.DeserializeObject<Root>(result);

            foreach (UserInfo userInfo in output.data)
            {
                Followers.Add(userInfo);
            }

            return output.meta.next_token;
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBoxOutputFolder.Text="d:\\testing";
            
        }

        private void buttonFetchFollowers_Click(object sender, EventArgs e)
        {
            if (textBoxUserName.Text.IsNullOrEmpty())
            {
                MessageBox.Show("You must enter a user name to search for");
                return;
            }
            string id = GetIdFromUserName(textBoxUserName.Text);
            if (id.IsNullOrEmpty())
            {
                return;
            }

            string strNextPageToken = GetNextPageOfFollowers(id, "");

            while (!strNextPageToken.IsNullOrEmpty())
            {
                strNextPageToken = GetNextPageOfFollowers(id, strNextPageToken);
            }

            DumpToCsv(textBoxUserName.Text + "_Followers.csv",Followers);
        }
    }
}
