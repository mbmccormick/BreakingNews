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
    public partial class TopicControl : UserControl
    {
        public TopicControl()
        {
            InitializeComponent();

            this.Loaded += TopicControl_Loaded;
        }

        private void TopicControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void TopicControl_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Topic item = ((FrameworkElement)sender).DataContext as Topic;

            App.RootFrame.Navigate(new Uri("/TopicPage.xaml?id=" + item.id, UriKind.Relative));
        }
    }
}
