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
    public class EmailAddressFacetMapper : FacetMapperBase<ContactEmailAddressesMapping>
    {
        public EmailAddressFacetMapper(IContactProfileProvider contactProfileProvider, Logger logger) : base(contactProfileProvider, logger)
        {
        }

        protected override void UpdateFacet(dynamic gigyaModel, ContactEmailAddressesMapping mapping)
        {
            if (mapping == null || mapping.Entries == null || !mapping.Entries.Any())
            {
                return;
            }

            try
            {
                var facet = _contactProfileProvider.Emails;

                var firstKey = mapping.Entries.FirstOrDefault(i => !string.IsNullOrEmpty(i.Key))?.Key;
                if (!string.IsNullOrEmpty(firstKey))
                {
                    facet.Preferred = firstKey;
                }

                foreach (var entryMapping in mapping.Entries)
                {
                    if (string.IsNullOrEmpty(entryMapping.Key))
                    {
                        _logger.Error("Key for email address mapping is empty.");
                        continue;
                    }

                    IEmailAddress emailAddress;
                    if (!facet.Entries.Contains(entryMapping.Key))
                    {
                        emailAddress = facet.Entries.Create(entryMapping.Key);
                    }
                    else
                    {
                        emailAddress = facet.Entries[entryMapping.Key];
                    }

                    emailAddress.SmtpAddress = DynamicUtils.GetValue<string>(gigyaModel, entryMapping.SmtpAddress);
                    emailAddress.BounceCount = DynamicUtils.GetValue<int>(gigyaModel, entryMapping.BounceCount);
                }
            }
            catch (FacetNotAvailableException ex)
            {
                _logger.Warn("The 'Email Addresses' facet is not available.", ex);
            }
        }
    }
}