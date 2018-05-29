using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Gigya.Module.Helpers
{
    public class GigyaUserProfileHelper : IGigyaUserProfileHelper
    {
        public virtual Item GetSelectedProfile(Item settings)
        {
            var profileId = ((Sitecore.Data.Fields.MultilistField)settings.Fields[Constants.Fields.Profile]).Items.FirstOrDefault();
            if (string.IsNullOrEmpty(profileId))
            {
                return GetDefaultProfileId();
            }

            Data.Database coreDb = Configuration.Factory.GetDatabase("core");
            return coreDb.GetItem(new Data.ID(profileId));
        }

        protected virtual Item GetDefaultProfileId()
        {
            Data.Database coreDb = Configuration.Factory.GetDatabase("core");
            return coreDb.GetItem(Constants.Templates.SitecoreDefaultUserProfile);
        }

        /// <summary>
        /// Gets the hard coded properties e.g. Full Name, Comment, Email
        /// </summary>
        /// <returns></returns>
        public virtual List<Item> GetDefaultUserProperties()
        {
            var userPropertiesFolder = Sitecore.Context.ContentDatabase.GetItem(Constants.Ids.UserProperties);
            if (userPropertiesFolder == null)
            {
                return new List<Item>();
            }

            var childTemplateFields = userPropertiesFolder.Children.Where(i => i.TemplateID == TemplateIDs.TemplateField).ToList();
            return childTemplateFields;
        }

        public virtual IEnumerable<Item> GetProfileProperties(Item profile)
        {
            var properties = new List<Item>();
            AddChildProperties(profile, properties);
            return properties;
        }

        private void AddChildProperties(Item current, List<Item> properties)
        {
            if (current == null || !current.Children.Any())
            {
                return;
            }

            var childTemplateFields = current.Children.Where(i => i.TemplateID == TemplateIDs.TemplateField).ToList();
            if (childTemplateFields.Any())
            {
                properties.AddRange(childTemplateFields);
            }

            foreach (Item childItem in current.Children)
            {
                AddChildProperties(childItem, properties);
            }
        }
    }
}