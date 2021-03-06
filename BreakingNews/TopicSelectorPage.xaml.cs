﻿using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using BreakingNews.API.Models;
using BreakingNews.Common;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace BreakingNews
{
    public partial class TopicSelectorPage : PhoneApplicationPage
    {
        #region List Properties

        public static ObservableCollection<TopicItem> Topics { get; set; }

        #endregion

        private bool isTopicsLoaded = false;

        ApplicationBarMenuItem feedback;
        ApplicationBarMenuItem about;

        public TopicSelectorPage()
        {
            InitializeComponent();

            App.UnhandledExceptionHandled += new EventHandler<ApplicationUnhandledExceptionEventArgs>(App_UnhandledExceptionHandled);

            Topics = new ObservableCollection<TopicItem>();

            this.BuildApplicationBar();
        }

        private void BuildApplicationBar()
        {
            feedback = new ApplicationBarMenuItem();
            feedback.Text = "feedback";
            feedback.Click += Feedback_Click;

            about = new ApplicationBarMenuItem();
            about.Text = "about";
            about.Click += About_Click;

            // build application bar
            ApplicationBar.MenuItems.Add(feedback);
            ApplicationBar.MenuItems.Add(about);
        }

        private void App_UnhandledExceptionHandled(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.txtEmpty.Visibility = System.Windows.Visibility.Visible;
                this.txtEmpty.Text = "Sorry, could not download topics right now.";

                this.prgLoading.Visibility = System.Windows.Visibility.Collapsed;
            });
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (isTopicsLoaded == false)
            {
                LoadData();
            }
        }

        private async void LoadData()
        {
            this.prgLoading.Visibility = System.Windows.Visibility.Visible;

            ResetDefaultLayout();

            await App.BreakingNewsClient.GetOngoingTopics((result) =>
            {
                SmartDispatcher.BeginInvoke(() =>
                {
                    Topics.Clear();

                    foreach (TopicItem item in result)
                    {
                        Topics.Add(item);
                    }

                    isTopicsLoaded = true;

                    if (isTopicsLoaded)
                    {
                        ToggleLoadingText();
                        ToggleEmptyText();

                        this.prgLoading.Visibility = System.Windows.Visibility.Collapsed;
                    }
                });
            });
        }

        private void ResetDefaultLayout()
        {
            this.txtEmpty.Visibility = System.Windows.Visibility.Collapsed;

            if (Topics.Count == 0)
            {
                this.txtLoading.Visibility = System.Windows.Visibility.Visible;

                this.lstTopics.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void ToggleLoadingText()
        {
            this.txtLoading.Visibility = System.Windows.Visibility.Collapsed;

            this.lstTopics.Visibility = System.Windows.Visibility.Visible;
        }

        private void ToggleEmptyText()
        {
            if (Topics.Count == 0)
                this.txtEmpty.Visibility = System.Windows.Visibility.Visible;
            else
                this.txtEmpty.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void Feedback_Click(object sender, EventArgs e)
        {
            if (this.prgLoading.Visibility == System.Windows.Visibility.Visible) return;

            FeedbackHelper.Default.Feedback();
        }

        private void About_Click(object sender, EventArgs e)
        {
            if (this.prgLoading.Visibility == System.Windows.Visibility.Visible) return;

            NavigationService.Navigate(new Uri("/YourLastAboutDialog;component/AboutPage.xaml", UriKind.Relative));
        }

        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.txtSearch.Text.Length == 0)
            {
                LoadData();
                return;
            }

            this.prgLoading.Visibility = System.Windows.Visibility.Visible;

            App.BreakingNewsClient.GetTopics((result) =>
            {
                SmartDispatcher.BeginInvoke(() =>
                {
                    Topics.Clear();

                    foreach (TopicItem item in result)
                    {
                        Topics.Add(item);
                    }

                    isTopicsLoaded = true;

                    if (isTopicsLoaded)
                    {
                        ToggleLoadingText();
                        ToggleEmptyText();

                        this.prgLoading.Visibility = System.Windows.Visibility.Collapsed;
                    }
                });
            }, this.txtSearch.Text);
        }

        private void TopicItemControl_FollowsChanged(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}