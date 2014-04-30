using System.Windows;
using System.Windows.Controls;
using BreakingNews.API.Models;
using Microsoft.Phone.Tasks;

namespace BreakingNews
{
    public partial class MediaControl : UserControl
    {
        public MediaControl()
        {
            InitializeComponent();
        }

        private void PostContent_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Post item = ((FrameworkElement)sender).DataContext as Post;

            WebBrowserTask webBrowserTask = new WebBrowserTask();
            webBrowserTask.Uri = item.FriendlyUrl;

            webBrowserTask.Show();
        }
    }
}
