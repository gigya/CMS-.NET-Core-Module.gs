using Gigya.Module.Core.Connector.Common;
using Gigya.Module.Core.Connector.Logging;
using Sitecore.Diagnostics;
using Sitecore.Exceptions;
using Sitecore.Gigya.DependencyInjection;
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

        public async Task UpdateFacetsAsync(dynamic gigyaModel, MappingFieldGroup mapping)
        {
            using (XConnect.Client.XConnectClient client = Sitecore.XConnect.Client.Configuration.SitecoreXConnectClientConfiguration.GetClient())
            {
                {
                    try
                    {
                        Contact contact = new Contact(new ContactIdentifier("twitter", "myrtlesitecore", ContactIdentifierType.Known));

                        client.AddContact(contact);

                        // Facet with a reference object, key is specified
                        PersonalInformation personalInfoFacet = new PersonalInformation()
                        {
                            FirstName = "Myrtle",
                            LastName = "McSitecore"
                        };

                        FacetReference reference = new FacetReference(contact, PersonalInformation.DefaultFacetKey);

                        client.SetFacet(reference, personalInfoFacet);

                        // Facet without a reference, using default key
                        EmailAddressList emails = new EmailAddressList(new EmailAddress("myrtle@test.test", true), "Home");

                        client.SetFacet(contact, emails);

                        // Facet without a reference, key is specified

                        AddressList addresses = new AddressList(new Address() { AddressLine1 = "Cool Street 12", City = "Sitecore City", PostalCode = "ABC 123" }, "Home");

                        client.SetFacet(contact, AddressList.DefaultFacetKey, addresses);

                        // Submit operations as batch
                        await client.SubmitAsync();
                    }
                    catch (XdbExecutionException ex)
                    {
                        _logger.Error("Failed to update contact facets", ex);
                    }
                }
            }

            new PersonalFacetMapper(ContactProfileProvider, _logger).Update(gigyaModel, mapping.PersonalInfoMapping);
            //new AddressFacetMapper(ContactProfileProvider, _logger).Update(gigyaModel, mapping.AddressesMapping);
            //new PhoneNumbersFacetMapper(ContactProfileProvider, _logger).Update(gigyaModel, mapping.PhoneNumbersMapping);
            //new EmailAddressFacetMapper(ContactProfileProvider, _logger).Update(gigyaModel, mapping.EmailAddressesMapping);

            //new CommunicationProfileFacetMapper(ContactProfileProvider, _logger).Update(gigyaModel, mapping.CommunicationProfileMapping);
            //new PreferencesFacetMapper(ContactProfileProvider, _logger).Update(gigyaModel, mapping.CommunicationPreferencesMapping);
            //new GigyaFacetMapper(ContactProfileProvider, _logger).Update(gigyaModel, mapping.GigyaFieldsMapping);

            ContactProfileProvider.Flush();
        }
    }
}