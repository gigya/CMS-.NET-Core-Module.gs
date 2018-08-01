using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Telerik.OpenAccess;
using Telerik.Sitefinity.Data.OA;
using System.Reflection;
using Telerik.OpenAccess.Metadata;
using Gigya.Module.DeleteSync.Models;

namespace Gigya.Sitefinity.Module.DeleteSync.Data
{
	public class GigyaDeleteSyncContext : SitefinityOAContext
    {
		public static GigyaDeleteSyncContext Get()
		{
            var context = OpenAccessConnection.GetContext(new GigyaMetaDataProvider(), "Sitefinity") as GigyaDeleteSyncContext;
            if (context.Cache != null)
            {
                context.Cache.ReleaseAll();
            }
            return context;
		}

		public GigyaDeleteSyncContext(string connectionString, BackendConfiguration backendConfig, Telerik.OpenAccess.Metadata.MetadataContainer metadataContainer)
			: base(connectionString, backendConfig, metadataContainer)
		{
		}

        protected override void Init(string connectionString, string cacheKey, BackendConfiguration backendConfiguration, MetadataContainer metadataContainer, Assembly callingAssembly)
        {
            base.Init(connectionString, cacheKey, backendConfiguration, metadataContainer, callingAssembly);

            // try and stop Sitefinity from caching everything
            this.LevelTwoCache.EvictAll<DeleteSyncSettings>();
        }

        /// <summary>
        /// Gets an IQueryable result of all DeleteSyncSettings.
        /// </summary>
        public IQueryable<DeleteSyncSettings> Settings
		{
			get 
            {
                return this.GetAll<DeleteSyncSettings>();
            }
		}

        /// <summary>
        /// Gets an IQueryable result of all DeleteSyncLog.
        /// </summary>
        public IQueryable<DeleteSyncLog> Logs
        {
            get
            {
                return this.GetAll<DeleteSyncLog>();
            }
        }

        static GigyaDeleteSyncContext()
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