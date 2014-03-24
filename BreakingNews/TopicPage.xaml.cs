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

        ApplicationBarIconButton refresh;

        ApplicationBarMenuItem favorite;
        ApplicationBarMenuItem unfavorite;
        ApplicationBarMenuItem feedback;
        ApplicationBarMenuItem about;

        public TopicPage()
        {
            InitializeComponent();

            App.UnhandledExceptionHandled += new EventHandler<ApplicationUnhandledExceptionEventArgs>(App_UnhandledExceptionHandled);

            CurrentTopic = null;
            TopicPosts = new ObservableCollection<Post>();

            this.BuildApplicationBar();
        }

        private void BuildApplicationBar()
        {
            refresh = new ApplicationBarIconButton();
            refresh.IconUri = new Uri("/Resources/Refresh.png", UriKind.RelativeOrAbsolute);
            refresh.Text = "refresh";
            refresh.Click += Refresh_Click;

            favorite = new ApplicationBarMenuItem();
            favorite.Text = "add to favorites";
            favorite.Click += Favorite_Click;

            unfavorite = new ApplicationBarMenuItem();
            unfavorite.Text = "remove from favorites";
            unfavorite.Click += Unfavorite_Click;

            feedback = new ApplicationBarMenuItem();
            feedback.Text = "feedback";
            feedback.Click += Feedback_Click;

            about = new ApplicationBarMenuItem();
            about.Text = "about";
            about.Click += About_Click;

            // build application bar
            ApplicationBar.Buttons.Add(refresh);

            ApplicationBar.MenuItems.Add(favorite);
            ApplicationBar.MenuItems.Add(feedback);
            ApplicationBar.MenuItems.Add(about);
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

                        ToggleFavoriteButton();

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

        private void ToggleFavoriteButton()
        {
            while (ApplicationBar.Buttons.Count > 0)
            {
                ApplicationBar.Buttons.RemoveAt(0);
            }

            while (ApplicationBar.MenuItems.Count > 0)
            {
                ApplicationBar.MenuItems.RemoveAt(0);
            }

            if (CurrentTopic.is_favorited == true)
            {
                ApplicationBar.Buttons.Add(refresh);

                ApplicationBar.MenuItems.Add(unfavorite);
                ApplicationBar.MenuItems.Add(feedback);
                ApplicationBar.MenuItems.Add(about);
            }
            else
            {
                ApplicationBar.Buttons.Add(refresh);

                ApplicationBar.MenuItems.Add(favorite);
                ApplicationBar.MenuItems.Add(feedback);
                ApplicationBar.MenuItems.Add(about);
            }
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            if (this.prgLoading.Visibility == System.Windows.Visibility.Visible) return;

            isTopicLoaded = false;
            isTopicPostsLoaded = false;

            LoadData();
        }

        private void Favorite_Click(object sender, EventArgs e)
        {
            CurrentTopic.is_favorited = true;
            App.BreakingNewsClient.FavoriteTopic(CurrentTopic);

            ToggleFavoriteButton();
        }

        private void Unfavorite_Click(object sender, EventArgs e)
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
                        CurrentTopic.is_favorited = false;
                        App.BreakingNewsClient.UnfavoriteTopic(CurrentTopic.id);

                        ToggleFavoriteButton();

                        break;
                    default:
                        break;
                }
            };

            messageBox.Show();
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
    }
}