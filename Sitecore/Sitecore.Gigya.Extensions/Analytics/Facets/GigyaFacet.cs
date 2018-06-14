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
        private const string _name = "Entries";

        public IElementDictionary<IGigyaElement> Entries
        {
            get
            {
                return this.GetDictionary<IGigyaElement>(_name);
            }
        }

        public GigyaFacet()
        {
            this.EnsureDictionary<IGigyaElement>(_name);
        }
    }
}