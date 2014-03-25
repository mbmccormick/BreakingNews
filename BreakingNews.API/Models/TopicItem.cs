using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace BreakingNews.API.Models
{
    public class TopicItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public int id { get; set; }
        public string name { get; set; }
        
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
