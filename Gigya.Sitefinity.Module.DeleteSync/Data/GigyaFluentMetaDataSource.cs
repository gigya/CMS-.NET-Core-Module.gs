using Gigya.Module.DeleteSync.Models;
using Gigya.Sitefinity.Module.DeleteSync.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telerik.OpenAccess.Metadata;
using Telerik.OpenAccess.Metadata.Fluent;

namespace Gigya.Sitefinity.Module.DeleteSync.Data
{
	public class GigyaFluentMetaDataSource : FluentMetadataSource
	{
        /// <summary>
        /// Called when this context instance is initializing and a model needs to be obtained.
        /// </summary>
        /// <returns></returns>
        protected override IList<MappingConfiguration> PrepareMapping()
		{
			var mappings = new List<MappingConfiguration>();
			mappings.Add(MapGigyaSettingsTable());
            mappings.Add(MapGigyaDeleteSyncLogTable());
            return mappings;
		}

		/// <summary>
		/// Maps the GigyaSetting class to a database table.
		/// </summary>
		/// <returns></returns>
		private MappingConfiguration<SitefinityDeleteSyncSettings> MapGigyaSettingsTable()
		{
			// map to table
			var tableMapping = new MappingConfiguration<SitefinityDeleteSyncSettings>();
			tableMapping.MapType().ToTable("sf_gigya_delete_sync_settings");

            // map properties
            tableMapping.HasProperty(t => t.Id).IsIdentity();
			tableMapping.HasProperty(t => t.EmailsOnFailure).HasColumnType("varchar(max)");
            tableMapping.HasProperty(t => t.EmailsOnSuccess).HasColumnType("varchar(max)");
            tableMapping.HasProperty(t => t.S3AccessKey).HasLength(200);
            tableMapping.HasProperty(t => t.S3BucketName).HasLength(200);
            tableMapping.HasProperty(t => t.S3ObjectKeyPrefix).HasLength(200);
            tableMapping.HasProperty(t => t.S3SecretKey).HasLength(200);

            return tableMapping;
		}

        /// <summary>
		/// Maps the GigyaSetting class to a database table.
		/// </summary>
		/// <returns></returns>
		private MappingConfiguration<SitefinityDeleteSyncLog> MapGigyaDeleteSyncLogTable()
        {
            // map to table
            var tableMapping = new MappingConfiguration<SitefinityDeleteSyncLog>();
            tableMapping.MapType().ToTable("sf_gigya_delete_sync_logs");

            // map properties
            tableMapping.HasProperty(t => t.Key).IsIdentity();

            return tableMapping;
        }
    }
}