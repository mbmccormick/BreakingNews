﻿using BreakingNews.API.Common;
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

        public List<TopicItem> FollowedTopics;
        public int MaxFollowedTopics = 250;

        public ServiceClient(bool debug)
        {
            FollowedTopics = IsolatedStorageHelper.GetObject<List<TopicItem>>("FollowedTopics");

            if (FollowedTopics == null)
                FollowedTopics = new List<TopicItem>();

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

        public async Task GetFollowedTopics(Action<List<TopicItem>> callback)
        {
            callback(FollowedTopics);
        }

        public async void GetTopic(Action<Topic> callback, int topicId)
        {
            HttpWebRequest request = HttpWebRequest.Create("http://" + serverAddress + "/api/v1/topic/" + topicId + "/") as HttpWebRequest;
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

        public void FollowTopic(Topic data)
        {
            bool found = false;
            foreach (TopicItem ti in FollowedTopics)
            {
                if (ti.id == data.id) found = true;
            }

            if (found == true) return;

            while (FollowedTopics.Count >= MaxFollowedTopics)
            {
                FollowedTopics.RemoveAt(MaxFollowedTopics - 1);
            }

            TopicItem item = new TopicItem()
            {
                id = data.id,
                name = data.name,
                is_following = data.is_following
            };

            FollowedTopics.Insert(0, item);
        }

        public void FollowTopic(TopicItem item)
        {
            bool found = false;
            foreach (TopicItem ti in FollowedTopics)
            {
                if (ti.id == item.id) found = true;
            }

            if (found == true) return;

            while (FollowedTopics.Count >= MaxFollowedTopics)
            {
                FollowedTopics.RemoveAt(MaxFollowedTopics - 1);
            }

            FollowedTopics.Insert(0, item);
        }

        public void UnfollowTopic(int topicId)
        {
            int i = 0;
            for (i = 0; i < FollowedTopics.Count; i++)
            {
                if (FollowedTopics[i].id == topicId) break;
            }

            FollowedTopics.RemoveAt(i);
        }

        public void SaveData()
        {
            IsolatedStorageHelper.SaveObject<List<TopicItem>>("FollowedTopics", FollowedTopics);
        }

        private Post FormatPost(Post data)
        {
            data.content = CleanText(data.content);
            
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
            for (int i = 0; i < FollowedTopics.Count; i++)
            {
                if (FollowedTopics[i].id == data.id)
                {
                    found = true;
                    break;
                }
            }

            data.is_following = found;

            return data;
        }

        private TopicItem FormatTopic(TopicItem data)
        {
            data.name = CleanText(data.name).ToUpper();

            bool found = false;
            for (int i = 0; i < FollowedTopics.Count; i++)
            {
                if (FollowedTopics[i].id == data.id)
                {
                    found = true;
                    break;
                }
            }

            data.is_following = found;

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
