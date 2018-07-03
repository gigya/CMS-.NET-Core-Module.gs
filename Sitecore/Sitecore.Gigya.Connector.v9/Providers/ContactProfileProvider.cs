using A = Sitecore.Analytics;
//using Sitecore.Analytics.Tracking;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Gigya.Extensions.Abstractions.Analytics.Facets;
using Sitecore.Gigya.Extensions.Abstractions.Services;
using Sitecore.XConnect;
using Sitecore.XConnect.Collection.Model;
using System;
using System.Collections.Generic;
using System.Linq;

using C = Sitecore.Gigya.Extensions.Abstractions.Analytics.Constants;
using Sitecore.XConnect.Client.Configuration;
using Sitecore.XConnect.Client;
using Sitecore.Analytics.Model;
using Sitecore.Gigya.XConnect.Models;

namespace Sitecore.Gigya.Connector.Providers
{
    public class ContactProfileProvider : IContactProfileProvider
    {
        private XConnectClient _client;
        private Contact _contact;
        //private Logger _logger = 

        public ContactProfileProvider()
        {
            _client = SitecoreXConnectClientConfiguration.GetClient();
        }

        public Contact Contact
        {
            get
            {
                if (_contact != null)
                {
                    return _contact;
                }

                if (!A.Tracker.IsActive)
                {
                    return null;
                }

                _contact = GetContact();
                return _contact;
            }
        }

        private Contact GetContact()
        {
            if (A.Tracker.Current.Contact == null)
            {
                return null;
            }

            if (A.Tracker.Current.Contact.IsNew)
            {
                return CreateContact();
            }

            var identifier = A.Tracker.Current.Contact.Identifiers.FirstOrDefault();
            if (identifier == null)
            {
                // unable to get a contact if we don't have a known identifier
                return null;
            }

            var contactIdentifier = new IdentifiedContactReference(identifier.Source, identifier.Identifier);
            var expandOptions = ExpandOptions();
            var contact = _client.Get(contactIdentifier, expandOptions);
            return contact;
        }

        private static ContactExpandOptions ExpandOptions()
        {
            return new ContactExpandOptions(PersonalInformation.DefaultFacetKey, EmailAddressList.DefaultFacetKey, AddressList.DefaultFacetKey, PhoneNumberList.DefaultFacetKey, C.FacetKeys.Gigya);
        }

        private Contact CreateContact()
        {
            var manager = Factory.CreateObject("tracking/contactManager", true) as A.Tracking.ContactManager;
            if (manager == null)
            {
                return null;
            }

            A.Tracker.Current.Contact.ContactSaveMode = ContactSaveMode.AlwaysSave;
            manager.SaveContactToCollectionDb(A.Tracker.Current.Contact);

            // Now that the contact is saved, you can retrieve it using the tracker identifier
            // NOTE: Sitecore.Analytics.XConnect.DataAccess.Constants.IdentifierSource is marked internal in 9.0 Initial and cannot be used. If you are using 9.0 Initial, pass "xDB.Tracker" in as a string.
            var trackerIdentifier = new IdentifiedContactReference("xDB.Tracker", A.Tracker.Current.Contact.ContactId.ToString("N"));

            // Get contact from xConnect, update and save the facet
            try
            {
                var expandOptions = ExpandOptions();
                var contact = _client.Get(trackerIdentifier, expandOptions);
                return contact;
            }
            catch (Exception ex)
            {
                // todo: log exception

            }

            return null;
        }

        //public IContactPicture Picture => GetFacet<IContactPicture>(C.FacetKeys.Picture);
        //public IContactPreferences Preferences => GetFacet<IContactPreferences>(C.FacetKeys.Preferences);
        public PersonalInformation PersonalInfo => GetFacet<PersonalInformation>(PersonalInformation.DefaultFacetKey);
        public AddressList Addresses => GetFacet<AddressList>(AddressList.DefaultFacetKey);
        public EmailAddressList Emails => GetFacet<EmailAddressList>(EmailAddressList.DefaultFacetKey);
        //public IContactCommunicationProfile CommunicationProfile => GetFacet<IContactCommunicationProfile>(C.FacetKeys.CommunicationProfile);
        public PhoneNumberList PhoneNumbers => GetFacet<PhoneNumberList>(PhoneNumberList.DefaultFacetKey);
        public ConsentInformation ConsentInformation => GetFacet<ConsentInformation>(ConsentInformation.DefaultFacetKey);
        public GigyaFacet Gigya => GetCustomFacet<GigyaFacet>(C.FacetKeys.Gigya);

        public Contact Flush()
        {
            if (Contact == null)
            {
                return null;
            }
            _client.Submit();
            return Contact;
        }

        protected virtual T GetCustomFacet<T>(string facetName) where T : Facet
        {
            var xConnectFacet = A.Tracker.Current.Contact.GetFacet<A.XConnect.Facets.IXConnectFacets>("XConnectFacets");
            if (xConnectFacet == null)
            {
                return null;
            }

            if (xConnectFacet.Facets.TryGetValue(facetName, out Facet facet))
            {
                return facet as T;
            }
            return null;
        }

        protected virtual T GetFacet<T>(string facetName) where T : Facet
        {
            return Contact?.GetFacet<T>(facetName);
        }

        public virtual void SetFacet<T>(T facet, string facetName) where T : Facet
        {
            if (Contact == null)
            {
                return;
            }

            _client.SetFacet<T>(Contact, facetName, facet);
        }
    }
}