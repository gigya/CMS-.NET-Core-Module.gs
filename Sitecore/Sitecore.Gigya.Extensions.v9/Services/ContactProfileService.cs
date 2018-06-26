using Gigya.Module.Core.Connector.Common;
using Gigya.Module.Core.Connector.Logging;
using Sitecore.Diagnostics;
using Sitecore.Exceptions;
using Sitecore.Gigya.Extensions.Abstractions.Analytics.Models;
using Sitecore.Gigya.Extensions.Abstractions.Services;
using Sitecore.Gigya.Extensions.Providers;
using Sitecore.Gigya.Extensions.Services.FacetMappers;
using Sitecore.XConnect;
using Sitecore.XConnect.Collection.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;

namespace Sitecore.Gigya.Extensions.Services
{
    public class ContactProfileService : IContactProfileService
    {
        protected readonly Logger _logger;
        public IContactProfileProvider ContactProfileProvider { get; private set; }

        public ContactProfileService(IContactProfileProvider contactProfileProvider, Logger logger)
        {
            ContactProfileProvider = contactProfileProvider;
            _logger = logger;
        }

        public Task UpdateFacetsAsync(dynamic gigyaModel, MappingFieldGroup mapping)
        {
            new PersonalFacetMapper(ContactProfileProvider, _logger).Update(gigyaModel, mapping.PersonalInfoMapping);
            //new AddressFacetMapper(ContactProfileProvider, _logger).Update(gigyaModel, mapping.AddressesMapping);
            //new PhoneNumbersFacetMapper(ContactProfileProvider, _logger).Update(gigyaModel, mapping.PhoneNumbersMapping);
            //new EmailAddressFacetMapper(ContactProfileProvider, _logger).Update(gigyaModel, mapping.EmailAddressesMapping);

            //new CommunicationProfileFacetMapper(ContactProfileProvider, _logger).Update(gigyaModel, mapping.CommunicationProfileMapping);
            //new PreferencesFacetMapper(ContactProfileProvider, _logger).Update(gigyaModel, mapping.CommunicationPreferencesMapping);
            //new GigyaFacetMapper(ContactProfileProvider, _logger).Update(gigyaModel, mapping.GigyaFieldsMapping);

            ContactProfileProvider.Flush();
            return Task.CompletedTask;
        }
    }
}