//using Gigya.Module.Core.Connector.Common;
//using Gigya.Module.Core.Connector.Logging;
//using Sitecore.Analytics.Model.Entities;
//using Sitecore.Analytics.Model.Framework;
//using Sitecore.Gigya.Extensions.Abstractions.Analytics.Models;
//using Sitecore.Gigya.Extensions.Abstractions.Services;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;

//namespace Sitecore.Gigya.Connector.Services.FacetMappers
//{
//    public class PreferencesFacetMapper : FacetMapperBase<PreferencesMapping>
//    {
//        public PreferencesFacetMapper(IContactProfileProvider contactProfileProvider, Logger logger) : base(contactProfileProvider, logger)
//        {
//        }

//        protected override void UpdateFacet(dynamic gigyaModel, PreferencesMapping mapping)
//        {
//            if (mapping == null)
//            {
//                return;
//            }

//            try
//            {
//                var facet = _contactProfileProvider.Preferences;

//                facet.Language = DynamicUtils.GetValue<string>(gigyaModel, mapping.Language);
//            }
//            catch (FacetNotAvailableException ex)
//            {
//                _logger.Warn("The 'Preferences' facet is not available.", ex);
//            }
//        }
//    }
//}