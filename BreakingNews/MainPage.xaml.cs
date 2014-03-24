﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using BreakingNews.API;
using BreakingNews.API.Models;
using System.Collections.ObjectModel;
using BreakingNews.Common;
using Microsoft.Phone.Tasks;
using System.Windows.Media;

namespace BreakingNews
{
    public partial class MainPage : PhoneApplicationPage
    {
        #region List Properties

        public static ObservableCollection<Post> LatestPosts { get; set; }
        public static ObservableCollection<Post> PopularPosts { get; set; }
        public static ObservableCollection<TopicItem> Topics { get; set; }
        public static ObservableCollection<TopicItem> FavoriteTopics { get; set; }

        #endregion

        private bool isLatestLoaded = false;
        private bool isPopularLoaded = false;
        private bool isTopicsLoaded = false;
        private bool isFavoriteTopicsLoaded = false;

        public MainPage()
        {
            InitializeComponent();

            App.UnhandledExceptionHandled += new EventHandler<ApplicationUnhandledExceptionEventArgs>(App_UnhandledExceptionHandled);

            LatestPosts = new ObservableCollection<Post>();
            PopularPosts = new ObservableCollection<Post>();
            Topics = new ObservableCollection<TopicItem>();
            FavoriteTopics = new ObservableCollection<TopicItem>();
        }

        private void App_UnhandledExceptionHandled(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                ToggleLoadingText();
                ToggleEmptyText();

                this.prgLoading.Visibility = System.Windows.Visibility.Collapsed;
            });
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (e.IsNavigationInitiator == false)
            {
                LittleWatson.CheckForPreviousException(true);

                if (isLatestLoaded == false ||
                    isPopularLoaded == false ||
                    isTopicsLoaded == false ||
                    isFavoriteTopicsLoaded == false)
                    LoadData(false);
            }
            else
            {
                LoadData(true);
            }
        }

        private void LoadData(bool isNavigationInitiator)
        {
            this.prgLoading.Visibility = System.Windows.Visibility.Visible;

            if (isNavigationInitiator == false)
            {
                App.BreakingNewsClient.GetLatestPosts((result) =>
                {
                    SmartDispatcher.BeginInvoke(() =>
                    {
                        LatestPosts.Clear();

                        foreach (Post item in result)
                        {
                            LatestPosts.Add(item);
                        }

                        isLatestLoaded = true;

                        if (isLatestLoaded &&
                            isPopularLoaded &&
                            isTopicsLoaded &&
                            isFavoriteTopicsLoaded)
                        {
                            ToggleLoadingText();
                            ToggleEmptyText();

                            this.prgLoading.Visibility = System.Windows.Visibility.Collapsed;
                        }
                    });
                });

                App.BreakingNewsClient.GetPopularPosts((result) =>
                {
                    SmartDispatcher.BeginInvoke(() =>
                    {
                        PopularPosts.Clear();

                        foreach (Post item in result)
                        {
                            PopularPosts.Add(item);
                        }

                        isPopularLoaded = true;

                        if (isLatestLoaded &&
                            isPopularLoaded &&
                            isTopicsLoaded &&
                            isFavoriteTopicsLoaded)
                        {
                            ToggleLoadingText();
                            ToggleEmptyText();

                            this.prgLoading.Visibility = System.Windows.Visibility.Collapsed;
                        }
                    });
                });
            }

            App.BreakingNewsClient.GetOngoingTopics((result) =>
            {
                SmartDispatcher.BeginInvoke(() =>
                {
                    Topics.Clear();

                    foreach (TopicItem item in result)
                    {
                        Topics.Add(item);
                    }

                    isTopicsLoaded = true;

                    if (isLatestLoaded &&
                        isPopularLoaded &&
                        isTopicsLoaded &&
                        isFavoriteTopicsLoaded)
                    {
                        ToggleLoadingText();
                        ToggleEmptyText();

                        this.prgLoading.Visibility = System.Windows.Visibility.Collapsed;
                    }
                });
            });

            App.BreakingNewsClient.GetFavoriteTopics((result) =>
            {
                SmartDispatcher.BeginInvoke(() =>
                {
                    FavoriteTopics.Clear();

                    foreach (TopicItem item in result)
                    {
                        FavoriteTopics.Add(item);
                    }

                    isFavoriteTopicsLoaded = true;

                    if (isLatestLoaded &&
                        isPopularLoaded &&
                        isTopicsLoaded &&
                        isFavoriteTopicsLoaded)
                    {
                        ToggleLoadingText();
                        ToggleEmptyText();

                        this.prgLoading.Visibility = System.Windows.Visibility.Collapsed;
                    }
                });
            });
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            if (this.prgLoading.Visibility == System.Windows.Visibility.Visible) return;

            isLatestLoaded = false;
            isPopularLoaded = false;
            isTopicsLoaded = false;
            isFavoriteTopicsLoaded = false;

            LoadData(false);
        }

        private void Feedback_Click(object sender, EventArgs e)
        {
            if (this.prgLoading.Visibility == System.Windows.Visibility.Visible) return;

            EmailComposeTask emailComposeTask = new EmailComposeTask();

            emailComposeTask.To = App.FeedbackEmailAddress;
            emailComposeTask.Subject = "Breaking News Feedback";
            emailComposeTask.Body = "Version " + App.ExtendedVersionNumber + " (" + App.PlatformVersionNumber + ")\n\n";
            emailComposeTask.Show();
        }

        private void About_Click(object sender, EventArgs e)
        {
            if (this.prgLoading.Visibility == System.Windows.Visibility.Visible) return;

            NavigationService.Navigate(new Uri("/YourLastAboutDialog;component/AboutPage.xaml", UriKind.Relative));
        }

        private void ToggleLoadingText()
        {
            this.txtLatestPostsLoading.Visibility = System.Windows.Visibility.Collapsed;
            this.txtPopularPostsLoading.Visibility = System.Windows.Visibility.Collapsed;
            this.txtTopicsLoading.Visibility = System.Windows.Visibility.Collapsed;
            this.txtFavoriteTopicsLoading.Visibility = System.Windows.Visibility.Collapsed;

            this.lstLatestPosts.Visibility = System.Windows.Visibility.Visible;
            this.lstPopularPosts.Visibility = System.Windows.Visibility.Visible;
            this.lstTopics.Visibility = System.Windows.Visibility.Visible;
            this.lstFavoriteTopics.Visibility = System.Windows.Visibility.Visible;
        }

        private void ToggleEmptyText()
        {
            if (LatestPosts.Count == 0)
                this.txtLatestPostsEmpty.Visibility = System.Windows.Visibility.Visible;
            else
                this.txtLatestPostsEmpty.Visibility = System.Windows.Visibility.Collapsed;

            if (PopularPosts.Count == 0)
                this.txtPopularPostsEmpty.Visibility = System.Windows.Visibility.Visible;
            else
                this.txtPopularPostsEmpty.Visibility = System.Windows.Visibility.Collapsed;

            if (Topics.Count == 0)
                this.txtTopicsEmpty.Visibility = System.Windows.Visibility.Visible;
            else
                this.txtTopicsEmpty.Visibility = System.Windows.Visibility.Collapsed;

            if (FavoriteTopics.Count == 0)
                this.txtFavoriteTopicsEmpty.Visibility = System.Windows.Visibility.Visible;
            else
                this.txtFavoriteTopicsEmpty.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void TopicControl_FavoritesChanged(object sender, EventArgs e)
        {
            LoadData(true);
        }
    }
}