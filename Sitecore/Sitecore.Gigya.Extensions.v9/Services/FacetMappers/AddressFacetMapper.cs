using Gigya.Module.Core.Connector.Common;
using Gigya.Module.Core.Connector.Logging;
using Sitecore.Analytics.Model.Entities;
using Sitecore.Analytics.Model.Framework;
using Sitecore.Gigya.Extensions.Abstractions.Analytics.Models;
using Sitecore.Gigya.Extensions.Abstractions.Services;
using Sitecore.Gigya.Extensions.Providers;
using Sitecore.XConnect.Collection.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Gigya.Extensions.Services.FacetMappers
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

                for (int i = 0; i < mapping.Entries.Count; i++)
                {
                    var mappingEntry = mapping.Entries[i];
                    if (string.IsNullOrEmpty(mappingEntry.Key))
                    {
                        _logger.Error("Key for address mapping is empty.");
                        continue;
                    }

                    var address = Map(gigyaModel, mappingEntry); ;

                    if (i == 0)
                    {
                        facet.PreferredAddress = address;
                        facet.PreferredKey = mappingEntry.Key;
                    }
                    else
                    {
                        facet.Others.Add(mappingEntry.Key, address);
                    }
                }
            }
            catch (FacetNotAvailableException ex)
            {
                _logger.Warn("The 'Addresses' facet is not available.", ex);
            }
        }

        private Address Map(dynamic gigyaModel, ContactAddressMapping entryMapping)
        {
            var entry = new Address();

            entry.City = DynamicUtils.GetValue<string>(gigyaModel, entryMapping.City);
            entry.CountryCode = DynamicUtils.GetValue<string>(gigyaModel, entryMapping.Country);
            entry.PostalCode = DynamicUtils.GetValue<string>(gigyaModel, entryMapping.PostalCode);
            entry.StateOrProvince = DynamicUtils.GetValue<string>(gigyaModel, entryMapping.StateProvince);
            entry.AddressLine1 = DynamicUtils.GetValue<string>(gigyaModel, entryMapping.StreetLine1);
            entry.AddressLine2 = DynamicUtils.GetValue<string>(gigyaModel, entryMapping.StreetLine2);
            entry.AddressLine3 = DynamicUtils.GetValue<string>(gigyaModel, entryMapping.StreetLine3);
            entry.AddressLine4 = DynamicUtils.GetValue<string>(gigyaModel, entryMapping.StreetLine4);
            entry.GeoCoordinate.Latitude = DynamicUtils.GetValue<float>(gigyaModel, entryMapping.Latitude);
            entry.GeoCoordinate.Longitude = DynamicUtils.GetValue<float>(gigyaModel, entryMapping.Longitude);

            return entry;
        }
    }
}