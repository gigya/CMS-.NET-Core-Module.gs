using Gigya.Module.Connector.Admin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Telerik.OpenAccess;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Data.Configuration;
using Telerik.Sitefinity.Data.OA;
using System.Reflection;
using Telerik.OpenAccess.Metadata;

namespace Gigya.Sitefinity.Module.DS.Data
{
	public class GigyaDSContext : SitefinityOAContext
    {
		public static GigyaDSContext Get()
		{
            var context = OpenAccessConnection.GetContext(new GigyaMetaDataProvider(), "Sitefinity") as GigyaDSContext;
            if (context.Cache != null)
            {
                context.Cache.ReleaseAll();
            }
            return context;
		}

		public GigyaDSContext(string connectionString, BackendConfiguration backendConfig, Telerik.OpenAccess.Metadata.MetadataContainer metadataContainer)
			: base(connectionString, backendConfig, metadataContainer)
		{
		}

        protected override void Init(string connectionString, string cacheKey, BackendConfiguration backendConfiguration, MetadataContainer metadataContainer, Assembly callingAssembly)
        {
            base.Init(connectionString, cacheKey, backendConfiguration, metadataContainer, callingAssembly);

            // try and stop Sitefinity from caching everything
            this.LevelTwoCache.EvictAll<GigyaSitefinityModuleDsSettings>();
        }

        /// <summary>
        /// Gets an IQueryable result of all gigya settings.
        /// </summary>
        public IQueryable<GigyaSitefinityModuleDsSettings> Settings
		{
			get 
            {
                return this.GetAll<GigyaSitefinityModuleDsSettings>();
            }
		}

        /// <summary>
        /// Gets an IQueryable result of all gigya ds mappings.
        /// </summary>
        public IQueryable<GigyaSitefinityDsMapping> Mappings
        {
            get
            {
                return this.GetAll<GigyaSitefinityDsMapping>();
            }
        }

        static GigyaDSContext()
        {
            MigrateToLatest();
        }

        public static void MigrateToLatest()
        {
            // check context for db
            using (var context = Get())
            {
                var schemaHandler = context.GetSchemaHandler();
                string script = null;

                // check if db needs creating or updating
                if (schemaHandler.DatabaseExists())
                {
                    script = schemaHandler.CreateUpdateDDLScript(null);
                }
                else
                {
                    schemaHandler.CreateDatabase();
                    script = schemaHandler.CreateDDLScript();
                }

                // execute script to create or update database
                if (!string.IsNullOrEmpty(script))
                {
                    schemaHandler.ForceExecuteDDLScript(script);
                }
            }
        }
	}
}