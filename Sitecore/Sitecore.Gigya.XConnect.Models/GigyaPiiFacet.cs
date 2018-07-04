using Sitecore.XConnect;
using Sitecore.XConnect.Collection.Model;
using Sitecore.XConnect.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using A = Sitecore.Gigya.Extensions.Abstractions.Analytics;

namespace Sitecore.Gigya.XConnect.Models
{
    [FacetKey(DefaultFacetKey)]
    [Serializable]
    [PIISensitive]
    public class GigyaPiiFacet : GigyaXConnectFacet
    {
        public const string DefaultFacetKey = A.Constants.FacetKeys.GigyaPii;
    }
}
