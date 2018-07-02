using Sitecore.Analytics;
using Sitecore.Analytics.Tracking;
using Sitecore.Diagnostics;
using Sitecore.Exceptions;
using Sitecore.Gigya.Extensions.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.Gigya.Connector.Services
{
    public class TrackerService : ITrackerService
    {
        public bool IsActive => Tracker.Current != null && Tracker.Current.IsActive;

        public void IdentifyContact(string identifier)
        {
            try
            {
                if (this.IsActive)
                {
                    Tracker.Current.Session.IdentifyAs(Sitecore.Gigya.Extensions.Abstractions.Analytics.Constants.IdentifierSource, identifier);
                }
            }
            catch (ItemNotFoundException ex)
            {
                //Error can happen if previous user profile has been deleted
                Log.Error($"Could not identify the user '{identifier}'", ex, this);
            }
        }
    }
}
