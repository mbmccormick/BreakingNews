using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace BreakingNews.API.Models
{
    public class Post : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public int importance { get; set; }
        public string date_ap { get; set; }
        public object source { get; set; }
        public string resource_uri { get; set; }
        public int id { get; set; }
        public string content { get; set; }
        public Media media { get; set; }
        public string note { get; set; }
        public object data_type { get; set; }
        public string push_content { get; set; }
        public Tweet tweet { get; set; }
        public string updated_date { get; set; }
        public List<Topic> topics { get; set; }
        public string local_url { get; set; }
        public string shortened_url { get; set; }
        public int whoas { get; set; }
        public string slug { get; set; }
        public string url { get; set; }
        public bool pushed { get; set; }
        public string type { get; set; }
        public string status { get; set; }
        public string url_domain { get; set; }
        public string permalink { get; set; }
        public Author author { get; set; }
        public object data { get; set; }
        public string submitter { get; set; }
        public string date { get; set; }
        public string short_permalink { get; set; }

        public string PrimaryTopic
        {
            get
            {
                return topics[0].name.ToUpper();
            }
        }

        public string FriendlyDate
        {
            get
            {
                DateTime postDate = DateTime.Now;
                DateTime.TryParse(date_ap, out postDate);

                double minutes = (DateTime.Now - postDate).TotalMinutes;
                double hours = minutes / 60;

                if (minutes < 2)
                    return "Posted just now";
                else if (minutes < 60)
                    return "Posted " + Convert.ToInt32(minutes) + " minutes ago";
                else if (minutes < 120)
                    return "Posted 1 hour ago";
                else
                    return "Posted " + Convert.ToInt32(hours) + " hours ago";
            }
        }

        private bool _is_read;
        public bool is_read
        {
            get
            {
                return _is_read;
            }

            set
            {
                _is_read = value;

                OnPropertyChanged("is_read");

                OnPropertyChanged("topic_foreground");
                OnPropertyChanged("content_foreground");
                OnPropertyChanged("description_foreground");
            }
        }

        public SolidColorBrush topic_foreground
        {
            get
            {
                if (this.is_read == true)
                    return new SolidColorBrush(Color.FromArgb(255, 195, 195, 195));
                else
                    return new SolidColorBrush(Color.FromArgb(255, 152, 170, 185));
            }
        }

        public SolidColorBrush content_foreground
        {
            get
            {
                if (this.is_read == true)
                    return new SolidColorBrush(Color.FromArgb(255, 195, 195, 195));
                else
                    return new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            }
        }

        public SolidColorBrush description_foreground
        {
            get
            {
                if (this.is_read == true)
                    return new SolidColorBrush(Color.FromArgb(255, 213, 145, 115));
                else
                    return new SolidColorBrush(Color.FromArgb(255, 248, 36, 29));
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

    public class Media
    {
        public string html { get; set; }
        public int thumbnail_width { get; set; }
        public string author_url { get; set; }
        public string original_url { get; set; }
        public string title { get; set; }
        public string resource_uri { get; set; }
        public int width { get; set; }
        public string author_name { get; set; }
        public string provider_url { get; set; }
        public int thumbnail_height { get; set; }
        public int height { get; set; }
        public string provider_name { get; set; }
        public int id { get; set; }
        public string url { get; set; }
        public string type { get; set; }
        public string thumbnail_url { get; set; }
    }

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

    public class Topic
    {
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
    }

    public class Author
    {
        public string first_name { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string last_name { get; set; }
        public string resource_uri { get; set; }
        public bool is_staff { get; set; }
        public int id { get; set; }
    }
}
