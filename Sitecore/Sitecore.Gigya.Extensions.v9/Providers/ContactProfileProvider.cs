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
        private Contact _contact;

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

            var identifier = A.Tracker.Current.Contact.Identifiers.FirstOrDefault(i => i.Source == C.IdentifierSource);
            if (identifier == null)
            {
                // unable to get a contact if we don't have a known identifier
                return null;
            }

            var contactIdentifier = new IdentifiedContactReference(C.IdentifierSource, identifier.Identifier);
            var contact = _client.Get<Contact>(contactIdentifier, new ContactExpandOptions(PersonalInformation.DefaultFacetKey, EmailAddressList.DefaultFacetKey, AddressList.DefaultFacetKey, PhoneNumberList.DefaultFacetKey));
            return contact ?? CreateContact(identifier.Identifier);
        }

        private Contact CreateContact(string identifier)
        {
            var trackerIdentifier = new ContactIdentifier(C.IdentifierSource, identifier, ContactIdentifierType.Known);
            var contact = new Contact(trackerIdentifier);
            _client.AddContact(_contact);
            return contact;
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
            if (Contact == null)
            {
                return;
            }

            _client.SetFacet<T>(Contact, facetName, facet);
        }
    }
}