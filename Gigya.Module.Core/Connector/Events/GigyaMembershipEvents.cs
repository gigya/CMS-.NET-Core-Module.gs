using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gigya.Module.Core.Connector.Events
{
    public interface IMapGigyaFieldEvent
    {
        /// <summary>
        /// Deserialized Gigya JSON object.
        /// </summary>
        dynamic GigyaModel { get; set; }
        /// <summary>
        /// Gigya field name e.g. profile.age
        /// </summary>
        string GigyaFieldName { get; set; }
        /// <summary>
        /// The CMS field name being updated.
        /// </summary>
        string CmsFieldName { get; set; }
        /// <summary>
        /// Set this to the value that will be saved to the CMS. It is popuplated with the default value.
        /// </summary>
        object GigyaValue { get; set; }
    }

    public class MapGigyaFieldEventArgs : EventArgs, IMapGigyaFieldEvent
    {
        public dynamic GigyaModel { get; set; }
        public string CmsFieldName { get; set; }
        public string GigyaFieldName { get; set; }
        public object GigyaValue { get; set; }
        public string Origin { get; set; }
    }
}