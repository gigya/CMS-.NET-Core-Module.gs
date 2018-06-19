//using Gigya.Module.Core.Connector.Common;
//using Gigya.Module.Core.Connector.Logging;
//using Sitecore.Analytics.Model.Entities;
//using Sitecore.Analytics.Model.Framework;
//using Sitecore.Gigya.Extensions.Abstractions.Analytics.Models;
//using Sitecore.Gigya.Extensions.Abstractions.Services;
//using Sitecore.Gigya.Extensions.Providers;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;

//namespace Sitecore.Gigya.Extensions.Services.FacetMappers
//{
//    public class PhoneNumbersFacetMapper : FacetMapperBase<ContactPhoneNumbersMapping>
//    {
//        public PhoneNumbersFacetMapper(IContactProfileProvider contactProfileProvider, Logger logger) : base(contactProfileProvider, logger)
//        {
//        }

//        protected override void UpdateFacet(dynamic gigyaModel, ContactPhoneNumbersMapping mapping)
//        {
//            if (mapping == null || mapping.Entries == null || !mapping.Entries.Any())
//            {
//                return;
//            }

//            try
//            {
//                var facet = _contactProfileProvider.PhoneNumbers;

//                var firstKey = mapping.Entries.FirstOrDefault(i => !string.IsNullOrEmpty(i.Key))?.Key;
//                if (!string.IsNullOrEmpty(firstKey))
//                {
//                    facet.Preferred = firstKey;
//                }

//                foreach (var entryMapping in mapping.Entries)
//                {
//                    if (string.IsNullOrEmpty(entryMapping.Key))
//                    {
//                        _logger.Error("Key for phone number mapping is empty.");
//                        continue;
//                    }

//                    IPhoneNumber phoneNumber;
//                    if (!facet.Entries.Contains(entryMapping.Key))
//                    {
//                        phoneNumber = facet.Entries.Create(entryMapping.Key);
//                    }
//                    else
//                    {
//                        phoneNumber = facet.Entries[entryMapping.Key];
//                    }

//                    phoneNumber.CountryCode = DynamicUtils.GetValue<string>(gigyaModel, entryMapping.CountryCode);
//                    phoneNumber.Extension = DynamicUtils.GetValue<string>(gigyaModel, entryMapping.Extension);
//                    phoneNumber.Number = DynamicUtils.GetValue<string>(gigyaModel, entryMapping.Number);
//                }
//            }
//            catch (FacetNotAvailableException ex)
//            {
//                _logger.Warn("The 'Phone Numbers' facet is not available.", ex);
//            }
//        }
//    }
//}