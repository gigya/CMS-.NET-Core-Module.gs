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
    public abstract class GigyaFacetMapperBase<T> : FacetMapperBase<GigyaFieldsMapping> where T: GigyaXConnectFacet
    {
        public GigyaFacetMapperBase(IContactProfileProvider contactProfileProvider, Logger logger) : base(contactProfileProvider, logger)
        {
        }

        protected abstract T GetOrCreateFacet();
        protected abstract void SetFacet(T facet);

        protected abstract string FacetKey { get; }

        protected override void UpdateFacet(dynamic gigyaModel, GigyaFieldsMapping mapping)
        {
            if (mapping == null || mapping.Entries == null || !mapping.Entries.Any())
            {
                return;
            }

            try
            {
                var facet = GetOrCreateFacet();

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

                SetFacet(facet);
            }
            catch (FacetNotAvailableException ex)
            {
                _logger.Warn("The " + FacetKey + " facet is not available.", ex);
            }
        }
    }
}