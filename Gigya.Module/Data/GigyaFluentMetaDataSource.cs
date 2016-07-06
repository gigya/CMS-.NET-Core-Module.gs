using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telerik.OpenAccess.Metadata;
using Telerik.OpenAccess.Metadata.Fluent;

namespace Gigya.Module.Data
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
            mappings.Add(MapGigyaLanguagesTable());
			return mappings;
		}

		/// <summary>
		/// Maps the GigyaSetting class to a database table.
		/// </summary>
		/// <returns></returns>
		private MappingConfiguration<GigyaModuleSettings> MapGigyaSettingsTable()
		{
			// map to table
			var tableMapping = new MappingConfiguration<GigyaModuleSettings>();
			tableMapping.MapType().ToTable("sf_gigya_settings");

            // map properties
            tableMapping.HasProperty(t => t.SiteId).IsIdentity();
			tableMapping.HasProperty(t => t.GlobalParameters).HasColumnType("varchar(max)");
            tableMapping.HasProperty(t => t.MappingFields).HasColumnType("varchar(max)");
            tableMapping.HasProperty(t => t.Language).HasLength(20);
            tableMapping.HasProperty(t => t.LanguageFallback).HasLength(20);
            tableMapping.HasProperty(t => t.DataCenter).HasLength(20);

            return tableMapping;
		}

        /// <summary>
		/// Maps the GigyaSetting class to a database table.
		/// </summary>
		/// <returns></returns>
		private MappingConfiguration<GigyaLanguage> MapGigyaLanguagesTable()
        {
            // map to table
            var tableMapping = new MappingConfiguration<GigyaLanguage>();
            tableMapping.MapType().ToTable("sf_gigya_languages");

            // map properties
            tableMapping.HasProperty(t => t.Language).IsIdentity().HasLength(20);

            return tableMapping;
        }
    }
}