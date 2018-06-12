using Gigya.Module.Core.Mvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using SC = Sitecore.Gigya.Module.Models;

using Core = Gigya.Module.Core;
using Gigya.Module.Core.Connector.Models;
using Sitecore.Data.Items;
using Sitecore.Gigya.Module.Controllers;
using Sitecore.Gigya.Module.Models;
using A = Sitecore.Gigya.Extensions.Abstractions.Analytics.Models;

namespace Sitecore.Gigya.Module.Helpers
{
    public class Mapper
    {
        public static SC.GigyaSettingsViewModel Map(Core.Mvc.Models.GigyaSettingsViewModel source)
        {
            return new SC.GigyaSettingsViewModel
            {
                ApiKey = source.ApiKey,
                DataCenter = source.DataCenter,
                DebugMode = source.DebugMode,
                ErrorMessage = source.ErrorMessage,
                GigyaScriptPath = source.GigyaScriptPath,
                Id = source.Id,
                IsGetInfoRequired = source.IsGetInfoRequired,
                IsLoggedIn = source.IsLoggedIn,
                LoggedInRedirectUrl = source.LoggedInRedirectUrl,
                LogoutUrl = source.LogoutUrl,
                RenderScript = source.RenderScript,
                Settings = source.Settings,
                SettingsJson = source.SettingsJson,
                EnableSSOToken = source.EnableSSOToken
            };
        }

        public static MappingField Map(Item item)
        {
            return new MappingField
            {
                CmsFieldName = item.Fields[Constants.Fields.MappingFields.SitecoreProperty].Value,
                GigyaFieldName = item.Fields[Constants.Fields.MappingFields.GigyaProperty].Value,
            };
        }

        public static A.MappingFieldGroup MapMappingFieldGroup(Item item)
        {
            var facet = new A.MappingFieldGroup { FacetName = item.Fields[Constants.Fields.FacetFolder.Name]?.Value };
            facet.Fields = item.Children.Select(MapMappingField).ToList();
            return facet;
        }

        public static A.MappingField MapMappingField(Item item)
        {
            return new A.MappingField
            {
                CmsFieldName = item.Fields[Constants.Fields.MappingFields.SitecoreProperty]?.Value,
                GigyaFieldName = item.Fields[Constants.Fields.MappingFields.GigyaProperty]?.Value,
            };
        }

        public static AutocompleteSuggestion Map(AccountSchemaProperty item)
        {
            return new AutocompleteSuggestion
            {
                Data = item.Name,
                Value = item.Name
            };
        }
    }
}