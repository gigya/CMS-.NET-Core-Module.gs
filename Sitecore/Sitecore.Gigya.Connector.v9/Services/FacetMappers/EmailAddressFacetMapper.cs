using Gigya.Module.Core.Connector.Common;
using Gigya.Module.Core.Connector.Logging;
using Sitecore.Analytics.Model.Entities;
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

                for (int i = 0; i < mapping.Entries.Count; i++)
                {
                    var mappingEntry = mapping.Entries[i];
                    if (string.IsNullOrEmpty(mappingEntry.Key))
                    {
                        _logger.Error("Key for email mapping is empty.");
                        continue;
                    }

                    var model = Map(gigyaModel, mappingEntry);

                    if (i == 0)
                    {
                        if (facet == null)
                        {
                            facet = new EmailAddressList(model, mappingEntry.Key);
                        }
                        else
                        {
                            facet.PreferredEmail = model;
                            facet.PreferredKey = mappingEntry.Key;
                        }
                    }
                    else
                    {
                        facet.Others.Add(mappingEntry.Key, model);
                    }
                }

                _contactProfileProvider.SetFacet(facet, EmailAddressList.DefaultFacetKey);
            }
            catch (FacetNotAvailableException ex)
            {
                _logger.Warn("The 'Email Addresses' facet is not available.", ex);
            }
        }

        private EmailAddress Map(dynamic gigyaModel, ContactEmailAddressMapping entryMapping)
        {
            var smtpAddress = DynamicUtils.GetValue<string>(gigyaModel, entryMapping.SmtpAddress);
            var validated = DynamicUtils.GetValue<bool>(gigyaModel, entryMapping.Validated);
            var entry = new EmailAddress(smtpAddress, validated);
            return entry;
        }
    }
}