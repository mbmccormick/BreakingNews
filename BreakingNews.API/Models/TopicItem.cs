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
        
        private bool _is_favorited;

        public bool is_favorited
        {
            get
            {
                return _is_favorited;
            }

            set
            {
                _is_favorited = value;

                OnPropertyChanged("is_favorited");
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
