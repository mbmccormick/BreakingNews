using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using BreakingNews.API.Models;
using Microsoft.Phone.Tasks;

namespace BreakingNews
{
    public partial class TopicItemControl : UserControl
    {
        public event EventHandler<EventArgs> FollowsChanged;

        public TopicItemControl()
        {
            InitializeComponent();

            this.Loaded += TopicItemControl_Loaded;
        }

        private void TopicItemControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void Topic_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            TopicItem item = ((FrameworkElement)sender).DataContext as TopicItem;

            item.is_following = true;
            App.BreakingNewsClient.FollowTopic(item);

            if (FollowsChanged != null)
                FollowsChanged(sender, e);
        }
    }
}
