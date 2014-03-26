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
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BreakingNews
{
    public partial class PostItemControl : UserControl
    {
        public PostItemControl()
        {
            InitializeComponent();
        }

        private void Topic_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Post item = ((FrameworkElement)sender).DataContext as Post;

            App.RootFrame.Navigate(new Uri("/TopicPage.xaml?id=" + item.topics[0].id, UriKind.Relative));
        }

        private void PostContent_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Post item = ((FrameworkElement)sender).DataContext as Post;

            if (item.url.Length > 0)
            {
                WebBrowserTask webBrowserTask = new WebBrowserTask();
                webBrowserTask.Uri = new Uri(item.url);

                webBrowserTask.Show();
            }
            else
            {
                WebBrowserTask webBrowserTask = new WebBrowserTask();
                webBrowserTask.Uri = new Uri(item.permalink);

                webBrowserTask.Show();
            }
        }

        private void Share_Click(object sender, RoutedEventArgs e)
        {
            Post item = ((FrameworkElement)sender).DataContext as Post;

            ShareLinkTask shareLinkTask = new ShareLinkTask();

            shareLinkTask.Title = item.content;
            if (item.url.Length > 0)
                shareLinkTask.LinkUri = new Uri(item.url);
            else
                shareLinkTask.LinkUri = new Uri(item.permalink);
            shareLinkTask.Message = "Check out this article I found on Breaking News for Windows Phone!";
            shareLinkTask.Show();
        }
    }
}
