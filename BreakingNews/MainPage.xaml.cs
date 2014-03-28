using System;
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
    public partial class MainPage : PhoneApplicationPage
    {
        #region List Properties

        public static ObservableCollection<Post> LatestPosts { get; set; }
        public static ObservableCollection<Post> PopularPosts { get; set; }
        public static ObservableCollection<TopicItem> FollowedTopics { get; set; }

        #endregion

        private bool isLatestLoaded = false;
        private bool isPopularLoaded = false;
        private bool isFollowedTopicsLoaded = false;

        ApplicationBarIconButton refresh;
        ApplicationBarIconButton add;

        ApplicationBarMenuItem feedback;
        ApplicationBarMenuItem about;

        public MainPage()
        {
            InitializeComponent();

            App.UnhandledExceptionHandled += new EventHandler<ApplicationUnhandledExceptionEventArgs>(App_UnhandledExceptionHandled);

            LatestPosts = new ObservableCollection<Post>();
            PopularPosts = new ObservableCollection<Post>();
            FollowedTopics = new ObservableCollection<TopicItem>();

            this.BuildApplicationBar();
        }

        private void BuildApplicationBar()
        {
            refresh = new ApplicationBarIconButton();
            refresh.IconUri = new Uri("/Resources/Refresh.png", UriKind.RelativeOrAbsolute);
            refresh.Text = "refresh";
            refresh.Click += Refresh_Click;

            add = new ApplicationBarIconButton();
            add.IconUri = new Uri("/Resources/Add.png", UriKind.RelativeOrAbsolute);
            add.Text = "add";
            add.Click += Add_Click;

            feedback = new ApplicationBarMenuItem();
            feedback.Text = "feedback";
            feedback.Click += Feedback_Click;

            about = new ApplicationBarMenuItem();
            about.Text = "about";
            about.Click += About_Click;

            // build application bar
            ApplicationBar.Buttons.Add(refresh);

            ApplicationBar.MenuItems.Add(feedback);
            ApplicationBar.MenuItems.Add(about);

            this.pivLayout.SelectionChanged += Layout_SelectionChanged;
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

                TrialManager.CheckLicense();

                if (isLatestLoaded == false ||
                    isPopularLoaded == false ||
                    isFollowedTopicsLoaded == false)
                {
                    NotificationsManager.SetupNotifications();
                    NotificationsManager.ResetLiveTiles();

                    LoadData(false);
                }

                FeedbackHelper.PromptForRating();
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
                            isFollowedTopicsLoaded)
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
                            isFollowedTopicsLoaded)
                        {
                            ToggleLoadingText();
                            ToggleEmptyText();

                            this.prgLoading.Visibility = System.Windows.Visibility.Collapsed;
                        }
                    });
                });
            }

            App.BreakingNewsClient.GetFollowedTopics((result) =>
            {
                SmartDispatcher.BeginInvoke(() =>
                {
                    FollowedTopics.Clear();

                    foreach (TopicItem item in result)
                    {
                        FollowedTopics.Add(item);
                    }

                    isFollowedTopicsLoaded = true;

                    if (isLatestLoaded &&
                        isPopularLoaded &&
                        isFollowedTopicsLoaded)
                    {
                        ToggleLoadingText();
                        ToggleEmptyText();

                        this.prgLoading.Visibility = System.Windows.Visibility.Collapsed;
                    }
                });
            });
        }

        private void ToggleLoadingText()
        {
            this.txtLatestPostsLoading.Visibility = System.Windows.Visibility.Collapsed;
            this.txtPopularPostsLoading.Visibility = System.Windows.Visibility.Collapsed;
            this.txtFollowedTopicsLoading.Visibility = System.Windows.Visibility.Collapsed;

            this.lstLatestPosts.Visibility = System.Windows.Visibility.Visible;
            this.lstPopularPosts.Visibility = System.Windows.Visibility.Visible;
            this.lstFollowedTopics.Visibility = System.Windows.Visibility.Visible;
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

            if (FollowedTopics.Count == 0)
                this.txtFollowedTopicsEmpty.Visibility = System.Windows.Visibility.Visible;
            else
                this.txtFollowedTopicsEmpty.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            if (this.prgLoading.Visibility == System.Windows.Visibility.Visible) return;

            isLatestLoaded = false;
            isPopularLoaded = false;
            isFollowedTopicsLoaded = false;

            LoadData(false);
        }

        private void Add_Click(object sender, EventArgs e)
        {
            if (this.prgLoading.Visibility == System.Windows.Visibility.Visible) return;

            NavigationService.Navigate(new Uri("/TopicSelectorPage.xaml", UriKind.Relative));
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

        private void TopicControl_FollowsChanged(object sender, EventArgs e)
        {
            LoadData(true);
        }

        private void Layout_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            while (ApplicationBar.Buttons.Count > 0)
            {
                ApplicationBar.Buttons.RemoveAt(0);
            }

            while (ApplicationBar.MenuItems.Count > 0)
            {
                ApplicationBar.MenuItems.RemoveAt(0);
            }

            if (this.pivLayout.SelectedIndex < 2)
            {
                ApplicationBar.Buttons.Add(refresh);

                ApplicationBar.MenuItems.Add(feedback);
                ApplicationBar.MenuItems.Add(about);
            }
            else
            {
                ApplicationBar.Buttons.Add(add);

                ApplicationBar.MenuItems.Add(feedback);
                ApplicationBar.MenuItems.Add(about);
            }
        }

        private int _offsetKnob = 5;

        private void LongListSelector_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            if (this.prgLoading.Visibility == System.Windows.Visibility.Visible) return;

            LongListSelector target = (LongListSelector)sender;

            if (target.ItemsSource != null &&
                target.ItemsSource.Count >= _offsetKnob)
            {
                if (e.ItemKind == LongListSelectorItemKind.Item)
                {
                    if ((e.Container.Content as Post).Equals(target.ItemsSource[target.ItemsSource.Count - _offsetKnob]))
                    {
                        this.prgLoading.Visibility = System.Windows.Visibility.Visible;

                        if (target == this.lstLatestPosts)
                        {
                            App.BreakingNewsClient.GetNextLatestPosts((result) =>
                            {
                                SmartDispatcher.BeginInvoke(() =>
                                {
                                    foreach (Post item in result)
                                    {
                                        LatestPosts.Add(item);
                                    }

                                    this.prgLoading.Visibility = System.Windows.Visibility.Collapsed;
                                });
                            });
                        }
                        else if (target == this.lstPopularPosts)
                        {
                            App.BreakingNewsClient.GetNextPopularPosts((result) =>
                            {
                                SmartDispatcher.BeginInvoke(() =>
                                {
                                    foreach (Post item in result)
                                    {
                                        PopularPosts.Add(item);
                                    }

                                    this.prgLoading.Visibility = System.Windows.Visibility.Collapsed;
                                });
                            });
                        }
                    }
                }
            }
        }

        private void LongListSelector_ItemUnrealized(object sender, ItemRealizationEventArgs e)
        {
        }
    }
}