using System;
using System.Windows;
using System.Windows.Controls;
using BreakingNews.API.Models;
using Microsoft.Phone.Tasks;

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

            WebBrowserTask webBrowserTask = new WebBrowserTask();
            webBrowserTask.Uri = item.FriendlyUrl;

            webBrowserTask.Show();
        }

        private void Share_Click(object sender, RoutedEventArgs e)
        {
            Post item = ((FrameworkElement)sender).DataContext as Post;

            ShareLinkTask shareLinkTask = new ShareLinkTask();
            shareLinkTask.Title = item.content;
            shareLinkTask.LinkUri = item.FriendlyUrl;
            shareLinkTask.Message = "Check out this article I found on Breaking News for Windows Phone!";
            
            shareLinkTask.Show();
        }
    }
}
