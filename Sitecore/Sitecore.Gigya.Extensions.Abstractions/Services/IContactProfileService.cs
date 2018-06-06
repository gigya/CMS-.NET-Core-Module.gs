using System.Collections.Generic;
using Sitecore.Gigya.Extensions.Abstractions.Analytics.Models;
using Sitecore.Gigya.Extensions.Abstractions.Services;

namespace Sitecore.Gigya.Extensions.Abstractions.Services
{
    public interface IContactProfileService
    {
        IContactProfileProvider ContactProfileProvider { get; }

        void UpdateFacets(dynamic gigyaModel, List<MappingFieldGroup> mappingFields);
    }
}