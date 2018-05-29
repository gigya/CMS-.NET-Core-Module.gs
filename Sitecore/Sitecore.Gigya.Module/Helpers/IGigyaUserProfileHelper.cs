using System.Collections.Generic;
using Sitecore.Data.Items;

namespace Sitecore.Gigya.Module.Helpers
{
    public interface IGigyaUserProfileHelper
    {
        List<Item> GetDefaultUserProperties();
        IEnumerable<Item> GetProfileProperties(Item profile);
        Item GetSelectedProfile(Item settings);
    }
}