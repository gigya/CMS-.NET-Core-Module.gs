using Gigya.Module.Core.Connector.Logging;
using Gigya.Module.DS.Config;
using Gigya.Module.DS.Helpers;
using Gigya.Umbraco.Module.Connector;
using Gigya.Umbraco.Module.DS.Helpers;
using Gigya.Umbraco.Module.DS.Mvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;

namespace Gigya.Umbraco.Module.DS.Mvc.Controllers
{
    [PluginController("GigyaDs")]
    public class GigyaDsSettingsApiController : UmbracoAuthorizedJsonController
    {
        private Logger _logger = new Logger(new UmbracoLogger());

        /// <summary>
        /// Gets all the settings data required for the client.
        /// </summary>
        /// <param name="id">Id of the homepage or -1 if global settings.</param>
        public GigyaDsSettingsApiResponseModel Get(int id)
        {
            var settingsHelper = new GigyaUmbracoDsSettingsHelper(_logger);
            var data = settingsHelper.Get(id.ToString(), false) ?? new GigyaDsSettings { SiteId = new string[] { id.ToString() }, Mappings = new List<GigyaDsMapping>() };
            var model = GetModel(id, data);

            var memberType = this.ApplicationContext.Services.MemberTypeService.Get(Constants.MemberTypeAlias);
            
            var wrappedModel = new GigyaDsSettingsApiResponseModel
            {
                Settings = model,
                Data = new GigyaDsConfigViewModel
                {
                    MemberProperties = memberType.PropertyTypes.Select(i => new GigyaDsMemberPropertyViewModel
                    {
                        Alias = i.Alias,
                        Name = i.Name
                    }).ToList()
                }
            };

            return wrappedModel;
        }

        /// <summary>
        /// Saves Gigya settings to the db.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public GigyaDsSettingsResponseModel Save(GigyaDsSettingsViewModel model)
        {
            var response = new GigyaDsSettingsResponseModel();
            if (!ModelState.IsValid)
            {
                var errorList = ModelState.Values.SelectMany(m => m.Errors)
                                     .Select(e => e.ErrorMessage)
                                     .ToList();

                response.Error = string.Join(" ", errorList);
                _logger.Error(response.Error);
                return response;
            }

            var settingsHelper = new GigyaUmbracoDsSettingsHelper(_logger);
            var settings = settingsHelper.GetRaw(model.Id);

            if (settings == null)
            {
                response.Error = "Settings not found for id: " + model.Id;
                _logger.Error(response.Error);
                return response;
            }

            if (model.Inherited && model.Id > 0)
            {
                if (settings.Id > -1)
                {
                    // don't delete global settings
                    settingsHelper.Delete(settings);
                }

                settingsHelper.ClearCache(model.Id);
                response.Success = true;

                // return global settings to refresh client
                var globalData = settingsHelper.Get(model.Id.ToString());
                var globalModel = GetModel(model.Id, globalData);
                response.Settings = globalModel;
                return response;
            }

            if (model.Mappings == null || !model.Mappings.Any())
            {
                response.Error = "At least one mapping is required.";
                return response;
            }

            if (model.Mappings.Any(i => string.IsNullOrEmpty(i.CmsName)))
            {
                response.Error = "Umbraco field is required.";
                return response;
            }

            if (model.Mappings.Any(i => string.IsNullOrEmpty(i.GigyaName)))
            {
                response.Error = "Gigya DS field is required.";
                return response;
            }

            if (model.Mappings.Any(i => string.IsNullOrEmpty(i.Oid)))
            {
                response.Error = "Gigya DS OID field is required.";
                return response;
            }

            // map back to db models
            settings.Method = (int)model.Method;
            settings.Mappings = model.Mappings.Select(i => new Data.GigyaUmbracoDsMapping
            {
                CmsName = i.CmsName,
                DsSettingId = settings.Id,
                GigyaName = i.GigyaName,
                Oid = i.Oid
            }).ToList();

            settingsHelper.Save(settings);

            response.Success = true;
            settingsHelper.ClearCache(model.Id);
            return response;
        }

        private GigyaDsSettingsViewModel GetModel(int id, GigyaDsSettings data)
        {
            var model = Map(data);

            var idString = id.ToString();
            model.Inherited = data.SiteId.All(i => i != idString);
            model.Id = id;
            
            return model;
        }

        /// <summary>
        /// Maps the core module to a vew model.
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        private GigyaDsSettingsViewModel Map(GigyaDsSettings settings)
        {
            var model = new GigyaDsSettingsViewModel
            {
                Mappings = settings.Mappings.Select(i => new GigyaDsMappingViewModel
                {
                    CmsName = i.CmsName,
                    GigyaName = i.GigyaName,
                    Oid = i.Custom.Oid
                }).ToList(),
                Method = settings.Method
            };

            return model;
        }
    }
}
