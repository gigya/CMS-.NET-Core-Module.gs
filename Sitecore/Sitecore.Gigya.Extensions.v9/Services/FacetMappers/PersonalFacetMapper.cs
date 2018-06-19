using Gigya.Module.Core.Connector.Common;
using Gigya.Module.Core.Connector.Logging;
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
    public class PersonalFacetMapper : FacetMapperBase<ContactPersonalInfoMapping>
    {
        public PersonalFacetMapper(IContactProfileProvider contactProfileProvider, Logger logger) : base(contactProfileProvider, logger)
        {
        }

        protected override void UpdateFacet(dynamic gigyaModel, ContactPersonalInfoMapping mapping)
        {
            try
            {
                var facet = _contactProfileProvider.PersonalInfo;
                var exists = facet != null;
                if (!exists)
                {
                    facet = new PersonalInformation();
                }

                facet.Birthdate = DynamicUtils.GetValue<DateTime?>(gigyaModel, mapping.BirthDate);
                facet.FirstName = DynamicUtils.GetValue<string>(gigyaModel, mapping.FirstName);
                facet.Gender = DynamicUtils.GetValue<string>(gigyaModel, mapping.Gender);
                facet.JobTitle = DynamicUtils.GetValue<string>(gigyaModel, mapping.JobTitle);
                facet.MiddleName = DynamicUtils.GetValue<string>(gigyaModel, mapping.MiddleName);
                facet.Nickname = DynamicUtils.GetValue<string>(gigyaModel, mapping.Nickname);
                facet.Suffix = DynamicUtils.GetValue<string>(gigyaModel, mapping.Suffix);
                facet.LastName = DynamicUtils.GetValue<string>(gigyaModel, mapping.Surname);
                facet.Title = DynamicUtils.GetValue<string>(gigyaModel, mapping.Title);
                facet.PreferredLanguage = DynamicUtils.GetValue<string>(gigyaModel, mapping.PreferredLanguage);

                if (!exists)
                {
                    _contactProfileProvider.SetFacet(facet, PersonalInformation.DefaultFacetKey);
                }
            }
            catch (FacetNotAvailableException ex)
            {
                _logger.Warn("The 'Personal' facet is not available.", ex);
            }
        }
    }
}