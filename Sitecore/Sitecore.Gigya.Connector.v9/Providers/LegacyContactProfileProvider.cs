using Sitecore.Analytics;
using Sitecore.Analytics.Model.Entities;
using Sitecore.Analytics.Model.Framework;
using Sitecore.Analytics.Tracking;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Gigya.Extensions.Abstractions.Analytics.Facets;
using Sitecore.Gigya.Extensions.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Linq;

using C = Sitecore.Gigya.Extensions.Abstractions.Analytics.Constants;

namespace Sitecore.Gigya.Connector.Providers
{
    public class LegacyContactProfileProvider : ILegacyContactProfileProvider
    {
        private readonly ContactManager _contactManager;
        private Contact _contact;

        public LegacyContactProfileProvider()
        {
            _contactManager = (ContactManager)Factory.CreateObject("tracking/contactManager", false);
        }

        public IEnumerable<IBehaviorProfileContext> BehaviorProfiles => this.Contact?.BehaviorProfiles.Profiles ?? Enumerable.Empty<IBehaviorProfileContext>();

        public Contact Contact
        {
            get
            {
                if (!Tracker.IsActive)
                {
                    return null;
                }

                if (_contact == null)
                {
                    _contact = Tracker.Current.Contact;
                }

                return _contact ?? (_contact = _contactManager?.CreateContact(ID.NewID));
            }
        }

        public IContactPicture Picture => GetFacet<IContactPicture>(C.FacetKeys.Picture);
        public IContactPreferences Preferences => GetFacet<IContactPreferences>(C.FacetKeys.Preferences);
        public IContactPersonalInfo PersonalInfo => GetFacet<IContactPersonalInfo>(C.FacetKeys.Personal);
        public IContactAddresses Addresses => GetFacet<IContactAddresses>(C.FacetKeys.Addresses);
        public IContactEmailAddresses Emails => GetFacet<IContactEmailAddresses>(C.FacetKeys.Emails);
        public IContactCommunicationProfile CommunicationProfile => GetFacet<IContactCommunicationProfile>(C.FacetKeys.CommunicationProfile);
        public IContactPhoneNumbers PhoneNumbers => GetFacet<IContactPhoneNumbers>(C.FacetKeys.PhoneNumbers);

        protected virtual T GetFacet<T>(string facetName) where T : class, IFacet
        {
            return Contact?.GetFacet<T>(facetName);
        }
    }
}