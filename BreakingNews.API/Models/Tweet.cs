
namespace BreakingNews.API.Models
{
    public class Tweet
    {
        public object location { get; set; }
        public string avatar { get; set; }
        public string resource_uri { get; set; }
        public string screen_name { get; set; }
        public string user_id { get; set; }
        public string name { get; set; }
        public int id { get; set; }
        public string url { get; set; }
        public string tweet_id { get; set; }
        public string content { get; set; }
        public string date { get; set; }
    }
}
