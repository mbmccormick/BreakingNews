using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    public partial class PhotoViewerPage : PhoneApplicationPage
    {
        #region List Properties

        public static Post CurrentPost { get; set; }

        #endregion

        private bool isPostLoaded = false;

        public PhotoViewerPage()
        {
            InitializeComponent();

            App.UnhandledExceptionHandled += new EventHandler<ApplicationUnhandledExceptionEventArgs>(App_UnhandledExceptionHandled);

            CurrentPost = null;
        }

        private void App_UnhandledExceptionHandled(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.imgMedia.Visibility = System.Windows.Visibility.Collapsed;
                this.txtContent.Visibility = System.Windows.Visibility.Collapsed;
                this.txtDescription.Visibility = System.Windows.Visibility.Collapsed;

                ToggleLoadingText();

                this.txtEmpty.Visibility = System.Windows.Visibility.Visible;
                this.txtEmpty.Text = "Sorry, could not download this photo right now.";

                this.prgLoading.Visibility = System.Windows.Visibility.Collapsed;
            });
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (isPostLoaded == false)
            {
                LoadData();
            }
        }

        private async void LoadData()
        {
            string id;
            if (NavigationContext.QueryString.TryGetValue("id", out id))
            {
                this.prgLoading.Visibility = System.Windows.Visibility.Visible;

                ResetDefaultLayout();

                int postId = Convert.ToInt32(id);

                await App.BreakingNewsClient.GetPost((result) =>
                {
                    SmartDispatcher.BeginInvoke(() =>
                    {
                        CurrentPost = result;

                        this.imgMedia.Source = CurrentPost.FriendlyImage;
                        this.txtContent.Text = CurrentPost.content;
                        this.txtDescription.Text = CurrentPost.FriendlyDate;

                        isPostLoaded = true;

                        if (isPostLoaded)
                        {
                            ToggleLoadingText();

                            this.prgLoading.Visibility = System.Windows.Visibility.Collapsed;
                        }
                    });
                }, postId);
            }
        }

        private void ResetDefaultLayout()
        {
            this.txtEmpty.Visibility = System.Windows.Visibility.Collapsed;

            if (CurrentPost == null)
            {
                this.txtLoading.Visibility = System.Windows.Visibility.Visible;

                this.imgMedia.Visibility = System.Windows.Visibility.Collapsed;
                this.txtContent.Visibility = System.Windows.Visibility.Collapsed;
                this.txtDescription.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void ToggleLoadingText()
        {
            this.txtLoading.Visibility = System.Windows.Visibility.Collapsed;

            this.imgMedia.Visibility = System.Windows.Visibility.Visible;
            this.txtContent.Visibility = System.Windows.Visibility.Visible;
            this.txtDescription.Visibility = System.Windows.Visibility.Visible;
        }

        private void ImageContent_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (this.stkCaption.Visibility == System.Windows.Visibility.Visible)
            {
                this.stkCaption.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                this.stkCaption.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void PostContent_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            WebBrowserTask webBrowserTask = new WebBrowserTask();
            webBrowserTask.Uri = CurrentPost.FriendlyUrl;

            webBrowserTask.Show();
        }
    }
}