using Gigya.Module.Core.Connector.Common;
using Gigya.Module.Core.Connector.Logging;
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
    public class GigyaFacetMapper : FacetMapperBase<GigyaFieldsMapping>
    {
        public GigyaFacetMapper(IContactProfileProvider contactProfileProvider, Logger logger) : base(contactProfileProvider, logger)
        {
        }

        protected override void UpdateFacet(dynamic gigyaModel, GigyaFieldsMapping mapping)
        {
            if (mapping == null || mapping.Entries == null || !mapping.Entries.Any())
            {
                return;
            }

            try
            {
                var facet = _contactProfileProvider.Gigya;

                foreach (var entry in mapping.Entries)
                {
                    var field = facet.Entries.Contains(entry.Key) ? facet.Entries[entry.Key] : facet.Entries.Create(entry.Key);
                    field.Value = DynamicUtils.GetValue<object>(gigyaModel, entry.GigyaProperty);
                }
            }
            catch (FacetNotAvailableException ex)
            {
                _logger.Warn("The 'Personal' facet is not available.", ex);
            }
        }
    }
}