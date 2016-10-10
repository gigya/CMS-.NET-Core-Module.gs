using Gigya.Module.Core.Connector.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Gigya.Umbraco.Module.Data
{
    /// <summary>
    /// Ideally this class wouldn't be required as we could just use GigyaModuleSettings but unfortunately there isn't a way to specify the Db type.
    /// The IGigyaModuleSettings interface Id property needs to be an object as some CMS' use Guid's as the primary key.
    /// </summary>
    [TableName("gigya_settings")]
    public class GigyaUmbracoModuleSettings
    {
        [PrimaryKeyColumn(AutoIncrement = false, Name = "PK_gigya_settings")]
        public int Id { get; set; }

        [Length(4000)]
        public string ApiKey { get; set; }

        [Length(4000)]
        public string ApplicationKey { get; set; }

        [Length(4000)]
        public string ApplicationSecret { get; set; }

        [Length(20)]
        public string Language { get; set; }

        [NullSetting(NullSetting = NullSettings.Null)]
        [Length(20)]
        public string LanguageFallback { get; set; }

        public bool DebugMode { get; set; }

        [Length(20)]
        public string DataCenter { get; set; }
        
        public bool EnableRaas { get; set; }

        [NullSetting(NullSetting = NullSettings.Null)]
        [Length(2000)]
        public string RedirectUrl { get; set; }

        [NullSetting(NullSetting = NullSettings.Null)]
        [Length(2000)]
        public string LogoutUrl { get; set; }

        [NullSetting(NullSetting = NullSettings.Null)]
        [Length(4000)]
        public string MappingFields { get; set; }

        [NullSetting(NullSetting = NullSettings.Null)]
        [Length(4000)]
        public string GlobalParameters { get; set; }

        public int SessionTimeout { get; set; }
        
        public GigyaSessionProvider SessionProvider { get; set; }

        [Ignore]
        public bool IsNew { get; set; }
    }
}
