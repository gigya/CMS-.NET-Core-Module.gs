using Sitecore.Analytics;
using Sitecore.Analytics.Tracking;
using Sitecore.Diagnostics;
using Sitecore.Exceptions;
using Sitecore.Gigya.DependencyInjection;
using Sitecore.Gigya.Extensions.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.Gigya.Extensions.Services
{
    [Service(typeof(ITrackerService))]
    public class TrackerService : ITrackerService
    {
        private const string _identifierSource = "gigya";

        public bool IsActive => Tracker.Current != null && Tracker.Current.IsActive;

        public void IdentifyContact(string identifier)
        {
            try
            {
                if (this.IsActive)
                {
                    Tracker.Current.Session.IdentifyAs(_identifierSource, identifier);
                }
            }
            catch (ItemNotFoundException ex)
            {
                //Error can happen if previous user profile has been deleted
                Log.Error($"Could not identify the user '{identifier}'", ex, this);
            }
        }



        public void UpdateFacets(dynamic gigyaModel)
        {
            //if (Sitecore.Analytics.Tracker.Current.Contact.IsNew)
            //{
            //    var manager = Sitecore.Configuration.Factory.CreateObject("tracking/contactManager", true) as Sitecore.Analytics.Tracking.ContactManager;

            //    if (manager != null)
            //    {
            //        // Save contact to xConnect; at this point, a contact has an anonymous
            //        // TRACKER IDENTIFIER, which follows a specific format. Do not use the contactId overload
            //        // and make sure you set the ContactSaveMode as demonstrated
            //        Sitecore.Analytics.Tracker.Current.Contact.ContactSaveMode = ContactSaveMode.AlwaysSave;
            //        manager.SaveContactToCollectionDb(Sitecore.Analytics.Tracker.Current.Contact);

            //        // Now that the contact is saved, you can retrieve it using the tracker identifier
            //        // NOTE: Sitecore.Analytics.XConnect.DataAccess.Constants.IdentifierSource is marked internal in 9.0 Initial and cannot be used. If you are using 9.0 Initial, pass "xDB.Tracker" in as a string.
            //        var trackerIdentifier = new IdentifiedContactReference(Sitecore.Analytics.XConnect.DataAccess.Constants.IdentifierSource, Sitecore.Analytics.Tracker.Current.Contact.ContactId.ToString("N"));

            //        // Get contact from xConnect, update and save the facet
            //        using (XConnectClient client = Sitecore.XConnect.Client.Configuration.SitecoreXConnectClientConfiguration.GetClient())
            //        {
            //            try
            //            {
            //                var contact = client.Get<Contact>(trackerIdentifier, new Sitecore.XConnect.ContactExpandOptions());

            //                if (contact != null)
            //                {
            //                    client.SetFacet<PersonalInformation>(contact, PersonalInformation.DefaultFacetKey, new PersonalInformation()
            //                    {
            //                        FirstName = "Myrtle" // Replace with real input source
            //                    });

            //                    client.Submit();

            //                    // Remove contact data from shared session state - contact will be re-loaded
            //                    // during subsequent request with updated facets
            //                    manager.RemoveFromSession(Sitecore.Analytics.Tracker.Current.Contact.ContactId);
            //                    Sitecore.Analytics.Tracker.Current.Session.Contact = manager.LoadContact(Sitecore.Analytics.Tracker.Current.Contact.ContactId);
            //                }
            //            }
            //            catch (XdbExecutionException ex)
            //            {
            //                // Manage conflicts / exceptions
            //            }
            //        }
            //    }
            //}
        }
    }
}
