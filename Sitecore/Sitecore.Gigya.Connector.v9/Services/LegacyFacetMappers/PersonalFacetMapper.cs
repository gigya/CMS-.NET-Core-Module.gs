using Gigya.Module.Core.Connector.Common;
using Gigya.Module.Core.Connector.Logging;
using Sitecore.Analytics.Model.Framework;
using Sitecore.Gigya.Extensions.Abstractions.Analytics.Models;
using Sitecore.Gigya.Extensions.Abstractions.Services;
using Sitecore.Gigya.Connector.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Gigya.Connector.Services.LegacyFacetMappers
{
    public class PersonalFacetMapper : FacetMapperBase<ContactPersonalInfoMapping>
    {
        public PersonalFacetMapper(ILegacyContactProfileProvider contactProfileProvider, Logger logger) : base(contactProfileProvider, logger)
        {
        }

        protected override void UpdateFacet(dynamic gigyaModel, ContactPersonalInfoMapping mapping)
        {
            try
            {
                var facet = _contactProfileProvider.PersonalInfo;

                facet.BirthDate = DynamicUtils.GetValue<DateTime?>(gigyaModel, mapping.BirthDate);
                facet.FirstName = DynamicUtils.GetValue<string>(gigyaModel, mapping.FirstName);
                facet.Gender = DynamicUtils.GetValue<string>(gigyaModel, mapping.Gender);
                facet.JobTitle = DynamicUtils.GetValue<string>(gigyaModel, mapping.JobTitle);
                facet.MiddleName = DynamicUtils.GetValue<string>(gigyaModel, mapping.MiddleName);
                facet.Nickname = DynamicUtils.GetValue<string>(gigyaModel, mapping.Nickname);
                facet.Suffix = DynamicUtils.GetValue<string>(gigyaModel, mapping.Suffix);
                facet.Surname = DynamicUtils.GetValue<string>(gigyaModel, mapping.Surname);
                facet.Title = DynamicUtils.GetValue<string>(gigyaModel, mapping.Title);
            }
            catch (FacetNotAvailableException ex)
            {
                _logger.Warn("The 'Personal' facet is not available.", ex);
            }
        }
    }
}