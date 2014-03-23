using BreakingNews.API.Common;
using BreakingNews.API.Models;
using BreakingNews.API.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
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
            
            for (int i = 0; i < data.Count; i++)
            {
                data[i] = FormatPost(data[i]);
            }

            callback(data);
        }

        public async Task GetOngoingTopics(Action<List<Topic>> callback)
        {
            HttpWebRequest request = HttpWebRequest.Create("http://" + serverAddress + "/api/v1/topic/hot/?kind__in=arc&limit=10") as HttpWebRequest;
            request.Accept = "application/json";

            var response = await request.GetResponseAsync().ConfigureAwait(false);

            Stream stream = response.GetResponseStream();
            UTF8Encoding encoding = new UTF8Encoding();
            StreamReader sr = new StreamReader(stream, encoding);

            JsonTextReader tr = new JsonTextReader(sr);
            OngoingResponse topicsResponse = new JsonSerializer().Deserialize<OngoingResponse>(tr);

            List<Topic> data = topicsResponse.objects;

            tr.Close();
            sr.Dispose();

            stream.Dispose();

            for (int i = 0; i < data.Count; i++)
            {
                data[i] = FormatTopic(data[i]);
            }

            callback(data);
        }

        public async void GetTopic(Action<Topic> callback, int topicId)
        {
            HttpWebRequest request = HttpWebRequest.Create("http://" + serverAddress + "/api/v1/topic/" + topicId) as HttpWebRequest;
            request.Accept = "application/json";

            var response = await request.GetResponseAsync().ConfigureAwait(false);

            Stream stream = response.GetResponseStream();
            UTF8Encoding encoding = new UTF8Encoding();
            StreamReader sr = new StreamReader(stream, encoding);

            JsonTextReader tr = new JsonTextReader(sr);
            Topic data = new JsonSerializer().Deserialize<Topic>(tr);

            tr.Close();
            sr.Dispose();

            stream.Dispose();

            data = FormatTopic(data);

            callback(data);
        }

        public async Task GetTopicPosts(Action<List<Post>> callback, int topicId)
        {
            HttpWebRequest request = HttpWebRequest.Create("http://" + serverAddress + "/api/v1/item/?topics=" + topicId) as HttpWebRequest;
            request.Accept = "application/json";

            var response = await request.GetResponseAsync().ConfigureAwait(false);

            Stream stream = response.GetResponseStream();
            UTF8Encoding encoding = new UTF8Encoding();
            StreamReader sr = new StreamReader(stream, encoding);

            JsonTextReader tr = new JsonTextReader(sr);
            TopicResponse postsResponse = new JsonSerializer().Deserialize<TopicResponse>(tr);

            List<Post> data = postsResponse.objects;

            tr.Close();
            sr.Dispose();

            stream.Dispose();

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
            data.content = CleanText(data.content);
            data.is_read = PostHistory.Contains(data.id);

            return data;
        }

        private Topic FormatTopic(Topic data)
        {
            data.name = CleanText(data.name).ToUpper();
            data.description = CleanText(data.description);

            if (data.description.Length > 140)
            {
                data.description = data.description.Substring(0, 140) + "...";
            }

            if (data.description.Length <= 0)
            {
                data.description = "There is no other information available for this topic at the moment.";
            }

            return data;
        }

        private string CleanText(string data)
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
            data = data.Replace("<p>", "");
            data = data.Replace("</p>", "");
            data = data.Replace("<strong>", "");
            data = data.Replace("</strong>", "");
            data = Regex.Replace(data, "<\\/?a[^<>]*>", "");
            data = data.Trim();

            return data;
        }
    }
}
