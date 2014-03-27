using System.Diagnostics;
using System.Windows;
using Microsoft.Phone.Scheduler;
using BreakingNews.API;
using Microsoft.Phone.Shell;
using BreakingNews.Background.Common;
using System;
using BreakingNews.API.Models;
using System.Threading;

namespace BreakingNews.Background
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        private static volatile bool _classInitialized;

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
            if (e.ExceptionObject is OutOfMemoryException)
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

        protected override void OnInvoke(ScheduledTask task)
        {
            App.BreakingNewsClient = new ServiceClient(Debugger.IsAttached);

            int notifyCompleteLock = 0;

            foreach (ShellTile tile in ShellTile.ActiveTiles)
            {
                notifyCompleteLock++;
            }

            foreach (ShellTile tile in ShellTile.ActiveTiles)
            {
                if (tile.NavigationUri.ToString() == "/") // application tile
                {
                    App.BreakingNewsClient.GetLiveTilePosts((result) =>
                    {
                        Deployment.Current.Dispatcher.BeginInvoke(delegate
                        {
                            FlipTileData data = LiveTileManager.RenderApplicationLiveTile(result.Count > 0 ? result[0] : null);
                            data.Count = result.Count;

                            tile.Update(data);

                            notifyCompleteLock--;
                            if (notifyCompleteLock == 0)
                            {
                                if (System.Diagnostics.Debugger.IsAttached)
                                    ScheduledActionService.LaunchForTest("BackgroundWorker", new TimeSpan(0, 0, 1, 0)); // every minute

                                App.BreakingNewsClient.SaveData();

                                NotifyComplete();
                            }
                        });
                    });
                }
                else
                {
                    string id = tile.NavigationUri.ToString().Split('=')[1];
                    int topicId = Convert.ToInt32(id);

                    App.BreakingNewsClient.GetLiveTileTopicPosts((result) =>
                    {
                        Deployment.Current.Dispatcher.BeginInvoke(delegate
                        {
                            FlipTileData data = LiveTileManager.RenderApplicationLiveTile(result.Count > 0 ? result[0] : null);
                            data.Count = result.Count;

                            tile.Update(data);

                            notifyCompleteLock--;
                            if (notifyCompleteLock == 0)
                            {
                                if (System.Diagnostics.Debugger.IsAttached)
                                    ScheduledActionService.LaunchForTest("BackgroundWorker", new TimeSpan(0, 0, 1, 0)); // every minute

                                App.BreakingNewsClient.SaveData();

                                NotifyComplete();
                            }
                        });
                    }, topicId);
                }
            }
        }
    }
}