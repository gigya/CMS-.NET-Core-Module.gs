using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telerik.OpenAccess.Metadata;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.Data.OA;
using Telerik.Sitefinity.Model;

namespace Gigya.Module.Data
{
	public class GigyaMetaDataProvider : IOpenAccessMetadataProvider, IOpenAccessCustomContextProvider
	{
		public MetadataSource GetMetaDataSource(IDatabaseMappingContext context)
		{
			return new GigyaFluentMetaDataSource();
		}

		public SitefinityOAContext GetContext(string connectionString, Telerik.OpenAccess.BackendConfiguration backendConfig, MetadataContainer metadataContainer)
		{
            backendConfig.SecondLevelCache.Enabled = false;
            backendConfig.SecondLevelCache.CacheQueryResults = false;

			return new GigyaContext(connectionString, backendConfig, metadataContainer);
		}

		public string ModuleName
		{
			get { return ModuleClass.ModuleName; }
		}
	}
}