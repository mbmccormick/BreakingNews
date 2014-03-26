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
            LittleWatson.ReportException(e.ExceptionObject, null);

            e.Handled = true;

            if (System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debugger.Break();
            }

            NotifyComplete();
        }

        protected override void OnInvoke(ScheduledTask task)
        {
            App.BreakingNewsClient = new ServiceClient(Debugger.IsAttached);

            int notifyCompleteLock = 0;

            foreach (ShellTile tile in ShellTile.ActiveTiles)
            {
                notifyCompleteLock++;

                if (tile.NavigationUri.ToString() == "/") // application tile
                {
                    // FlipTileData data = LiveTileManager.RenderApplicationLiveTile();

                    // tile.Update(data);

                    notifyCompleteLock--;
                    if (notifyCompleteLock == 0) NotifyComplete();
                }
                else
                {
                    string id = tile.NavigationUri.ToString().Split('=')[1];
                    int topicId = Convert.ToInt32(id);

                    App.BreakingNewsClient.GetLiveTilePost((result) =>
                    {
                        FlipTileData data = LiveTileManager.RenderLiveTile(result);

                        tile.Update(data);

                        notifyCompleteLock--;
                        if (notifyCompleteLock == 0) NotifyComplete();
                    }, topicId);
                }
            }
        }
    }
}