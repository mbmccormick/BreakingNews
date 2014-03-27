using System;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;

namespace BreakingNews.Common
{
    public class NotificationsManager
    {
        public static void SetupNotifications()
        {
            if (ScheduledActionService.Find("BackgroundWorker") == null)
            {
                PeriodicTask task = new PeriodicTask("BackgroundWorker");
                task.Description = "Handles the live tile updates and notifications for Breaking News.";

                ScheduledActionService.Add(task);
            }

            // increase background worder interval for debug mode
            if (System.Diagnostics.Debugger.IsAttached)
                ScheduledActionService.LaunchForTest("BackgroundWorker", new TimeSpan(0, 0, 1, 0)); // every minute
        }

        public static void ResetLiveTiles()
        {
            foreach (ShellTile tile in ShellTile.ActiveTiles)
            {
                SmartDispatcher.BeginInvoke(() =>
                {
                    FlipTileData data = new FlipTileData();
                    data.Count = 0;
                    data.BackBackgroundImage = null;
                    data.WideBackBackgroundImage = null;
                        
                    tile.Update(data);
                });
            }
        }
    }
}
