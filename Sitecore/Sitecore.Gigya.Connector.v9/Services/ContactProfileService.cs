using Gigya.Module.Core.Connector.Common;
using Gigya.Module.Core.Connector.Logging;
using Sitecore.Diagnostics;
using Sitecore.Exceptions;
using Sitecore.Gigya.Extensions.Abstractions.Analytics.Models;
using Sitecore.Gigya.Extensions.Abstractions.Services;
using Sitecore.Gigya.Connector.Providers;
using Sitecore.Gigya.Connector.Services.FacetMappers;
using Sitecore.XConnect;
using Sitecore.XConnect.Collection.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using Sitecore.Gigya.Extensions.Abstractions;
using LFM = Sitecore.Gigya.Connector.Services.LegacyFacetMappers;
using Sitecore.Gigya.Module.Models.Pipelines;
using Sitecore.Pipelines;

namespace Sitecore.Gigya.Connector.Services
{
    public class ContactProfileService : IContactProfileService
    {
        protected readonly Logger _logger;
        public IContactProfileProvider ContactProfileProvider { get; private set; }
        public ILegacyContactProfileProvider LegacyContactProfileProvider { get; private set; }

        public ContactProfileService(IContactProfileProvider contactProfileProvider, ILegacyContactProfileProvider legacyContactProfileProvider, Logger logger)
        {
            ContactProfileProvider = contactProfileProvider;
            LegacyContactProfileProvider = legacyContactProfileProvider;
            _logger = logger;
        }

        public Task UpdateFacetsAsync(dynamic gigyaModel, MappingFieldGroup mapping)
        {
            try
            {
                new PersonalFacetMapper(ContactProfileProvider, _logger).Update(gigyaModel, mapping.PersonalInfoMapping);
                new AddressFacetMapper(ContactProfileProvider, _logger).Update(gigyaModel, mapping.AddressesMapping);
                new PhoneNumbersFacetMapper(ContactProfileProvider, _logger).Update(gigyaModel, mapping.PhoneNumbersMapping);
                new EmailAddressFacetMapper(ContactProfileProvider, _logger).Update(gigyaModel, mapping.EmailAddressesMapping);
                new GigyaFacetMapper(ContactProfileProvider, _logger).Update(gigyaModel, mapping.GigyaFieldsMapping);
                new GigyaPiiFacetMapper(ContactProfileProvider, _logger).Update(gigyaModel, mapping.GigyaPiiFieldsMapping);

                // legacy facets in session aren't updated using xconnect...so we have to do it twice...convenient
                UpdateLegacyFacets(gigyaModel, mapping);

                // add a pipeline here for custom facets
                var args = new FacetsUpdatedPipelineArgs
                {
                    GigyaModel = gigyaModel,
                    Mappings = mapping
                };
                CorePipeline.Run("gigya.module.facetsAllUpdated", args, false);

                ContactProfileProvider.Flush();
            }
            catch (Exception e)
            {
                _logger.Error("Unable to update facets.", e);
            }

            return Task.CompletedTask;
        }

        private void UpdateLegacyFacets(dynamic gigyaModel, MappingFieldGroup mapping)
        {
            new LFM.PersonalFacetMapper(LegacyContactProfileProvider, _logger).Update(gigyaModel, mapping.PersonalInfoMapping);
            new LFM.AddressFacetMapper(LegacyContactProfileProvider, _logger).Update(gigyaModel, mapping.AddressesMapping);
            new LFM.PhoneNumbersFacetMapper(LegacyContactProfileProvider, _logger).Update(gigyaModel, mapping.PhoneNumbersMapping);
            new LFM.EmailAddressFacetMapper(LegacyContactProfileProvider, _logger).Update(gigyaModel, mapping.EmailAddressesMapping);
            new LFM.CommunicationProfileFacetMapper(LegacyContactProfileProvider, _logger).Update(gigyaModel, mapping.CommunicationProfileMapping);
            new LFM.PreferencesFacetMapper(LegacyContactProfileProvider, _logger).Update(gigyaModel, mapping.CommunicationPreferencesMapping);
        }
    }
}