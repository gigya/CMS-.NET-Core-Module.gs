namespace Sitecore.Gigya.Extensions.Abstractions.Repositories
{
    using Sitecore.Mvc.Presentation;

    public interface IRenderingPropertiesRepository
    {
        T Get<T>(Rendering rendering);
    }
}