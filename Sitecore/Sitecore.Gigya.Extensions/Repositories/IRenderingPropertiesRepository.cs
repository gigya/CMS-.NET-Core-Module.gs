namespace Sitecore.Gigya.Extensions.Repositories
{
    using Sitecore.Mvc.Presentation;

    public interface IRenderingPropertiesRepository
    {
        T Get<T>(Rendering rendering);
    }
}