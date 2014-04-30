using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Net;
using System.Windows;
using BreakingNews.API;
using BreakingNews.API.Models;
using BreakingNews.Background.Common;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;

namespace BreakingNews.Background
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        private static volatile bool _classInitialized;

        public List<int> NotifiedPosts;
        public int MaxNotifiedPosts = 250;

        public ScheduledAgent()
        {
            if (!_classInitialized)
            {
                _classInitialized = true;
                Deployment.Current.Dispatcher.BeginInvoke(delegate
                {
                    Application.Current.UnhandledException += ScheduledAgent_UnhandledException;
                });
            }
        }

        private void ScheduledAgent_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is WebException)
            {
                // ignore these exceptions
            } 
            else if (e.ExceptionObject is OutOfMemoryException)
            {
                // ignore these exceptions
            }
            else
            {
                LittleWatson.ReportException(e.ExceptionObject, null);
            }

            e.Handled = true;

            if (System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debugger.Break();
            }

            if (System.Diagnostics.Debugger.IsAttached)
                ScheduledActionService.LaunchForTest("BackgroundWorker", new TimeSpan(0, 0, 1, 0)); // every minute

            NotifyComplete();
        }

        int notifyCompleteLock = 0;

        protected async override void OnInvoke(ScheduledTask task)
        {
            App.BreakingNewsClient = new ServiceClient(Debugger.IsAttached);

            NotifiedPosts = IsolatedStorageHelper.GetObject<List<int>>("NotifiedPosts");

            if (NotifiedPosts == null)
                NotifiedPosts = new List<int>();

            notifyCompleteLock = 0;

            foreach (ShellTile tile in ShellTile.ActiveTiles)
            {
                notifyCompleteLock++;
            }

            notifyCompleteLock++;

            await App.BreakingNewsClient.GetLiveTilePosts((result) =>
            {
                Deployment.Current.Dispatcher.BeginInvoke(delegate
                {
                    foreach (var item in result)
                    {
                        // only show new notifications
                        if (NotifiedPosts.Contains(item.id) == false)
                        {
                            ShellToast toast = new ShellToast();
                            toast.Title = "Breaking News";
                            toast.Content = item.push_content;
                            toast.NavigationUri = new Uri("/MainPage.xaml?NavigationUri=" + item.FriendlyUrl.ToString(), UriKind.Relative);

                            toast.Show();

                            MarkPostAsNotified(item);
                        }
                    }

                    notifyCompleteLock--;
                    SafeNotifyComplete();
                });
            });

            foreach (ShellTile tile in ShellTile.ActiveTiles)
            {
                if (tile.NavigationUri.ToString() == "/") // application tile
                {
                    await App.BreakingNewsClient.GetLiveTilePosts((result) =>
                    {
                        Deployment.Current.Dispatcher.BeginInvoke(delegate
                        {
                            FlipTileData data = LiveTileManager.RenderApplicationLiveTile(result.Count > 0 ? result[0] : null);
                            data.Count = result.Count;

                            tile.Update(data);

                            notifyCompleteLock--;
                            SafeNotifyComplete();
                        });
                    });
                }
                else
                {
                    string id = tile.NavigationUri.ToString().Split('=')[1];
                    int topicId = Convert.ToInt32(id);

                    await App.BreakingNewsClient.GetLiveTileTopicPosts((result) =>
                    {
                        Deployment.Current.Dispatcher.BeginInvoke(delegate
                        {
                            FlipTileData data = LiveTileManager.RenderLiveTile(result.Count > 0 ? result[0] : null);
                            data.Count = result.Count;

                            tile.Update(data);

                            notifyCompleteLock--;
                            SafeNotifyComplete();
                        });
                    }, topicId);
                }
            }

            SafeNotifyComplete();
        }

        public void MarkPostAsNotified(Post item)
        {
            bool found = false;
            foreach (int pi in NotifiedPosts)
            {
                if (pi == item.id) found = true;
            }

            if (found == true) return;

            while (NotifiedPosts.Count >= MaxNotifiedPosts)
            {
                NotifiedPosts.RemoveAt(MaxNotifiedPosts - 1);
            }

            NotifiedPosts.Insert(0, item.id);

            IsolatedStorageHelper.SaveObject<List<int>>("NotifiedPosts", NotifiedPosts);
        }

        private void SafeNotifyComplete()
        {
            if (notifyCompleteLock == 0)
            {
                IsolatedStorageSettings.ApplicationSettings.Save();
                
                if (System.Diagnostics.Debugger.IsAttached)
                    ScheduledActionService.LaunchForTest("BackgroundWorker", new TimeSpan(0, 0, 1, 0)); // every minute
                
                NotifyComplete();
            }
        }
    }
}
