using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telerik.OpenAccess.Metadata;
using Telerik.OpenAccess.Metadata.Fluent;

namespace Gigya.Sitefinity.Module.DS.Data
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
			mappings.Add(MapGigyaDsSettingsTable());
            mappings.Add(MapGigyaDsMappingsTable());
            return mappings;
		}

		/// <summary>
		/// Maps the GigyaSetting class to a database table.
		/// </summary>
		/// <returns></returns>
		private MappingConfiguration<GigyaSitefinityModuleDsSettings> MapGigyaDsSettingsTable()
		{
			// map to table
			var tableMapping = new MappingConfiguration<GigyaSitefinityModuleDsSettings>();
			tableMapping.MapType().ToTable("gigya_ds_settings");

            // map properties
            tableMapping.HasProperty(t => t.SiteId).IsIdentity();
            tableMapping.HasProperty(t => t.Method).IsNotNullable();

            return tableMapping;
		}

        /// <summary>
		/// Maps the GigyaSetting class to a database table.
		/// </summary>
		/// <returns></returns>
		private MappingConfiguration<GigyaSitefinityDsMapping> MapGigyaDsMappingsTable()
        {
            // map to table
            var tableMapping = new MappingConfiguration<GigyaSitefinityDsMapping>();
            tableMapping.MapType().ToTable("gigya_ds_mapping");

            // map properties
            tableMapping.HasProperty(t => t.Id).IsIdentity(KeyGenerator.Autoinc);
            tableMapping.HasProperty(t => t.CmsName).IsNotNullable();
            tableMapping.HasProperty(t => t.DsSettingId).IsNotNullable();
            tableMapping.HasProperty(t => t.GigyaName).IsNotNullable();
            tableMapping.HasProperty(t => t.Oid).IsNotNullable();
            tableMapping.HasAssociation(t => t.Settings)
                .WithOpposite(t => t.Mappings)
                .HasConstraint((m, s) => m.DsSettingId == s.SiteId);

            return tableMapping;
        }
    }
}