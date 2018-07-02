using Gigya.Module.Core.Connector.Common;
using Gigya.Module.Core.Connector.Logging;
using Sitecore.Analytics.Model.Framework;
using Sitecore.Gigya.Extensions.Abstractions.Analytics.Models;
using Sitecore.Gigya.Extensions.Abstractions.Services;
using Sitecore.Gigya.Connector.Providers;
using Sitecore.XConnect.Collection.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Gigya.Connector.Services.FacetMappers
{
    public class ConsentFacetMapper : FacetMapperBase<ConsentMapping>
    {
        public ConsentFacetMapper(IContactProfileProvider contactProfileProvider, Logger logger) : base(contactProfileProvider, logger)
        {
        }

        protected override void UpdateFacet(dynamic gigyaModel, ConsentMapping mapping)
        {
            try
            {
                var facet = _contactProfileProvider.ConsentInformation;
                var exists = facet != null;
                if (!exists)
                {
                    facet = new ConsentInformation();
                }

                facet.ConsentRevoked = DynamicUtils.GetValue<bool>(gigyaModel, mapping.ConsentRevoked);
                facet.DoNotMarket = DynamicUtils.GetValue<bool>(gigyaModel, mapping.DoNotMarket);

                if (!exists)
                {
                    _contactProfileProvider.SetFacet(facet, PersonalInformation.DefaultFacetKey);
                }
            }
            catch (FacetNotAvailableException ex)
            {
                _logger.Warn("The 'Personal' facet is not available.", ex);
            }
        }
    }
}