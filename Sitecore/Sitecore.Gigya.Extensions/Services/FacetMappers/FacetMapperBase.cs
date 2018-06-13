using Gigya.Module.Core.Connector.Logging;
using Sitecore.Gigya.Extensions.Abstractions.Analytics.Models;
using Sitecore.Gigya.Extensions.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Gigya.Extensions.Services.FacetMappers
{
    public abstract class FacetMapperBase<T> where T: MappingBase
    {
        protected readonly Logger _logger;
        public IContactProfileProvider _contactProfileProvider;

        public FacetMapperBase(IContactProfileProvider contactProfileProvider, Logger logger)
        {
            _contactProfileProvider = contactProfileProvider;
            _logger = logger;
        }

        public void Update(dynamic gigyaModel, T mapping)
        {
            if (mapping == null)
            {
                return;
            }

            UpdateFacet(gigyaModel, mapping);
        }

        protected abstract void UpdateFacet(dynamic gigyaModel, T mapping);
    }
}