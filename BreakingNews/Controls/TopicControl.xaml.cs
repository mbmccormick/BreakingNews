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
using BreakingNews.Common;

namespace BreakingNews
{
    public partial class TopicControl : UserControl
    {
        public event EventHandler<EventArgs> FollowsChanged;

        public TopicControl()
        {
            InitializeComponent();

            this.Loaded += TopicControl_Loaded;
        }

        private void TopicControl_Loaded(object sender, RoutedEventArgs e)
        {
            TopicItem item = this.DataContext as TopicItem;

            if (item.is_following)
            {
                this.vbxFollow.Visibility = System.Windows.Visibility.Collapsed;
                this.vbxUnfollow.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                this.vbxFollow.Visibility = System.Windows.Visibility.Visible;
                this.vbxUnfollow.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void Topic_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            TopicItem item = ((FrameworkElement)sender).DataContext as TopicItem;

            App.RootFrame.Navigate(new Uri("/TopicPage.xaml?id=" + item.id, UriKind.Relative));
        }

        private void Follow_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            TopicItem item = ((FrameworkElement)sender).DataContext as TopicItem;

            item.is_following = true;
            App.BreakingNewsClient.FollowTopic(item);

            TopicControl_Loaded(sender, null);

            if (FollowsChanged != null)
                FollowsChanged(sender, e);
        }

        private void Unfollow_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            TopicItem item = ((FrameworkElement)sender).DataContext as TopicItem;

            CustomMessageBox messageBox = new CustomMessageBox()
                {
                    Caption = "Unfollow topic",
                    Message = "Are you sure you want to unfollow this topic?",
                    LeftButtonContent = "yes",
                    RightButtonContent = "no",
                    IsFullScreen = false
                };

            messageBox.Dismissed += (s1, e1) =>
            {
                switch (e1.Result)
                {
                    case CustomMessageBoxResult.LeftButton:
                        item.is_following = false;
                        App.BreakingNewsClient.UnfollowTopic(item.id);

                        TopicControl_Loaded(sender, null);

                        if (FollowsChanged != null)
                            FollowsChanged(sender, e);

                        break;
                    default:
                        break;
                }
            };

            messageBox.Show();
        }

        private void PinToStart_Click(object sender, RoutedEventArgs e)
        {
            TopicItem item = ((FrameworkElement)sender).DataContext as TopicItem;

            ShareLinkTask shareLinkTask = new ShareLinkTask();

            ShellTile secondaryTile = null;
            secondaryTile = ShellTile.ActiveTiles.FirstOrDefault(t => t.NavigationUri.ToString().Contains("id=" + item.id));

            if (secondaryTile == null)
            {
                FlipTileData data = new FlipTileData();
                data = LiveTileManager.RenderLiveTile(item);

                ShellTile.Create(new Uri("/TopicPage.xaml?id=" + item.id, UriKind.Relative), data, true);
            }
            else
            {
                MessageBox.Show("This topic is already pinned to your start screen. If you need to replace it, remove the tile from your start screen and then try again.", "Pin to start", MessageBoxButton.OK);
            }
        }
    }
}
