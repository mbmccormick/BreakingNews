using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Marketplace;
using Microsoft.Phone.Tasks;

namespace BreakingNews.Common
{
    public class TrialManager
    {
        private static double MaximumTrialDays = 3.0;
        private static bool ForceTrialMode = false;

        public static void CheckLicense()
        {
            LicenseInformation LicenseInfo = new LicenseInformation();

            if (LicenseInfo.IsTrial() == true ||
                ForceTrialMode == true)
            {
                DateTime? installDate = IsolatedStorageHelper.GetObject<DateTime?>("InstallationDate");
                if (installDate == null)
                {
                    installDate = DateTime.UtcNow;
                    IsolatedStorageHelper.SaveObject("InstallationDate", installDate);
                }

                if ((DateTime.UtcNow - installDate.Value).TotalDays > MaximumTrialDays)
                {
                    CustomMessageBox messageBox = new CustomMessageBox()
                    {
                        Caption = "Trial Expired",
                        Message = "Your trial period for Breaking News has ended. You will need to purchase the app in order to continue using it. Would you like to purchase it now?",
                        LeftButtonContent = "purchase",
                        RightButtonContent = "no thanks",
                        IsFullScreen = false
                    };

                    messageBox.Dismissed += (s1, e1) =>
                    {
                        switch (e1.Result)
                        {
                            case CustomMessageBoxResult.LeftButton:
                                MarketplaceDetailTask marketplaceDetailTask = new MarketplaceDetailTask();
                                marketplaceDetailTask.Show();

                                break;
                            default:
                                Application.Current.Terminate();

                                break;
                        }
                    };

                    messageBox.Show();
                }
            }
        }
    }
}
