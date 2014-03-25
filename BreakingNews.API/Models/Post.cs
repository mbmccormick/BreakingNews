﻿using BreakingNews.API.Models;
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

        public string FriendlyTopic
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
                DateTime postDate = new DateTime();
                DateTime.TryParse(date, out postDate);

                double minutes = (DateTime.UtcNow - postDate).TotalMinutes;
                double hours = minutes / 60;
                double days = hours / 24;

                if (days >= 2.0)
                    return "Posted " + Convert.ToInt32(days) + " days ago";
                else if (days >= 1.0)
                    return "Posted 1 day ago";
                else if (hours >= 2.0)
                    return "Posted " + Convert.ToInt32(hours) + " hours ago";
                else if (hours >= 1.0)
                    return "Posted 1 hour ago";
                else if (minutes >= 2.0)
                    return "Posted " + Convert.ToInt32(minutes) + " minutes ago";
                else
                    return "Posted just now";
            }
        }

        public SolidColorBrush content_foreground
        {
            get
            {
                if (this.importance > 4)
                    return new SolidColorBrush(Color.FromArgb(255, 194, 67, 10));
                else if (this.importance > 2)
                    return new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
                else
                    return new SolidColorBrush(Color.FromArgb(255, 195, 195, 195));
            }
        }

        public double media_opacity
        {
            get
            {
                if (this.importance > 2)
                    return 1.0;
                else
                    return 0.5;
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
