using Sitecore.Analytics.Model.Framework;
using Sitecore.Gigya.Extensions.Abstractions.Analytics.Facets;
using Sitecore.XConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Gigya.Extensions.Analytics.Facets
{
    [Serializable]
    [FacetKey(DefaultFacetKey)]
    public class GigyaFacet : Sitecore.XConnect.Facet
    {
        public const string DefaultFacetKey = "Gigya";

        public GigyaFacet()
        {

        }

        //private const string _name = "Entries";

        //public IElementDictionary<IGigyaElement> Entries
        //{
        //    get
        //    {
        //        return this.GetDictionary<IGigyaElement>(_name);
        //    }
        //}

        //public GigyaFacet()
        //{
        //    this.EnsureDictionary<IGigyaElement>(_name);
        //}
    }
}