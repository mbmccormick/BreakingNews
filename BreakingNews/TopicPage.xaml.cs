using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Navigation;
using BreakingNews.API.Models;
using BreakingNews.Common;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Windows.ApplicationModel.Store;

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

        ApplicationBarMenuItem follow;
        ApplicationBarMenuItem unfollow;
        ApplicationBarMenuItem feedback;
        ApplicationBarMenuItem donate;
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

            follow = new ApplicationBarMenuItem();
            follow.Text = "follow this topic";
            follow.Click += Follow_Click;

            unfollow = new ApplicationBarMenuItem();
            unfollow.Text = "unfollow this topic";
            unfollow.Click += Unfollow_Click;

            feedback = new ApplicationBarMenuItem();
            feedback.Text = "feedback";
            feedback.Click += Feedback_Click;

            donate = new ApplicationBarMenuItem();
            donate.Text = "donate";
            donate.Click += Donate_Click;

            about = new ApplicationBarMenuItem();
            about.Text = "about";
            about.Click += About_Click;

            // build application bar
            ApplicationBar.Buttons.Add(refresh);

            ApplicationBar.MenuItems.Add(follow);
            ApplicationBar.MenuItems.Add(feedback);
            ApplicationBar.MenuItems.Add(donate);
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
            if (e.IsNavigationInitiator == false)
            {
                LittleWatson.CheckForPreviousException(true);

                FeedbackHelper.PromptForRating();
            }
            
            if (isTopicLoaded == false ||
                isTopicPostsLoaded == false)
            {
                NotificationsManager.SetupNotifications();
                NotificationsManager.ResetLiveTiles();

                LoadData();
            }
        }

        private async void LoadData()
        {
            string id;
            if (NavigationContext.QueryString.TryGetValue("id", out id))
            {
                this.prgLoading.Visibility = System.Windows.Visibility.Visible;

                int topicId = Convert.ToInt32(id);

                await App.BreakingNewsClient.GetTopic((result) =>
                {
                    SmartDispatcher.BeginInvoke(() =>
                    {
                        CurrentTopic = result;

                        this.txtName.Text = CurrentTopic.name;
                        this.txtDescription.Text = CurrentTopic.description;

                        ToggleFollowButton();

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

                await App.BreakingNewsClient.GetTopicPosts((result) =>
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

        private void ToggleFollowButton()
        {
            while (ApplicationBar.Buttons.Count > 0)
            {
                ApplicationBar.Buttons.RemoveAt(0);
            }

            while (ApplicationBar.MenuItems.Count > 0)
            {
                ApplicationBar.MenuItems.RemoveAt(0);
            }

            if (CurrentTopic.is_following == true)
            {
                ApplicationBar.Buttons.Add(refresh);

                ApplicationBar.MenuItems.Add(unfollow);
                ApplicationBar.MenuItems.Add(feedback);
                ApplicationBar.MenuItems.Add(donate);
                ApplicationBar.MenuItems.Add(about);
            }
            else
            {
                ApplicationBar.Buttons.Add(refresh);

                ApplicationBar.MenuItems.Add(follow);
                ApplicationBar.MenuItems.Add(feedback);
                ApplicationBar.MenuItems.Add(donate);
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

        private void Follow_Click(object sender, EventArgs e)
        {
            CurrentTopic.is_following = true;
            App.BreakingNewsClient.FollowTopic(CurrentTopic);

            ToggleFollowButton();
        }

        private void Unfollow_Click(object sender, EventArgs e)
        {
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
                        CurrentTopic.is_following = false;
                        App.BreakingNewsClient.UnfollowTopic(CurrentTopic.id);

                        ToggleFollowButton();

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

            FeedbackHelper.Default.Feedback();
        }

        private async void Donate_Click(object sender, EventArgs e)
        {
            try
            {
                var productList = await CurrentApp.LoadListingInformationAsync();
                var product = productList.ProductListings.FirstOrDefault(p => p.Value.ProductType == ProductType.Consumable);
                var receipt = await CurrentApp.RequestProductPurchaseAsync(product.Value.ProductId, true);

                if (CurrentApp.LicenseInformation.ProductLicenses[product.Value.ProductId].IsActive)
                {
                    CurrentApp.ReportProductFulfillment(product.Value.ProductId);

                    MessageBox.Show("Thank you for your donation! Your support motivates me to keep developing for Breaking News, the best Breaking News client for Windows Phone.", "Thank You", MessageBoxButton.OK);
                }
            }
            catch (Exception ex)
            {
                // do nothing
            }
        }

        private void About_Click(object sender, EventArgs e)
        {
            if (this.prgLoading.Visibility == System.Windows.Visibility.Visible) return;

            NavigationService.Navigate(new Uri("/YourLastAboutDialog;component/AboutPage.xaml", UriKind.Relative));
        }

        private void Description_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (this.txtDescription.Text.Length == 140)
            {
                this.txtDescription.Text = CurrentTopic.description;
            }
            else if (this.txtDescription.Text.Length > 137)
            {
                this.txtDescription.Text = this.txtDescription.Text.Substring(0, 137).Trim() + "...";
                this.txtDescription.Text.Replace("....", "...");
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

                        App.BreakingNewsClient.GetNextTopicPosts((result) =>
                        {
                            SmartDispatcher.BeginInvoke(() =>
                            {
                                foreach (Post item in result)
                                {
                                    TopicPosts.Add(item);
                                }

                                this.prgLoading.Visibility = System.Windows.Visibility.Collapsed;
                            });
                        });
                    }
                }
            }
        }

        private void LongListSelector_ItemUnrealized(object sender, ItemRealizationEventArgs e)
        {
        }
    }
}