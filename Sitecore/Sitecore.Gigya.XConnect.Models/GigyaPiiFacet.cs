using Sitecore.XConnect;
using Sitecore.XConnect.Collection.Model;
using Sitecore.XConnect.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.Gigya.XConnect.Models
{
    [FacetKey(DefaultFacetKey)]
    [Serializable]
    [PIISensitive]
    public class GigyaPiiFacet : Facet
    {
        public const string DefaultFacetKey = "GigyaPii";

        public List<GigyaElement> Entries { get; set; }
    }

    [Serializable]
    public class GigyaPiiElement
    {
        public object Value { get; set; }
    }

    public class GigyaPiiModel
    {
        public static XdbModel Model { get; } = BuildModel();

        private static XdbModel BuildModel()
        {
            var builder = new XdbModelBuilder("xConnectIntroModel", new XdbModelVersion(1, 0));
            builder.ReferenceModel(CollectionModel.Model);
            builder.DefineFacet<Contact, GigyaPiiFacet>(GigyaPiiFacet.DefaultFacetKey);
            return builder.BuildModel();
        }
    }
}
