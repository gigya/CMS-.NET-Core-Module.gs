using Sitecore.Analytics.Model.Framework;
using Sitecore.Gigya.Extensions.Abstractions.Analytics.Facets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Gigya.Extensions.Analytics.Facets
{
    [Serializable]
    public class GigyaFacet : Facet, IGigyaFacet
    {
        private const string _fields = "Fields";

        public IElementCollection<IGigyaElement> Fields
        {
            get
            {
                return this.GetCollection<IGigyaElement>(_fields);
            }
        }

        public GigyaFacet()
        {
            this.EnsureAttribute<IGigyaElement>(_fields);
        }
    }
}