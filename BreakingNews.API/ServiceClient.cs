using BreakingNews.API.Common;
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

        public List<TopicItem> FavoriteTopics;
        public int MaxFavoriteTopics = 250;

        public ServiceClient(bool debug)
        {
            PostHistory = IsolatedStorageHelper.GetObject<List<int>>("PostHistory");
            FavoriteTopics = IsolatedStorageHelper.GetObject<List<TopicItem>>("FavoriteTopics");

            if (PostHistory == null)
                PostHistory = new List<int>();

            if (FavoriteTopics == null)
                FavoriteTopics = new List<TopicItem>();

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

            try
            {
                JsonTextReader tr = new JsonTextReader(sr);
                LatestPostsResponse postsResponse = new JsonSerializer().Deserialize<LatestPostsResponse>(tr);

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
            catch (JsonSerializationException ex)
            {
                throw ex;
            }
        }

        public async Task GetPopularPosts(Action<List<Post>> callback)
        {
            HttpWebRequest request = HttpWebRequest.Create("http://" + serverAddress + "/api/v1/popular/1/") as HttpWebRequest;
            request.Accept = "application/json";

            var response = await request.GetResponseAsync().ConfigureAwait(false);

            Stream stream = response.GetResponseStream();
            UTF8Encoding encoding = new UTF8Encoding();
            StreamReader sr = new StreamReader(stream, encoding);

            try
            {
                JsonTextReader tr = new JsonTextReader(sr);
                PopularPostsResponse postsResponse = new JsonSerializer().Deserialize<PopularPostsResponse>(tr);

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
            catch (JsonSerializationException ex)
            {
                throw ex;
            }
        }

        public async Task GetOngoingTopics(Action<List<TopicItem>> callback)
        {
            HttpWebRequest request = HttpWebRequest.Create("http://" + serverAddress + "/api/v1/topic/hot/?kind__in=arc&limit=10") as HttpWebRequest;
            request.Accept = "application/json";

            var response = await request.GetResponseAsync().ConfigureAwait(false);

            Stream stream = response.GetResponseStream();
            UTF8Encoding encoding = new UTF8Encoding();
            StreamReader sr = new StreamReader(stream, encoding);

            try
            {
                JsonTextReader tr = new JsonTextReader(sr);
                OngoingTopicsResponse topicsResponse = new JsonSerializer().Deserialize<OngoingTopicsResponse>(tr);

                List<TopicItem> data = topicsResponse.objects;

                tr.Close();
                sr.Dispose();

                stream.Dispose();

                for (int i = 0; i < data.Count; i++)
                {
                    data[i] = FormatTopic(data[i]);
                }

                callback(data);
            }
            catch (JsonSerializationException ex)
            {
                throw ex;
            }
        }

        public async Task GetFavoriteTopics(Action<List<TopicItem>> callback)
        {
            callback(FavoriteTopics);
        }

        public async void GetTopic(Action<Topic> callback, int topicId)
        {
            HttpWebRequest request = HttpWebRequest.Create("http://" + serverAddress + "/api/v1/topic/" + topicId) as HttpWebRequest;
            request.Accept = "application/json";

            var response = await request.GetResponseAsync().ConfigureAwait(false);

            Stream stream = response.GetResponseStream();
            UTF8Encoding encoding = new UTF8Encoding();
            StreamReader sr = new StreamReader(stream, encoding);

            try
            {
                JsonTextReader tr = new JsonTextReader(sr);
                Topic data = new JsonSerializer().Deserialize<Topic>(tr);

                tr.Close();
                sr.Dispose();

                stream.Dispose();

                data = FormatTopic(data);

                callback(data);
            }
            catch (JsonSerializationException ex)
            {
                throw ex;
            }
        }

        public async Task GetTopicPosts(Action<List<Post>> callback, int topicId)
        {
            HttpWebRequest request = HttpWebRequest.Create("http://" + serverAddress + "/api/v1/item/?topics=" + topicId) as HttpWebRequest;
            request.Accept = "application/json";

            var response = await request.GetResponseAsync().ConfigureAwait(false);

            Stream stream = response.GetResponseStream();
            UTF8Encoding encoding = new UTF8Encoding();
            StreamReader sr = new StreamReader(stream, encoding);

            try
            {
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
            catch (JsonSerializationException ex)
            {
                throw ex;
            }
        }

        public async Task GetTopics(Action<List<TopicItem>> callback, string query)
        {
            HttpWebRequest request = HttpWebRequest.Create("http://" + serverAddress + "/api/v1/topic/search/?q=" + query + "&order_by=-item_count&limit=20") as HttpWebRequest;
            request.Accept = "application/json";

            var response = await request.GetResponseAsync().ConfigureAwait(false);

            Stream stream = response.GetResponseStream();
            UTF8Encoding encoding = new UTF8Encoding();
            StreamReader sr = new StreamReader(stream, encoding);

            try
            {
                JsonTextReader tr = new JsonTextReader(sr);
                TopicSearchResponse topicsResponse = new JsonSerializer().Deserialize<TopicSearchResponse>(tr);

                List<TopicItem> data = topicsResponse.objects;

                tr.Close();
                sr.Dispose();

                stream.Dispose();

                for (int i = 0; i < data.Count; i++)
                {
                    data[i] = FormatTopic(data[i]);
                }

                callback(data);
            }
            catch (JsonSerializationException ex)
            {
                throw ex;
            }
        }

        public void MarkPostAsRead(int postId)
        {
            while (PostHistory.Count >= MaxPostHistory)
            {
                PostHistory.RemoveAt(MaxPostHistory - 1);
            }

            PostHistory.Insert(0, postId);
        }

        public void FavoriteTopic(Topic data)
        {
            while (FavoriteTopics.Count >= MaxFavoriteTopics)
            {
                FavoriteTopics.RemoveAt(MaxFavoriteTopics - 1);
            }

            TopicItem item = new TopicItem()
            {
                id = data.id,
                name = data.name,
                is_favorited = data.is_favorited
            };

            FavoriteTopics.Insert(0, item);
        }

        public void FavoriteTopic(TopicItem item)
        {
            while (FavoriteTopics.Count >= MaxFavoriteTopics)
            {
                FavoriteTopics.RemoveAt(MaxFavoriteTopics - 1);
            }

            FavoriteTopics.Insert(0, item);
        }

        public void UnfavoriteTopic(int topicId)
        {
            int i = 0;
            for (i = 0; i < FavoriteTopics.Count; i++)
            {
                if (FavoriteTopics[i].id == topicId) break;
            }

            FavoriteTopics.RemoveAt(i);
        }

        public void SaveData()
        {
            IsolatedStorageHelper.SaveObject<List<int>>("PostHistory", PostHistory);
            IsolatedStorageHelper.SaveObject<List<TopicItem>>("FavoriteTopics", FavoriteTopics);
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

            bool found = false;
            for (int i = 0; i < FavoriteTopics.Count; i++)
            {
                if (FavoriteTopics[i].id == data.id)
                {
                    found = true;
                    break;
                }
            }

            data.is_favorited = found;

            return data;
        }

        private TopicItem FormatTopic(TopicItem data)
        {
            data.name = CleanText(data.name).ToUpper();

            bool found = false;
            for (int i = 0; i < FavoriteTopics.Count; i++)
            {
                if (FavoriteTopics[i].id == data.id)
                {
                    found = true;
                    break;
                }
            }

            data.is_favorited = found;

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
