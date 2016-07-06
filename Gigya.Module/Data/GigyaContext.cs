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

namespace Gigya.Module.Data
{
	public class GigyaContext : SitefinityOAContext
    {
		public static GigyaContext Get()
		{
            var context = OpenAccessConnection.GetContext(new GigyaMetaDataProvider(), "Sitefinity") as GigyaContext;
            if (context.Cache != null)
            {
                context.Cache.ReleaseAll();
            }
            return context;
		}

		public GigyaContext(string connectionString, BackendConfiguration backendConfig, Telerik.OpenAccess.Metadata.MetadataContainer metadataContainer)
			: base(connectionString, backendConfig, metadataContainer)
		{
		}

        protected override void Init(string connectionString, string cacheKey, BackendConfiguration backendConfiguration, MetadataContainer metadataContainer, Assembly callingAssembly)
        {
            base.Init(connectionString, cacheKey, backendConfiguration, metadataContainer, callingAssembly);

            // try and stop Sitefinity from caching everything
            this.LevelTwoCache.EvictAll<GigyaModuleSettings>();
        }

        /// <summary>
        /// Gets an IQueryable result of all gigya settings.
        /// </summary>
        public IQueryable<GigyaModuleSettings> Settings
		{
			get 
            {
                return this.GetAll<GigyaModuleSettings>();
            }
		}

        /// <summary>
        /// Gets an IQueryable result of all gigya languages.
        /// </summary>
        public IQueryable<GigyaLanguage> Languages
        {
            get
            {
                return this.GetAll<GigyaLanguage>();
            }
        }

        static GigyaContext()
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