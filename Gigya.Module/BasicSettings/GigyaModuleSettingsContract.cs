using System;
using System.Linq;
using System.Runtime.Serialization;
using Gigya.Module.Configuration;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.SiteSettings;
using Gigya.Module.Data;
using Gigya.Module.Connector.Encryption;
using System.Text;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Security.Claims;
using Newtonsoft.Json;
using Gigya.Module.Connector.Models;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;
using Gigya.Module.Connector.Helpers;

namespace Gigya.Module.BasicSettings
{
    [DataContract]
    public class GigyaModuleSettingsContract : IGigyaSettingsDataContract
    {
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
                var settings = siteSettingsAndGlobal.FirstOrDefault(i => i.SiteId == id) ?? siteSettingsAndGlobal.FirstOrDefault() ?? new GigyaModuleSettings { SiteId = id, EnableRaas = true };

                // map settigs to this
                this.ApiKey = settings.ApiKey;
                this.ApplicationKey = settings.ApplicationKey;

                // check if user can view application secret
                var identity = ClaimsManager.GetCurrentIdentity();
                this.CanViewApplicationSecret = identity.IsAuthenticated && Connector.Admin.Roles.HasRole(identity);

                if (CanViewApplicationSecret && !string.IsNullOrEmpty(settings.ApplicationSecret) && Encryptor.IsConfigured)
                {
                    var key = Encryptor.Decrypt(settings.ApplicationSecret);
                    if (!string.IsNullOrEmpty(key))
                    {
                        this.ApplicationSecretMasked = MaskedKey(key);
                    }
                }

                this.LoadedField = "Loaded";
                this.DataCenter = settings.DataCenter;
                this.DataCenterOther = settings.DataCenter;
                this.DebugMode = settings.DebugMode;
                this.EnableRaas = settings.EnableRaas;
                this.GlobalParameters = settings.GlobalParameters;
                this.Language = settings.Language;
                this.LanguageOther = settings.Language;
                this.LanguageFallback = settings.LanguageFallback;
                this.SessionTimeout = settings.SessionTimeout;

                var mappingFields = !string.IsNullOrEmpty(settings.MappingFields) ? JsonConvert.DeserializeObject<List<MappingField>>(settings.MappingFields) : new List<MappingField>();
                AddMappingField(Constants.GigyaFields.FirstName, Constants.SitefinityFields.FirstName, ref mappingFields, true);
                AddMappingField(Constants.GigyaFields.LastName, Constants.SitefinityFields.LastName, ref mappingFields, true);
                AddMappingField(Constants.GigyaFields.Email, Constants.SitefinityFields.Email, ref mappingFields, true);
                this.MappingFields = JsonConvert.SerializeObject(mappingFields, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });

                this.LogoutUrl = settings.LogoutUrl;
                this.RedirectUrl = settings.RedirectUrl;
                this.SiteId = settings.SiteId;
            }
        }

        private void AddMappingField(string gigyaFieldName, string defaultSitefinityFieldName, ref List<MappingField> fields, bool required)
        {
            var field = fields.FirstOrDefault(i => i.GigyaFieldName == gigyaFieldName);
            if (field != null)
            {
                field.Required = required;
            }
            else
            {
                fields.Add(new MappingField
                {
                    GigyaFieldName = gigyaFieldName,
                    SitefinityFieldName = defaultSitefinityFieldName,
                    Required = required
                });
            }
        }

        private string MaskedKey(string key)
        {
            var masked = new StringBuilder(string.Concat(Enumerable.Repeat("*", key.Length)));
            masked = masked.Remove(0, 2);
            masked = masked.Remove(masked.Length - 1, 1);
            masked.Insert(0, key.Substring(0, 2));
            masked.Append(key.Substring(key.Length - 1));

            return masked.ToString();
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
                var settings = context.Settings.FirstOrDefault(i => i.SiteId == id) ?? new GigyaModuleSettings { SiteId = id };

                // update all fields
                settings.ApiKey = this.ApiKey;
                settings.DebugMode = this.DebugMode;
                settings.ApplicationKey = this.ApplicationKey;
                settings.DataCenter = !string.IsNullOrEmpty(this.DataCenter) ? this.DataCenter : this.DataCenterOther;
                settings.EnableRaas = this.EnableRaas;
                settings.GlobalParameters = this.GlobalParameters;
                settings.Language = !string.IsNullOrEmpty(this.Language) ? this.Language : this.LanguageOther;
                settings.LanguageFallback = this.LanguageFallback;
                settings.MappingFields = this.MappingFields;
                settings.RedirectUrl = this.RedirectUrl;
                settings.LogoutUrl = this.LogoutUrl;
                settings.SessionTimeout = this.SessionTimeout;

                // application secret that we will use to validate the settings - store this in a separate var as it's unencrypted
                string plainTextApplicationSecret = string.Empty;

                // check if user can view application secret
                if (!string.IsNullOrEmpty(ApplicationSecret))
                {
                    plainTextApplicationSecret = ApplicationSecret;
                    var identity = ClaimsManager.GetCurrentIdentity();
                    var canViewApplicationSecret = identity.IsAuthenticated && Gigya.Module.Connector.Admin.Roles.HasRole(identity);
                    if (canViewApplicationSecret)
                    {
                        if (!Encryptor.IsConfigured)
                        {
                            throw new ArgumentException("Encryption key not specified. Refer to installation guide.");
                        }
                        settings.ApplicationSecret = Encryptor.Encrypt(this.ApplicationSecret);
                    }
                }

                if (string.IsNullOrEmpty(plainTextApplicationSecret) && Encryptor.IsConfigured && !string.IsNullOrEmpty(settings.ApplicationSecret))
                {
                    plainTextApplicationSecret = Encryptor.Decrypt(settings.ApplicationSecret);
                }

                Validate(settings, plainTextApplicationSecret);
                context.Add(settings);
                context.SaveChanges();
            }
        }

        private void Validate(GigyaModuleSettings settings, string applicationSecret)
        {
            if (string.IsNullOrEmpty(settings.ApiKey))
            {
                throw new ArgumentException("API key is required");
            }

            if (string.IsNullOrEmpty(settings.ApplicationKey))
            {
                throw new ArgumentException("API key is required");
            }

            if (string.IsNullOrEmpty(settings.DataCenter))
            {
                throw new ArgumentException("DataCenter is required");
            }

            if (string.IsNullOrEmpty(settings.Language))
            {
                throw new ArgumentException("Language is required");
            }

            if (!string.IsNullOrEmpty(settings.GlobalParameters))
            {
                try
                {
                    JsonConvert.DeserializeObject<dynamic>(settings.GlobalParameters);
                }
                catch
                {
                    throw new ArgumentException("Couldn't deserialize global parameters. Check it's valid JSON.");
                }
            }

            // verify settings are correct
            var testResponse = GigyaApiHelper.VerifySettings(settings, applicationSecret);
            if (testResponse.GetErrorCode() != 0)
            {
                throw new InvalidOperationException("Error: " + testResponse.GetErrorMessage());
            }
        }

        #endregion
    }
}