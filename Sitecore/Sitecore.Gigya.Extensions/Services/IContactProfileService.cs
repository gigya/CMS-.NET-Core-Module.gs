using System.Collections.Generic;
using System.Threading.Tasks;
using Sitecore.Gigya.Extensions.Abstractions.Analytics.Models;
using Sitecore.Gigya.Extensions.Abstractions.Services;
using Sitecore.Gigya.Extensions.Providers;

namespace Sitecore.Gigya.Extensions.Services
{
    public interface IContactProfileService
    {
        IContactProfileProvider ContactProfileProvider { get; }

        Task UpdateFacetsAsync(dynamic gigyaModel, MappingFieldGroup mappingFields);
    }
}