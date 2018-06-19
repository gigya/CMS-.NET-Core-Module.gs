using System.Collections.Generic;
using Sitecore.Analytics.Model.Entities;
using Sitecore.Analytics.Model.Framework;
using Sitecore.Analytics.Tracking;
using Sitecore.Gigya.Extensions.Abstractions.Analytics.Facets;
using Sitecore.Gigya.Extensions.Analytics.Facets;

namespace Sitecore.Gigya.Extensions.Providers
{
    public interface IContactProfileProvider
    {
        IContactAddresses Addresses { get; }
        IEnumerable<IBehaviorProfileContext> BehaviorProfiles { get; }
        IContactCommunicationProfile CommunicationProfile { get; }
        Contact Contact { get; }
        IContactEmailAddresses Emails { get; }
        IGigyaFacet Gigya { get; }
        Sitecore.Analytics.Tracking.KeyBehaviorCache KeyBehaviorCache { get; }
        IContactPersonalInfo PersonalInfo { get; }
        IContactPhoneNumbers PhoneNumbers { get; }
        IContactPicture Picture { get; }
        IContactPreferences Preferences { get; }

        Contact Flush();
    }
}