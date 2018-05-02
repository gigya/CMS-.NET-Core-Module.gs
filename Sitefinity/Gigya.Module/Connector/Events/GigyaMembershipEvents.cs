using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telerik.Sitefinity.Services.Events;

namespace Gigya.Module.Connector.Events
{
    public interface IMapGigyaFieldEvent : IEvent
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
        /// The Sitefinity field name being updated.
        /// </summary>
        string SitefinityFieldName { get; set; }
        /// <summary>
        /// Set this to the value that will be saved to Sitefinity. It is popuplated with the default value.
        /// </summary>
        object GigyaValue { get; set; }
    }

    public class MapGigyaFieldEvent : IMapGigyaFieldEvent
    {
        public dynamic GigyaModel { get; set; }
        public string SitefinityFieldName { get; set; }
        public string GigyaFieldName { get; set; }
        public object GigyaValue { get; set; }
        public string Origin { get; set; }
    }
}