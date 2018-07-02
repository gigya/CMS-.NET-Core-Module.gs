using Gigya.Module.Core.Connector.Common;
using Gigya.Module.Core.Connector.Logging;
using Sitecore.Analytics.Model.Entities;
using Sitecore.Analytics.Model.Framework;
using Sitecore.Gigya.Extensions.Abstractions.Analytics.Models;
using Sitecore.Gigya.Extensions.Abstractions.Services;
using Sitecore.Gigya.Connector.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Gigya.Connector.Services.FacetMappers
{
    public class AddressFacetMapper : FacetMapperBase<ContactAddressesMapping>
    {
        public AddressFacetMapper(IContactProfileProvider contactProfileProvider, Logger logger) : base(contactProfileProvider, logger)
        {
        }

        protected override void UpdateFacet(object gigyaModel, ContactAddressesMapping mapping)
        {
            if (mapping == null || mapping.Entries == null || !mapping.Entries.Any())
            {
                return;
            }

            try
            {
                var facet = _contactProfileProvider.Addresses;

                var firstKey = mapping.Entries.FirstOrDefault(i => !string.IsNullOrEmpty(i.Key))?.Key;
                if (!string.IsNullOrEmpty(firstKey))
                {
                    facet.Preferred = firstKey;
                }

                foreach (var entryMapping in mapping.Entries)
                {
                    if (string.IsNullOrEmpty(entryMapping.Key))
                    {
                        _logger.Error("Key for address mapping is empty.");
                        continue;
                    }

                    IAddress entry;
                    if (!facet.Entries.Contains(entryMapping.Key))
                    {
                        entry = facet.Entries.Create(entryMapping.Key);
                    }
                    else
                    {
                        entry = facet.Entries[entryMapping.Key];
                    }

                    entry.City = DynamicUtils.GetValue<string>(gigyaModel, entryMapping.City);
                    entry.Country = DynamicUtils.GetValue<string>(gigyaModel, entryMapping.Country);
                    entry.PostalCode = DynamicUtils.GetValue<string>(gigyaModel, entryMapping.PostalCode);
                    entry.StateProvince = DynamicUtils.GetValue<string>(gigyaModel, entryMapping.StateProvince);
                    entry.StreetLine1 = DynamicUtils.GetValue<string>(gigyaModel, entryMapping.StreetLine1);
                    entry.StreetLine2 = DynamicUtils.GetValue<string>(gigyaModel, entryMapping.StreetLine2);
                    entry.StreetLine3 = DynamicUtils.GetValue<string>(gigyaModel, entryMapping.StreetLine3);
                    entry.StreetLine4 = DynamicUtils.GetValue<string>(gigyaModel, entryMapping.StreetLine4);
                    entry.Location.Latitude = DynamicUtils.GetValue<float>(gigyaModel, entryMapping.Latitude);
                    entry.Location.Longitude = DynamicUtils.GetValue<float>(gigyaModel, entryMapping.Longitude);
                }
            }
            catch (FacetNotAvailableException ex)
            {
                _logger.Warn("The 'Addresses' facet is not available.", ex);
            }
        }
    }
}