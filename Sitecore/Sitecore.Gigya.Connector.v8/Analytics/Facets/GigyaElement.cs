using Sitecore.Analytics.Model.Framework;
using Sitecore.Gigya.Extensions.Abstractions.Analytics.Facets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Gigya.Connector.Analytics.Facets
{
    [Serializable]
    public class GigyaElement : Element, IGigyaElement
    {
        private const string _value = "Value";

        public GigyaElement()
        {
            EnsureAttribute<object>(_value);
        }

        public object Value
        {
            get
            {
                return GetAttribute<object>(_value);
            }
            set
            {
                SetAttribute(_value, value);
            }
        }
    }
}