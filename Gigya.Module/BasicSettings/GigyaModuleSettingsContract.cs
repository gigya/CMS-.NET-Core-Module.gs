using System;
using System.Linq;
using System.Runtime.Serialization;
using Gigya.Module.Configuration;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.SiteSettings;
using Gigya.Module.Data;
using System.Text;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Security.Claims;
using Newtonsoft.Json;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;
using Gigya.Module.Connector.Helpers;
using Gigya.Module.Core.Data;
using Gigya.Module.Core.Connector.Encryption;
using Gigya.Module.Core.Connector.Helpers;
using Gigya.Module.Core.Connector.Models;
using Gigya.Module.Core.Connector.Logging;
using Gigya.Module.Connector.Logging;
using Gigya.Module.Core.Connector.Enums;
using Telerik.Sitefinity.Security;
using System.ComponentModel;

namespace Gigya.Module.BasicSettings
{
    [DataContract]
    public class GigyaModuleSettingsContract : IGigyaSettingsDataContract
    {
        private Logger _logger;
        private Logger Logger
        {
            get
            {
                if (_logger == null)
                {
                    _logger = new Logger(new SitefinityLogger());
                }
                return _logger;
            }
        }

        private Connector.Helpers.GigyaSettingsHelper _settingsHelper;
        private Connector.Helpers.GigyaSettingsHelper SettingsHelper
        {
            get
            {
                if (_settingsHelper == null)
                {
                    _settingsHelper = new Connector.Helpers.GigyaSettingsHelper();
                }
                return _settingsHelper;
            }
        }

        #region Data Members

        [DataMember]
        public Guid SiteId { get; set; }

        [DataMember]
        public string ApiKey { get; set; }

        [DataMember]
        public string ApplicationKey { get; set; }

        [DataMember]
        public string ApplicationSecret { get; set; }

        [DataMember]
        public string ApplicationSecretMasked { get; set; }

        [DataMember]
        public string Language { get; set; }

        [DataMember]
        public string LanguageOther { get; set; }

        [DataMember]
        public string LanguageFallback { get; set; }

        [DataMember]
        public bool DebugMode { get; set; }

        [DataMember]
        public string DataCenter { get; set; }

        [DataMember]
        public string DataCenterOther { get; set; }

        [DataMember]
        public bool EnableRaas { get; set; }

        [DataMember]
        public string RedirectUrl { get; set; }

        [DataMember]
        public string LogoutUrl { get; set; }

        [DataMember]
        public string MappingFields { get; set; }

        [DataMember]
        public string GlobalParameters { get; set; }

        [DataMember]
        public bool CanViewApplicationSecret { get; set; }

        [DataMember]
        public int SessionTimeout { get; set; }

        [DataMember]
        public GigyaSessionProvider SessionProvider { get; set; }

        [DataMember]
        public string ProfileProperties { get; set; }

        /// <summary>
        /// This field is required so we can poll on the client and detect if the service has responded.
        /// </summary>
        [DataMember]
        public string LoadedField { get; set; }

        #endregion

        #region ISettingsDataContract Implementation

        public void LoadDefaults(bool forEdit = false)
        {
            Load(Guid.Empty);
        }

        /// <summary>
        /// Get values from module config and load them into data members
        /// </summary>
        /// <param name="forEdit"></param>
        public void Load(Guid id)
        {
            using (var context = GigyaContext.Get())
            {
                // get settings for site or global settings or new settings
                var siteSettingsAndGlobal = context.Settings.Where(i => i.SiteId == id || i.SiteId == Guid.Empty).ToList();
                var settings = siteSettingsAndGlobal.FirstOrDefault(i => i.SiteId == id) ?? siteSettingsAndGlobal.FirstOrDefault() ?? new GigyaSitefinityModuleSettings { SiteId = id, EnableRaas = true };

                // map settigs to this
                this.ApiKey = settings.ApiKey;
                this.ApplicationKey = settings.ApplicationKey;

                // check if user can view application secret
                var identity = ClaimsManager.GetCurrentIdentity();
                this.CanViewApplicationSecret = identity.IsAuthenticated && Connector.Admin.Roles.HasRole(identity);

                if (CanViewApplicationSecret && !string.IsNullOrEmpty(settings.ApplicationSecret) && Encryptor.IsConfigured)
                {
                    var key = TryDecryptApplicationSecret(settings.ApplicationSecret, false);
                    if (!string.IsNullOrEmpty(key))
                    {
                        this.ApplicationSecretMasked = StringHelper.MaskInput(key, "*", 2, 2);
                    }
                }

                this.LoadedField = "Loaded";
                settings.DataCenter = Core.Connector.Helpers.GigyaSettingsHelper.MapOldDataCenter(settings.DataCenter);

                this.DataCenter = string.IsNullOrEmpty(settings.DataCenter) || Core.Constants.DataCenter.DataCenters.Contains(settings.DataCenter) ? settings.DataCenter : string.Empty;
                this.DataCenterOther = settings.DataCenter;
                this.DebugMode = settings.DebugMode;
                this.EnableRaas = settings.EnableRaas;
                this.GlobalParameters = settings.GlobalParameters;
                this.Language = settings.Language;
                this.LanguageOther = settings.Language;
                this.LanguageFallback = settings.LanguageFallback;
                this.SessionTimeout = settings.SessionTimeout;
                this.SessionProvider = settings.SessionProvider;

                var mappingFields = !string.IsNullOrEmpty(settings.MappingFields) ? JsonConvert.DeserializeObject<List<MappingField>>(settings.MappingFields) : new List<MappingField>();
                AddMappingField(Constants.GigyaFields.UserId, Constants.SitefinityFields.UserId, ref mappingFields, true);
                AddMappingField(Constants.GigyaFields.FirstName, Constants.SitefinityFields.FirstName, ref mappingFields, true);
                AddMappingField(Constants.GigyaFields.LastName, Constants.SitefinityFields.LastName, ref mappingFields, true);
                AddMappingField(Constants.GigyaFields.Email, Constants.SitefinityFields.Email, ref mappingFields, true);
                
                // required fields first
                mappingFields = mappingFields.Where(i => !i.Required || !string.IsNullOrEmpty(i.CmsFieldName)).OrderByDescending(i => i.Required).ThenBy(i => i.CmsFieldName).ToList();

                this.MappingFields = JsonConvert.SerializeObject(mappingFields, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });

                this.LogoutUrl = settings.LogoutUrl;
                this.RedirectUrl = settings.RedirectUrl;
                this.SiteId = settings.SiteId;

                // get a list of available profile properties
                var profileProperties = SitefinityUtils.GetProfileProperties();
                profileProperties.Add(new KeyValuePair<string, string>(Constants.SitefinityFields.UserId, Constants.SitefinityFields.UserId));
                profileProperties.Add(new KeyValuePair<string, string>(Constants.SitefinityFields.Email, Constants.SitefinityFields.Email));
                this.ProfileProperties = JsonConvert.SerializeObject(profileProperties.OrderBy(i => i.Value));
            }
        }

        private void AddMappingField(string gigyaFieldName, string defaultSitefinityFieldName, ref List<MappingField> fields, bool required)
        {
            var field = fields.FirstOrDefault(i => i.CmsFieldName == defaultSitefinityFieldName);
            if (field != null)
            {
                field.Required = required;
                field.CmsFieldName = defaultSitefinityFieldName;
            }
            else
            {
                fields.Add(new MappingField
                {
                    GigyaFieldName = gigyaFieldName,
                    CmsFieldName = defaultSitefinityFieldName,
                    Required = required
                });
            }
        }

        public void SaveDefaults()
        {
            Save(Guid.Empty);
        }

        /// <summary>
        /// Save values from basic settings page
        /// </summary>
        public void Save(Guid id)
        {
            using (var context = GigyaContext.Get())
            {
                // get settings to update
                var settings = context.Settings.FirstOrDefault(i => i.SiteId == id) ?? new GigyaSitefinityModuleSettings { SiteId = id };

                // update all fields
                settings.ApiKey = this.ApiKey.Trim();
                settings.DebugMode = this.DebugMode;
                settings.ApplicationKey = this.ApplicationKey.Trim();
                settings.DataCenter = !string.IsNullOrEmpty(this.DataCenter) ? this.DataCenter : this.DataCenterOther;
                settings.EnableRaas = this.EnableRaas;
                settings.GlobalParameters = this.GlobalParameters;
                settings.Language = !string.IsNullOrEmpty(this.Language) ? this.Language : this.LanguageOther;
                settings.LanguageFallback = this.LanguageFallback;
                settings.MappingFields = this.MappingFields;
                settings.RedirectUrl = this.RedirectUrl;
                settings.LogoutUrl = this.LogoutUrl;
                settings.SessionTimeout = this.SessionTimeout;
                settings.SessionProvider = this.SessionProvider;

                var mappingFields = JsonConvert.DeserializeObject<List<MappingField>>(MappingFields);
                if (mappingFields == null || !mappingFields.Any())
                {
                    throw new ArgumentException(Constants.Errors.UIDFieldRequired);
                }

                // validate that there is a mapping field for UID
                var usernameMappingExists = mappingFields.Any(i => i.GigyaFieldName == Constants.GigyaFields.UserId);
                if (!usernameMappingExists)
                {
                    throw new ArgumentException(Constants.Errors.UIDFieldRequired);
                }

                if (mappingFields.Any(i => string.IsNullOrEmpty(i.GigyaFieldName)))
                {
                    throw new ArgumentException(Constants.Errors.GigyaFieldNameRequired);
                }

                if (mappingFields.Any(i => string.IsNullOrEmpty(i.CmsFieldName)))
                {
                    throw new ArgumentException(Constants.Errors.CmsFieldNameRequired);
                }

                // application secret that we will use to validate the settings - store this in a separate var as it's unencrypted
                string plainTextApplicationSecret = string.Empty;

                // check if user can view application secret
                if (!string.IsNullOrEmpty(ApplicationSecret))
                {
                    plainTextApplicationSecret = ApplicationSecret.Trim();
                    var identity = ClaimsManager.GetCurrentIdentity();
                    var canViewApplicationSecret = identity.IsAuthenticated && Gigya.Module.Connector.Admin.Roles.HasRole(identity);
                    if (canViewApplicationSecret)
                    {
                        if (!Encryptor.IsConfigured)
                        {
                            throw new ArgumentException("Encryption key not specified. Refer to installation guide.");
                        }

                        settings.ApplicationSecret = Encryptor.Encrypt(plainTextApplicationSecret);
                    }
                }

                if (string.IsNullOrEmpty(plainTextApplicationSecret) && Encryptor.IsConfigured && !string.IsNullOrEmpty(settings.ApplicationSecret))
                {
                    plainTextApplicationSecret = TryDecryptApplicationSecret(settings.ApplicationSecret);
                }

                var mappedSettings = Map(settings);
                
                // validate input
                SettingsHelper.Validate(mappedSettings);

                // verify settings are correct
                var apiHelper = new GigyaApiHelper(SettingsHelper, Logger);
                var testResponse = apiHelper.VerifySettings(mappedSettings, plainTextApplicationSecret);
                if (testResponse.GetErrorCode() != 0)
                {
                    var gigyaErrorDetail = testResponse.GetString("errorDetails", string.Empty);
                    var message = string.Concat("Error: ", testResponse.GetErrorMessage());
                    if (!string.IsNullOrEmpty(gigyaErrorDetail))
                    {
                        message = string.Concat(message, ". ", gigyaErrorDetail);
                    }

                    throw new InvalidOperationException(message);
                }
                
                context.Add(settings);
                context.SaveChanges();
            }
        }

        private string TryDecryptApplicationSecret(string secret, bool throwOnException = true)
        {
            try
            {
                return Encryptor.Decrypt(secret);
            }
            catch (Exception e)
            {
                Logger.Error("Couldn't decrypt application secret.", e);
                if (throwOnException)
                {
                    throw new ArgumentException("Couldn't decrypt application secret. Please enter it again.");
                }
            }
            return null;
        }

        private IGigyaModuleSettings Map(GigyaSitefinityModuleSettings settings)
        {
            var model = new GigyaModuleSettings
            {
                ApiKey = settings.ApiKey,
                ApplicationSecret = settings.ApplicationSecret,
                ApplicationKey = settings.ApplicationKey,
                DataCenter = settings.DataCenter,
                DebugMode = settings.DebugMode,
                EnableRaas = settings.EnableRaas,
                GlobalParameters = settings.GlobalParameters,
                Id = settings.SiteId,
                Language = settings.Language,
                LanguageFallback = settings.LanguageFallback,
                LogoutUrl = settings.LogoutUrl,
                MappingFields = settings.MappingFields,
                RedirectUrl = settings.RedirectUrl,
                SessionTimeout = settings.SessionTimeout,
                SessionProvider = settings.SessionProvider
            };

            return model;
        }

        #endregion
    }
}