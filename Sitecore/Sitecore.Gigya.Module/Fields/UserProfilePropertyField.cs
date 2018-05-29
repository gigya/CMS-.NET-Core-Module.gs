using Sitecore.Data.Items;
using Sitecore.Gigya.Module.Helpers;
using Sitecore.SecurityModel;
using Sitecore.Shell.Applications.ContentEditor;
using System.Collections.Generic;
using System.Linq;

namespace Sitecore.Gigya.Module.Fields
{
    public class UserProfilePropertyField : ValueLookupEx
    {
        private readonly GigyaUserProfileHelper _profileHelper = new GigyaUserProfileHelper();

        protected override Item[] GetItems(Item current)
        {
            using (new SecurityDisabler())
            {
                var userProperties = _profileHelper.GetDefaultUserProperties();

                // find nearest parent that has a template of Gigya Settings
                var settings = GetSettingsParent(current);
                if (settings == null)
                {
                    return userProperties.ToArray();
                }

                var profile = _profileHelper.GetSelectedProfile(settings);
                if (profile == null)
                {
                    return userProperties.ToArray();
                }

                var profileTemplate = profile.Template;
                var profileProperties = _profileHelper.GetProfileProperties(profileTemplate);
                userProperties.AddRange(profileProperties);
                
                return userProperties.ToArray();
            }
        }

        private Item GetSettingsParent(Item current)
        {
            if (current == null || current.ID.IsNull)
            {
                return null;
            }

            if (current.TemplateID == Constants.Templates.GigyaSettings)
            {
                return current;
            }

            return GetSettingsParent(current.Parent);
        }
    }
}