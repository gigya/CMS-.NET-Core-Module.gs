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

namespace Sitecore.Gigya.Extensions.Providers
{
    public class ContactProfileProvider : IContactProfileProvider
    {
        private XConnectClient _client;
        //private readonly ContactManager _contactManager;
        private Contact _contact;

        public ContactProfileProvider()
        {
            _client = SitecoreXConnectClientConfiguration.GetClient();
        }

        public Contact Contact
        {
            get
            {
                if (!A.Tracker.IsActive)
                {
                    return null;
                }

                if (_contact == null && A.Tracker.Current.Contact != null)
                {
                    var trackerIdentifier = new IdentifiedContactReference(C.IdentifierSource, A.Tracker.Current.Contact.ContactId.ToString("N"));
                    _contact = _client.Get<Contact>(trackerIdentifier, new ContactExpandOptions(PersonalInformation.DefaultFacetKey, EmailAddressList.DefaultFacetKey, AddressList.DefaultFacetKey, PhoneNumberList.DefaultFacetKey));
                }

                if (_contact == null)
                {
                    var trackerIdentifier = new ContactIdentifier(C.IdentifierSource, Guid.NewGuid().ToString("N"), ContactIdentifierType.Known);
                    _contact = new Contact();
                    _client.AddContact(_contact);
                }

                return _contact;
            }
        }

        //public IContactPicture Picture => GetFacet<IContactPicture>(C.FacetKeys.Picture);
        //public IContactPreferences Preferences => GetFacet<IContactPreferences>(C.FacetKeys.Preferences);
        public PersonalInformation PersonalInfo => GetFacet<PersonalInformation>(PersonalInformation.DefaultFacetKey);
        public AddressList Addresses => GetFacet<AddressList>(AddressList.DefaultFacetKey);
        public EmailAddressList Emails => GetFacet<EmailAddressList>(EmailAddressList.DefaultFacetKey);
        //public IContactCommunicationProfile CommunicationProfile => GetFacet<IContactCommunicationProfile>(C.FacetKeys.CommunicationProfile);
        public PhoneNumberList PhoneNumbers => GetFacet<PhoneNumberList>(PhoneNumberList.DefaultFacetKey);
        public ConsentInformation ConsentInformation => GetFacet<ConsentInformation>(ConsentInformation.DefaultFacetKey);
        //public IGigyaFacet Gigya => GetFacet<IGigyaFacet>(C.FacetKeys.Gigya);

        public Contact Flush()
        {
            if (Contact == null)
            {
                return null;
            }
            _client.Submit();
            return Contact;
        }

        protected virtual T GetFacet<T>(string facetName) where T : Facet
        {
            return Contact?.GetFacet<T>(facetName);
        }

        public virtual void SetFacet<T>(T facet, string facetName) where T : Facet
        {
            _client.SetFacet<T>(Contact, facetName, facet);
        }
    }
}