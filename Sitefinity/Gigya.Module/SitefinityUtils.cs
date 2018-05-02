using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using Telerik.Sitefinity.Security;

namespace Gigya.Module
{
    public class SitefinityUtils
    {
        public static string GetPageUrl(Guid? pageId)
        {
            if (!pageId.HasValue)
            {
                return null;
            }

            var node = SiteMap.Provider.FindSiteMapNodeFromKey(pageId.Value.ToString());
            if (node != null)
            {
                return VirtualPathUtility.ToAbsolute(node.Url);
            }

            return null;
        }

        public static List<KeyValuePair<string, string>> GetProfileProperties()
        {
            var profileManager = UserProfileManager.GetManager();
            var profile = profileManager.GetUserProfiles().FirstOrDefault();

            if (profile != null)
            {
                var profileProperties = TypeDescriptor.GetProperties(profile);
                List<KeyValuePair<string, string>> propertyNames = new List<KeyValuePair<string, string>>();

                foreach (PropertyDescriptor property in profileProperties)
                {
                    if (!Gigya.Module.Constants.Profiles.BuiltInProfileProperties.ContainsKey(property.Name))
                    {
                        propertyNames.Add(new KeyValuePair<string, string>(property.Name, property.DisplayName));
                    }
                }

                if (propertyNames.Any())
                {
                    return propertyNames;
                }
            }

            return null;
        }
    }
}