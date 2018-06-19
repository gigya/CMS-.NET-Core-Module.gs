using Gigya.Module.Core.Connector.Common;
using Gigya.Module.Core.Connector.Logging;
using Sitecore.Analytics.Model.Entities;
using Sitecore.Analytics.Model.Framework;
using Sitecore.Gigya.Extensions.Abstractions.Analytics.Models;
using Sitecore.Gigya.Extensions.Abstractions.Services;
using Sitecore.Gigya.Extensions.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Gigya.Extensions.Services.FacetMappers
{
    public class CommunicationProfileFacetMapper : FacetMapperBase<CommunicationProfileMapping>
    {
        public CommunicationProfileFacetMapper(IContactProfileProvider contactProfileProvider, Logger logger) : base(contactProfileProvider, logger)
        {
        }

        protected override void UpdateFacet(dynamic gigyaModel, CommunicationProfileMapping mapping)
        {
            if (mapping == null)
            {
                return;
            }

            try
            {
                var facet = _contactProfileProvider.CommunicationProfile;

                facet.CommunicationRevoked = DynamicUtils.GetValue<bool>(gigyaModel, mapping.CommunicationRevoked);
                facet.ConsentRevoked = DynamicUtils.GetValue<bool>(gigyaModel, mapping.ConsentRevoked);
            }
            catch (FacetNotAvailableException ex)
            {
                _logger.Warn("The 'Communication Profile' facet is not available.", ex);
            }
        }
    }
}