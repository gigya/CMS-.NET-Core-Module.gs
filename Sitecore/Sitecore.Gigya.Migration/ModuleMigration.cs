using GigyaSecurityProvider.Utils;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Gigya.Module.Encryption;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace Sitecore.Gigya.Migration
{
    public class ModuleMigration
    {
        private Database _database;

        public ModuleMigration(string database = "master")
        {
            _database = Sitecore.Configuration.Factory.GetDatabase(database);
        }

        public ModuleMigrationModel DoIt()
        {
            var response = new ModuleMigrationModel();

            if (_database == null)
            {
                response.Messages.AppendLine("Couldn't access content database");
                return response;
            }

            try
            {
                var globalSettings = GetGlobalSettings();
                if (globalSettings == null)
                {
                    response.Messages.AppendLine("Couldn't find global settings item in Sitecore.");
                    return response;
                }

                var dataCenter = GetDataCenter();
                MapSettings(globalSettings, dataCenter);
                response.Messages.AppendLine("Migrated global settings");

                MapXdbPersonalFacet(ref response);
                // create gigya facet mappings for d_terms, d_subscribe ????

                MapMembershipFields(ref response);

                // create profile fields?

                // create settings for each site that has a dif application key
                var siteInfoList = Factory.GetSiteInfoList();
                foreach (var siteName in GigyaSettings.APIKeyCollection.AllKeys)
                {
                    var site = siteInfoList.FirstOrDefault(i => i.Name == siteName);
                    if (site == null)
                    {
                        continue;
                    }

                    var apiKey = GigyaSettings.APIKeyCollection[siteName];
                    if (!string.IsNullOrEmpty(apiKey))
                    {
                        var siteRoot = _database.GetItem(site.RootPath +  site.StartItem);
                        var settings = siteRoot.Children.FirstOrDefault(i => i.TemplateID == Constants.Ids.GigyaSettings) ?? globalSettings.CopyTo(siteRoot, "Gigya Settings", ID.NewID, false);

                        MapSettings(settings, dataCenter);

                        settings.Editing.BeginEdit();
                        var settingsParent = ((MultilistField)settings.Fields[Constants.Fields.Parent]);
                        settingsParent.Value = string.Empty;
                        settingsParent.Add(globalSettings.ID.ToString());
                        settings.Editing.EndEdit();
                    }
                }

                response.Success = true;
                response.Messages.AppendLine("Migration completed successfully. Updated Sitecore items will need to be published.");
            }
            catch (Exception e)
            {
                response.Messages.AppendLine(e.ToString());
            }

            return response;
        }

        private void MapXdbPersonalFacet(ref ModuleMigrationModel response)
        {
            var xDbPersonalMapping = _database.GetItem(Constants.Ids.xDbPersonalMapping);
            if (xDbPersonalMapping == null)
            {
                response.Messages.AppendLine("Couldn't find personal facet settings");
                return;
            }

            xDbPersonalMapping.Editing.BeginEdit();
            xDbPersonalMapping.Fields[Constants.Fields.PersonalFacet.FirstName].Value = "profile.firstName";
            xDbPersonalMapping.Fields[Constants.Fields.PersonalFacet.Surname].Value = "profile.lastName";
            xDbPersonalMapping.Fields[Constants.Fields.PersonalFacet.Gender].Value = "profile.gender";
            xDbPersonalMapping.Editing.EndEdit();

            response.Messages.AppendLine("Migrated personal facet settings");
        }

        private string GetDataCenter()
        {
            var dataCenterFolder = _database.GetItem(Constants.Ids.DataCenterFolder);
            if (dataCenterFolder == null)
            {
                return null;
            }

            var oldDataCenter = GigyaSettings.GigyaDataCenter;
            return dataCenterFolder.Children.FirstOrDefault(i => i.Fields[Constants.Fields.DataCenter].Value == oldDataCenter)?.ID.ToString();
        }

        private void MapSettings(Item globalSettings, string dataCenter)
        {
            globalSettings.Editing.BeginEdit();

            globalSettings.Fields[Constants.Fields.ApiKey].Value = GigyaSettings.APIKeyCollection["default"];
            globalSettings.Fields[Constants.Fields.ApplicationKey].Value = GigyaSettings.GetGigyaApplicationUserKeyForSite();

            var encrypted = SitecoreEncryptionService.Instance.Encrypt(GigyaSettings.GetGigyaApplicationSecretKeyForSite());
            globalSettings.Fields[Constants.Fields.ApplicationSecret].Value = string.Concat(Module.Constants.EncryptionPrefix, encrypted);

            if (!string.IsNullOrEmpty(dataCenter))
            {
                globalSettings.Fields[Constants.Fields.DataCenter].Value = dataCenter;
            }

            var providers = GetEnabledProviders();
            if (!string.IsNullOrEmpty(providers))
            {
                globalSettings.Fields[Constants.Fields.GlobalParameters].Value = string.Format("{{ \"enabledProviders\":\"{0}\"}}", providers);
            }

            globalSettings.Editing.EndEdit();
        }

        private void MapMembershipFields(ref ModuleMigrationModel response)
        {
            var mappingFolder = _database.GetItem(Constants.Ids.MembershipMapping);
            if (mappingFolder == null)
            {
                response.Messages.AppendLine("Couldn't find membership settings");
                return;
            }

            var section = (ProfileSection)WebConfigurationManager.OpenWebConfiguration("/aspnet").GetSection("system.web/profile");
            foreach (ProfilePropertySettings settings in section.PropertySettings)
            {
                string customProviderData = settings.CustomProviderData.Trim();
                if (IsGigyaProperty(customProviderData))
                {
                    string propertyName;
                    string propertyType = settings.Type;

                    string[] splitStrings = customProviderData.Trim().Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                    if (splitStrings.Length >= 2 && !string.IsNullOrEmpty(propertyName = splitStrings[1]))
                    {
                        propertyName = propertyName.Trim();

                        if (mappingFolder.Children.Any(i => i.Name == settings.Name))
                        {
                            continue;
                        }

                        var mapping = mappingFolder.Add(settings.Name, new TemplateID(Constants.Ids.MappingFieldTemplate));

                        mapping.Editing.BeginEdit();
                        mapping.Fields[Constants.Fields.MappingFields.GigyaProperty].Value = string.Concat("profile.", propertyName);
                        mapping.Fields[Constants.Fields.MappingFields.SitecoreProperty].Value = settings.Name;
                        mapping.Editing.EndEdit();

                        response.Messages.AppendLine(string.Format("Created memebership mapping for {0} and {1}", settings.Name, propertyName));
                    }
                }
            }
        }

        private bool IsGigyaProperty(string customProviderData)
        {
            return !string.IsNullOrEmpty(customProviderData) && customProviderData.StartsWith(string.Format("{0}|", GigyaSettings.GigyaUserDomainName));
        }

        private string GetEnabledProviders()
        {
            Item providerSettingsItem = _database.GetItem(Constants.Ids.ShareProviderSettingsItemId);
            if (providerSettingsItem == null)
            {
                return string.Empty;
            }
            return providerSettingsItem[Constants.Fields.PluginConfigItemFieldNames.ShareProviders];
        }

        private Item GetGlobalSettings()
        {
            var globalSettings = _database.GetItem(Constants.Ids.GlobalSettings);
            if (globalSettings != null)
            {
                return globalSettings;
            }
            return null;
        }
    }

    public class ModuleMigrationModel
    {
        public bool Success { get; set; }
        public StringBuilder Messages { get; set; } = new StringBuilder();
    }
}
