using Gigya.Module.Core.Connector.Common;
using Gigya.Module.Core.Connector.Logging;
using Sitecore.Analytics.Model.Framework;
using Sitecore.Gigya.Connector.Providers;
using Sitecore.Gigya.Extensions.Abstractions.Analytics.Models;
using Sitecore.Gigya.Extensions.Abstractions.Services;
using Sitecore.Gigya.XConnect.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Gigya.Connector.Services.FacetMappers
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
                var facet = _contactProfileProvider.Gigya ?? new GigyaFacet { Entries = new Dictionary<string, GigyaElement>() };

                foreach (var entry in mapping.Entries)
                {
                    if (!facet.Entries.TryGetValue(entry.Key, out GigyaElement gigyaElement))
                    {
                        gigyaElement = new GigyaElement();
                        facet.Entries.Add(entry.Key, gigyaElement);
                    }

                    var gigyaValue = DynamicUtils.GetValue<object>(gigyaModel, entry.GigyaProperty);
                    if (gigyaValue != null)
                    {
                        gigyaElement.Value = gigyaValue.ToString();
                    }
                }

                _contactProfileProvider.SetFacet(facet, GigyaFacet.DefaultFacetKey);
            }
            catch (FacetNotAvailableException ex)
            {
                _logger.Warn("The 'Personal' facet is not available.", ex);
            }
        }
    }
}