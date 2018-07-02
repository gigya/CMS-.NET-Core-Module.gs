using System.Collections.Generic;
using Sitecore.Analytics.Model.Entities;
using Sitecore.Analytics.Model.Framework;
using Sitecore.Analytics.Tracking;
using Sitecore.Gigya.Extensions.Abstractions.Analytics.Facets;

namespace Sitecore.Gigya.Connector.Providers
{
    public interface ILegacyContactProfileProvider
    {
        IContactAddresses Addresses { get; }
        IEnumerable<IBehaviorProfileContext> BehaviorProfiles { get; }
        IContactCommunicationProfile CommunicationProfile { get; }
        Contact Contact { get; }
        IContactEmailAddresses Emails { get; }
        IContactPersonalInfo PersonalInfo { get; }
        IContactPhoneNumbers PhoneNumbers { get; }
        IContactPicture Picture { get; }
        IContactPreferences Preferences { get; }
    }
}