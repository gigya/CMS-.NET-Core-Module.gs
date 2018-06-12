using Sitecore.Data.Items;

namespace Sitecore.Gigya.Module.Helpers
{
    public interface ISitecoreContentHelper
    {
        Item GetSettingsParent(Item current);
        Item GetFacetFolderParent(Item current);
    }
}