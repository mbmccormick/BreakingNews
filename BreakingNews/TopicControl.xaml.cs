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
        public event EventHandler<EventArgs> FavoritesChanged;

        public TopicControl()
        {
            InitializeComponent();

            this.Loaded += TopicControl_Loaded;
        }

        private void TopicControl_Loaded(object sender, RoutedEventArgs e)
        {
            TopicItem item = this.DataContext as TopicItem;

            if (item.is_favorited)
            {
                this.vbxFavorite.Visibility = System.Windows.Visibility.Collapsed;
                this.vbxUnfavorite.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                this.vbxFavorite.Visibility = System.Windows.Visibility.Visible;
                this.vbxUnfavorite.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void Topic_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            TopicItem item = ((FrameworkElement)sender).DataContext as TopicItem;

            App.RootFrame.Navigate(new Uri("/TopicPage.xaml?id=" + item.id, UriKind.Relative));
        }

        private void Favorite_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            TopicItem item = ((FrameworkElement)sender).DataContext as TopicItem;

            if (sender == this.vbxFavorite)
            {
                item.is_favorited = true;
                App.BreakingNewsClient.FavoriteTopic(item);

                TopicControl_Loaded(sender, null);

                if (FavoritesChanged != null)
                    FavoritesChanged(sender, e);
            }
            else
            {
                CustomMessageBox messageBox = new CustomMessageBox()
                {
                    Caption = "Remove from favorites",
                    Message = "Are you sure you want to remove this topic from your favorites?",
                    LeftButtonContent = "yes",
                    RightButtonContent = "no",
                    IsFullScreen = false
                };

                messageBox.Dismissed += (s1, e1) =>
                {
                    switch (e1.Result)
                    {
                        case CustomMessageBoxResult.LeftButton:
                            item.is_favorited = false;
                            App.BreakingNewsClient.UnfavoriteTopic(item.id);

                            TopicControl_Loaded(sender, null);

                            if (FavoritesChanged != null)
                                FavoritesChanged(sender, e);

                            break;
                        default:
                            break;
                    }
                };

                messageBox.Show();
            }
        }
    }
}
