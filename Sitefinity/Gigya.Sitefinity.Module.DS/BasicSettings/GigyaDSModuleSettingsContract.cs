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
using Gigya.Sitefinity.Module.DS.Data;
using System.Configuration;
using Gigya.Module.DS.Config;
using Gigya.Sitefinity.Module.DS.Helpers;
using Gigya.Module.DS.Helpers;
using Telerik.Sitefinity.Security;
using System.ComponentModel;
using Gigya.Module;

namespace Gigya.Sitefinity.Module.DS.BasicSettings
{
    [DataContract]
    public class GigyaDSModuleSettingsContract : IGigyaDSSettingsDataContract
    {
        private static readonly bool _validateDsSettings = bool.Parse(ConfigurationManager.AppSettings["Gigya.DS.Validate"] ?? bool.TrueString);

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

        private GigyaSitefinityDsSettingsHelper _settingsHelper;
        private GigyaSitefinityDsSettingsHelper SettingsHelper
        {
            get
            {
                if (_settingsHelper == null)
                {
                    _settingsHelper = new GigyaSitefinityDsSettingsHelper(Logger);
                }
                return _settingsHelper;
            }
        }

        #region Data Members

        [DataMember]
        public Guid SiteId { get; set; }

        [DataMember]
        public GigyaDsMethod Method { get; set; }

        [DataMember]
        public string MappingFields { get; set; }

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
            using (var context = GigyaDSContext.Get())
            {
                // get settings for site or global settings or new settings
                var siteSettingsAndGlobal = context.Settings.Where(i => i.SiteId == id || i.SiteId == Guid.Empty).ToList();
                var settings = siteSettingsAndGlobal.FirstOrDefault(i => i.SiteId == id) ?? siteSettingsAndGlobal.FirstOrDefault() ?? new GigyaSitefinityModuleDsSettings { SiteId = id, Mappings = new List<GigyaSitefinityDsMapping>() };

                // let client know that the data has been retrieved - hack as there is no "loaded" sitefinity client side event
                this.LoadedField = "Loaded";

                var mappingFields = settings.Mappings.Select(Map).OrderBy(i => i.CmsFieldName).ToList();
                this.MappingFields = JsonConvert.SerializeObject(mappingFields, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
                
                this.SiteId = settings.SiteId;
                this.Method = settings.Method;

                // get a list of available profile properties
                this.ProfileProperties = JsonConvert.SerializeObject(SitefinityUtils.GetProfileProperties().OrderBy(i => i.Value));
            }
        }

        private GigyaDsMappingViewModel Map(GigyaSitefinityDsMapping mapping)
        {
            return new GigyaDsMappingViewModel
            {
                CmsFieldName = mapping.CmsName,
                GigyaFieldName = mapping.GigyaName,
                Oid = mapping.Oid
            };
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
            using (var context = GigyaDSContext.Get())
            {
                // get settings to update
                var settings = context.Settings.FirstOrDefault(i => i.SiteId == id) ?? new GigyaSitefinityModuleDsSettings { SiteId = id };
                
                var mappingFields = JsonConvert.DeserializeObject<List<GigyaDsMappingViewModel>>(MappingFields);

                if (mappingFields == null)
                {
                    var error = "Invalid mapping param supplied. Please check your mappings.";
                    Logger.Error(error);
                    throw new ArgumentException(error);
                }

                if (mappingFields.Any(i => string.IsNullOrEmpty(i.CmsFieldName)))
                {
                    var error = "Sitefinity field is required.";
                    Logger.Error(error);
                    throw new ArgumentException(error);
                }

                if (mappingFields.Any(i => string.IsNullOrEmpty(i.GigyaFieldName)))
                {
                    var error = "Gigya DS field is required.";
                    Logger.Error(error);
                    throw new ArgumentException(error);
                }

                if (mappingFields.Any(i => string.IsNullOrEmpty(i.Oid)))
                {
                    var error = "Gigya DS OID field is required.";
                    Logger.Error(error);
                    throw new ArgumentException(error);
                }

                if (mappingFields.Any(i => !i.GigyaFieldName.StartsWith("ds.") || i.GigyaFieldName.Split('.').Length < 3))
                {
                    var error = "Gigya DS fields must be in the format ds.type.fieldName";
                    Logger.Error(error);
                    throw new ArgumentException(error);
                }

                // remove old mappings
                if (settings.Mappings != null)
                {
                    context.Delete(settings.Mappings);
                }

                // update all fields
                settings.Method = this.Method;
                settings.Mappings = mappingFields.Select(i => new Data.GigyaSitefinityDsMapping
                {
                    CmsName = i.CmsFieldName,
                    DsSettingId = settings.SiteId,
                    GigyaName = i.GigyaFieldName,
                    Oid = i.Oid
                }).ToList();

                var mappedSettings = SettingsHelper.Map(settings);
                if (_validateDsSettings && !Validate(mappedSettings))
                {
                    return;
                }

                context.Add(settings);
                context.SaveChanges();
                SettingsHelper.ClearCache(SiteId);
            }
        }

        private bool Validate(GigyaDsSettings settings)
        {
            var coreSettingsHelper = new Gigya.Module.Connector.Helpers.GigyaSettingsHelper();
            var coreSettings = coreSettingsHelper.Get(settings.SiteId, true);

            var dsHelper = new GigyaDsHelper(coreSettings, Logger, settings);

            var errorMessage = dsHelper.Validate();
            if (!string.IsNullOrEmpty(errorMessage))
            {
                Logger.Error(errorMessage);
                throw new ArgumentException(errorMessage);
            }

            return true;
        }

        #endregion
    }
}