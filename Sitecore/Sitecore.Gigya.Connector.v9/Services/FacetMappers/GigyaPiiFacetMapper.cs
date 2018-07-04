using Gigya.Module.Core.Connector.Common;
using Gigya.Module.Core.Connector.Logging;
using Sitecore.Analytics.Model.Framework;
using Sitecore.Gigya.Connector.Providers;
using Sitecore.Gigya.Extensions.Abstractions.Analytics.Models;
using Sitecore.Gigya.Extensions.Abstractions.Services;
using Sitecore.Gigya.XConnect.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using C = Sitecore.Gigya.Extensions.Abstractions.Analytics.Constants;

namespace Sitecore.Gigya.Connector.Services.FacetMappers
{
    public class GigyaPiiFacetMapper : GigyaFacetMapperBase<GigyaPiiFacet>
    {
        public GigyaPiiFacetMapper(IContactProfileProvider contactProfileProvider, Logger logger) : base(contactProfileProvider, logger)
        {
        }

        protected override string FacetKey => C.FacetKeys.GigyaPii;

        protected override void SetFacet(GigyaPiiFacet facet)
        {
            _contactProfileProvider.SetFacet(facet, FacetKey);
        }

        protected override GigyaPiiFacet GetOrCreateFacet()
        {
            return _contactProfileProvider.GigyaPii ?? new GigyaPiiFacet { Entries = new Dictionary<string, GigyaElement>() };
        }
    }
}