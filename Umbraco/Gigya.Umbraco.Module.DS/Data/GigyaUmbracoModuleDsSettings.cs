using Gigya.Module.DS.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Gigya.Umbraco.Module.DS.Data
{
    [TableName("gigya_ds_settings")]
    public class GigyaUmbracoModuleDsSettings
    {
        [PrimaryKeyColumn(AutoIncrement = false, Name = "PK_gigya_ds_settings")]
        public int Id { get; set; }
        public int Method { get; set; }

        [Ignore]
        public bool IsNew { get; set; }

        [Ignore]
        public List<GigyaUmbracoDsMapping> Mappings { get; set; }
    }

    [TableName("gigya_ds_mapping")]
    [PrimaryKey("Id", autoIncrement = true)]
    public class GigyaUmbracoDsMapping
    {
        [PrimaryKeyColumn(AutoIncrement = true, Name = "PK_gigya_ds_mapping")]
        public int Id { get; set; }
        [Index(IndexTypes.NonClustered, Name = "IX_DsSettingId")]
        [ForeignKey(typeof(GigyaUmbracoModuleDsSettings), Column = "Id", Name = "FK_gigya_ds_mapping_DsSettingId")]
        public int DsSettingId { get; set; }
        public string CmsName { get; set; }
        public string GigyaName { get; set; }
        public string Oid { get; set; }
    }
}
