using System.ComponentModel;

namespace BreakingNews.API.Models
{
    public class Topic : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string wikipedia_url { get; set; }
        public bool muted_by_default { get; set; }
        public string category { get; set; }
        public int id { get; set; }
        public string latest_item_date { get; set; }
        public string twitter_username { get; set; }
        public string links { get; set; }
        public string related_topics_uri { get; set; }
        public string livevideo { get; set; }
        public string kind { get; set; }
        public int? woeid { get; set; }
        public string image_url { get; set; }
        public int recent_count { get; set; }
        public string created_on { get; set; }
        public string slug { get; set; }
        public double? longitude { get; set; }
        public string name { get; set; }
        public string local_url { get; set; }
        public double? latitude { get; set; }
        public string resource_uri { get; set; }
        public int daily_count { get; set; }
        public string image_credit { get; set; }
        public string geohash { get; set; }
        public string placetype { get; set; }
        public string items_resource_uri { get; set; }
        public string facebookchat { get; set; }
        public bool active { get; set; }
        public string permalink { get; set; }
        public int item_count { get; set; }
        public string description { get; set; }
        public string short_permalink { get; set; }
        public bool? primary { get; set; }

        private bool _is_following;

        public bool is_following
        {
            get
            {
                return _is_following;
            }

            set
            {
                _is_following = value;

                OnPropertyChanged("is_following");
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
