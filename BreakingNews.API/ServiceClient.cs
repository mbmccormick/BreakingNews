using BreakingNews.API.Common;
using BreakingNews.API.Models;
using HackerNews.API.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace BreakingNews.API
{
    public class ServiceClient
    {
        private string serverAddress = "api.breakingnews.com";

        public List<int> PostHistory;
        public int MaxPostHistory = 250;

        public ServiceClient(bool debug)
        {
            PostHistory = IsolatedStorageHelper.GetObject<List<int>>("PostHistory");

            if (PostHistory == null)
                PostHistory = new List<int>();

            if (debug == true)
                serverAddress = "api-breakingnews-com-pqib6nr22m80.runscope.net";
        }

        public async Task GetLatestPosts(Action<List<Post>> callback)
        {
            HttpWebRequest request = HttpWebRequest.Create("http://" + serverAddress + "/api/v1/item/") as HttpWebRequest;
            request.Accept = "application/json";

            var response = await request.GetResponseAsync().ConfigureAwait(false);

            Stream stream = response.GetResponseStream();
            UTF8Encoding encoding = new UTF8Encoding();
            StreamReader sr = new StreamReader(stream, encoding);

            JsonTextReader tr = new JsonTextReader(sr);
            LatestResponse postsResponse = new JsonSerializer().Deserialize<LatestResponse>(tr);

            List<Post> data = postsResponse.objects;

            tr.Close();
            sr.Dispose();

            stream.Dispose();

            foreach (var item in data)
            {
                item.content = CleanContentText(item.content);
                item.is_read = PostHistory.Contains(item.id);
            }

            for (int i = 0; i < data.Count; i++)
            {
                data[i] = FormatPost(data[i]);
            }

            callback(data);
        }

        public async Task GetPopularPosts(Action<List<Post>> callback)
        {
            HttpWebRequest request = HttpWebRequest.Create("http://" + serverAddress + "/api/v1/popular/1/") as HttpWebRequest;
            request.Accept = "application/json";

            var response = await request.GetResponseAsync().ConfigureAwait(false);

            Stream stream = response.GetResponseStream();
            UTF8Encoding encoding = new UTF8Encoding();
            StreamReader sr = new StreamReader(stream, encoding);

            JsonTextReader tr = new JsonTextReader(sr);
            PopularResponse postsResponse = new JsonSerializer().Deserialize<PopularResponse>(tr);

            List<Post> data = postsResponse.items;

            tr.Close();
            sr.Dispose();

            stream.Dispose();

            foreach (var item in data)
            {
                item.content = CleanContentText(item.content);
                item.is_read = PostHistory.Contains(item.id);
            }

            for (int i = 0; i < data.Count; i++)
            {
                data[i] = FormatPost(data[i]);
            }

            callback(data);
        }

        public void MarkPostAsRead(int postId)
        {
            while (PostHistory.Count >= MaxPostHistory)
            {
                PostHistory.RemoveAt(MaxPostHistory - 1);
            }

            PostHistory.Insert(0, postId);
        }

        public void SaveData()
        {
            IsolatedStorageHelper.SaveObject<List<int>>("PostHistory", PostHistory);
        }

        private Post FormatPost(Post data)
        {
            return data;
        }

        private string CleanContentText(string data)
        {
            data = data.Replace("�", "");
            data = data.Replace("&amp;", "");
            data = data.Replace("&euro;&trade;", "'");
            data = data.Replace("&euro;&oelig;", "\"");
            data = data.Replace("&euro;?", "\"");
            data = data.Replace("&euro;&ldquo;", "-");
            data = data.Replace("&euro;&tilde;", "'");
            data = data.Replace("&euro;", "...");
            data = data.Replace("__BR__", "\n\n");
            data = data.Replace("\\", "");
            data = data.Trim();

            return data;
        }
    }
}
