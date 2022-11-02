using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Net;
using Newtonsoft.Json;


namespace TwitterFollows
{
    public partial class Form1 : Form
    {
        #region Fields and Properties 

        List<UserInfo> Following = new List<UserInfo>();
        List<UserInfo> Followers = new List<UserInfo>();
        string bearerToken= "AAAAAAAAAAAAAAAAAAAAACbdigEAAAAAYMD0WniWyL%2Fv2y5b2dcMsCF%2F%2FH0%3DG11kQLVQxt3u1E1wSeZtlMacDw5c2EQOL56SkqqcsD4KXG1vOO";

        #endregion

        #region Form Events
        public Form1()
        {
            InitializeComponent();
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            var result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBoxOutputFolder.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBoxOutputFolder.Text = AppContext.BaseDirectory;
        }

        private void buttonFetchFriends_Click(object sender, EventArgs e)
        {
            Following=new List<UserInfo>();

            if (string.IsNullOrEmpty(textBoxUserName.Text))
            {
                MessageBox.Show("You must enter a user name to search for");
                return;
            }
            string id = GetIdFromUserName(textBoxUserName.Text);
            if (string.IsNullOrEmpty(id))
            {
                MessageBox.Show("No user with that Id found");
                return;
            }

            string strNextPageToken = GetNextPageOfFriends(id, "");

            while (!string.IsNullOrEmpty(strNextPageToken))
            {
                strNextPageToken = GetNextPageOfFriends(id, strNextPageToken);
            }

            if (Following.Count > 0)
            {
                DumpToCsv(textBoxUserName.Text + "_Following.csv",Following);
            }
            MessageBox.Show(Following.Count.ToString() + " users are being followed by this account");
            System.Diagnostics.Process.Start("explorer.exe", textBoxOutputFolder.Text);
        }

        private void buttonFetchFollowers_Click(object sender, EventArgs e)
        {
            Followers = new List<UserInfo>();
            if (string.IsNullOrEmpty(textBoxUserName.Text))
            {
                MessageBox.Show("You must enter a user name to search for");
                return;
            }
            string id = GetIdFromUserName(textBoxUserName.Text);
            if (string.IsNullOrEmpty(id))
            {
                MessageBox.Show("No user with that Id found");
                return;
            }

            string strNextPageToken = GetNextPageOfFollowers(id, "");

            while (!string.IsNullOrEmpty(strNextPageToken))
            {
                strNextPageToken = GetNextPageOfFollowers(id, strNextPageToken);
            }
            if (Followers.Count > 0)
            {
                DumpToCsv(textBoxUserName.Text + "_Followers.csv", Followers);
            }
            MessageBox.Show(Followers.Count.ToString() + " followers of this account found");
            System.Diagnostics.Process.Start("explorer.exe", textBoxOutputFolder.Text);
        }

        #endregion

        #region Helper Functions

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

            if (!string.IsNullOrEmpty(nextPageId))
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
            if (output==null || output.data==null) return "";
            foreach (UserInfo userInfo in output.data)
            {
                Following.Add(userInfo);
            }

            return output.meta.next_token;
        }

        string GetNextPageOfFollowers(string userId, string nextPageId)
        {
            var url = "https://api.twitter.com/2/users/" + userId + "/followers";

            if (!string.IsNullOrEmpty(nextPageId))
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
            if (output==null || output.data==null) return "";
            foreach (UserInfo userInfo in output.data)
            {
                Followers.Add(userInfo);
            }

            return output.meta.next_token;
        }

       #endregion

    }
}
