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
using System.Collections.ObjectModel;
using BreakingNews.Common;
using Microsoft.Phone.Tasks;

namespace BreakingNews
{
    public partial class TopicPage : PhoneApplicationPage
    {
        #region List Properties

        public static Topic CurrentTopic { get; set; }
        public static ObservableCollection<Post> TopicPosts { get; set; }

        #endregion

        private bool isTopicLoaded = false;
        private bool isTopicPostsLoaded = false;

        public TopicPage()
        {
            InitializeComponent();

            App.UnhandledExceptionHandled += new EventHandler<ApplicationUnhandledExceptionEventArgs>(App_UnhandledExceptionHandled);

            CurrentTopic = null;
            TopicPosts = new ObservableCollection<Post>();
        }

        private void App_UnhandledExceptionHandled(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.txtName.Visibility = System.Windows.Visibility.Collapsed;
                this.txtDescription.Visibility = System.Windows.Visibility.Collapsed;

                ToggleLoadingText();

                this.txtEmpty.Visibility = System.Windows.Visibility.Visible;
                this.txtEmpty.Text = "Sorry, could not download posts right now.";

                this.prgLoading.Visibility = System.Windows.Visibility.Collapsed;
            });
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (isTopicLoaded == false ||
                isTopicPostsLoaded == false)
            {
                LoadData();
            }
        }

        private void LoadData()
        {
            string id;
            if (NavigationContext.QueryString.TryGetValue("id", out id))
            {
                this.prgLoading.Visibility = System.Windows.Visibility.Visible;

                int topicId = Convert.ToInt32(id);

                App.BreakingNewsClient.GetTopic((result) =>
                {
                    SmartDispatcher.BeginInvoke(() =>
                    {
                        CurrentTopic = result;

                        this.txtName.Text = CurrentTopic.name;
                        this.txtDescription.Text = CurrentTopic.description;

                        isTopicLoaded = true;

                        if (isTopicLoaded &&
                            isTopicPostsLoaded)
                        {
                            ToggleLoadingText();
                            ToggleEmptyText();

                            this.prgLoading.Visibility = System.Windows.Visibility.Collapsed;
                        }
                    });
                }, topicId);

                App.BreakingNewsClient.GetTopicPosts((result) =>
                {
                    SmartDispatcher.BeginInvoke(() =>
                    {
                        TopicPosts.Clear();

                        foreach (Post item in result)
                        {
                            TopicPosts.Add(item);
                        }

                        isTopicPostsLoaded = true;

                        if (isTopicLoaded &&
                            isTopicPostsLoaded)
                        {
                            ToggleLoadingText();
                            ToggleEmptyText();

                            this.prgLoading.Visibility = System.Windows.Visibility.Collapsed;
                        }
                    });
                }, topicId);
            }
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
            SmartDispatcher.BeginInvoke(() =>
            {
                this.txtLoading.Visibility = System.Windows.Visibility.Collapsed;

                this.txtName.Visibility = System.Windows.Visibility.Visible;
                this.txtDescription.Visibility = System.Windows.Visibility.Visible;
                this.lstPosts.Visibility = System.Windows.Visibility.Visible;
            });
        }

        private void ToggleEmptyText()
        {
            SmartDispatcher.BeginInvoke(() =>
            {
                if (TopicPosts.Count == 0)
                    this.txtEmpty.Visibility = System.Windows.Visibility.Visible;
                else
                    this.txtEmpty.Visibility = System.Windows.Visibility.Collapsed;
            });
        }
    }
}