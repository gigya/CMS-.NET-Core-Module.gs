using Gigya.Module.Core.Connector.Logging;
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
        private readonly Database _database;
        private readonly Logger _logger = Sitecore.Gigya.Module.Logging.LoggerFactory.Instance();
        private ModuleMigrationModel _response;

        public ModuleMigration(string database = "master")
        {
            _database = Sitecore.Configuration.Factory.GetDatabase(database);
        }

        public ModuleMigrationModel DoIt()
        {
            var result = DoMigration();
            _logger.Debug(_response.Messages.ToString());

            return result;
        }

        private ModuleMigrationModel DoMigration()
        {
            _response = new ModuleMigrationModel();

            if (_database == null)
            {
                _response.Messages.AppendLine("[Error]: Couldn't access content database.");
                return _response;
            }

            try
            {
                var globalSettings = GetGlobalSettings();
                if (globalSettings == null)
                {
                    _response.Messages.AppendLine("[Error]: Couldn't find global settings item in Sitecore.");
                    return _response;
                }

                var dataCenter = GetDataCenter();
                MapSettings(globalSettings, dataCenter);
                _response.Messages.AppendLine("Migrated global settings.");

                MapXdbPersonalFacet();
                // create gigya facet mappings for d_terms, d_subscribe ????

                var coreDb = Factory.GetDatabase("core");
                var userTemplate = coreDb.GetItem(Constants.Ids.UserTemplateId);
                if (userTemplate == null)
                {
                    _response.Messages.AppendLine(string.Format("[Error]: Couldn't find user template with id of {0} so didn't add any fields to the default user template.", Constants.Ids.UserTemplateId));
                }

                MapMembershipFields(userTemplate);

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

                _response.Success = true;
                _response.Messages.AppendLine();
                _response.Messages.AppendLine("Migration completed successfully. Updated Sitecore items will need to be published.");
                _response.Messages.AppendLine("Please review the user profile field types in the core db at /sitecore/templates/System/Security/User.");

                _response.Messages.AppendLine();
                _response.Messages.AppendLine("IMPORTANT - Comment out all profile properties (system.web/profile/properties) in web.config except \"SC_UserData\".");
            }
            catch (Exception e)
            {
                _response.Messages.AppendLine(e.ToString());
            }

            return _response;
        }

        private void MapXdbPersonalFacet()
        {
            var xDbPersonalMapping = _database.GetItem(Constants.Ids.xDbPersonalMapping);
            if (xDbPersonalMapping == null)
            {
                _response.Messages.AppendLine("[Error]: Couldn't find personal facet settings.");
                return;
            }

            xDbPersonalMapping.Editing.BeginEdit();
            xDbPersonalMapping.Fields[Constants.Fields.PersonalFacet.FirstName].Value = "profile.firstName";
            xDbPersonalMapping.Fields[Constants.Fields.PersonalFacet.Surname].Value = "profile.lastName";
            xDbPersonalMapping.Fields[Constants.Fields.PersonalFacet.Gender].Value = "profile.gender";
            xDbPersonalMapping.Editing.EndEdit();

            _response.Messages.AppendLine("Migrated personal facet settings.");
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

        private void MapMembershipFields(Item userTemplate)
        {
            var mappingFolder = _database.GetItem(Constants.Ids.MembershipMapping);
            if (mappingFolder == null)
            {
                _response.Messages.AppendLine("[Error]: Couldn't find membership settings.");
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
                            _response.Messages.AppendLine(string.Format("Membership mapping for {0} and profile.{1} already exists.", settings.Name, propertyName));
                            continue;
                        }

                        var mapping = mappingFolder.Add(settings.Name, new TemplateID(Constants.Ids.MappingFieldTemplate));

                        mapping.Editing.BeginEdit();
                        mapping.Fields[Constants.Fields.MappingFields.GigyaProperty].Value = string.Concat("profile.", propertyName);
                        mapping.Fields[Constants.Fields.MappingFields.SitecoreProperty].Value = settings.Name;
                        mapping.Editing.EndEdit();

                        _response.Messages.AppendLine(string.Format("Created membership mapping for {0} and {1}.", settings.Name, propertyName));

                        if (userTemplate != null)
                        {
                            var templateSection = userTemplate.Children.FirstOrDefault(i => i.Name == "Data");
                            if (templateSection == null)
                            {
                                continue;
                            }

                            if (templateSection.Children.Any(i => i.Name == settings.Name))
                            {
                                _response.Messages.AppendLine(string.Format("Profile field {0} already exists.", settings.Name));
                                continue;
                            }

                            var newField = templateSection.Add(settings.Name, new TemplateID(Constants.Ids.TemplateField));
                            newField.Editing.BeginEdit();
                            newField.Fields["Type"].Value = "Single-Line Text";
                            newField.Editing.EndEdit();

                            _response.Messages.AppendLine(string.Format("Created profile field {0} with type of Single-Line Text.", settings.Name));
                        }
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
