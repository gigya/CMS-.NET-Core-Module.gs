using Sitecore.Analytics.Model.Framework;
using Sitecore.Gigya.Extensions.Abstractions.Analytics.Facets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Gigya.Extensions.Analytics.Facets
{
    [Serializable]
    public class GigyaElement : Element, IGigyaElement
    {
        private const string _field = "Field";
        private const string _value = "Value";

        public GigyaElement()
        {
            EnsureAttribute<string>(_field);
            EnsureAttribute<string>(_value);
        }

        public string Field
        {
            get
            {
                return GetAttribute<string>(_field);
            }
            set
            {
                SetAttribute(_field, value);
            }
        }

        public string Value
        {
            get
            {
                return GetAttribute<string>(_value);
            }
            set
            {
                SetAttribute(_value, value);
            }
        }
    }
}