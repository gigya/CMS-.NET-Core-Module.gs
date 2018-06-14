using Sitecore.Analytics.Model.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.Gigya.Extensions.Abstractions.Analytics.Facets
{
    public interface IGigyaElement : IElement
    {
        object Value { get; set; }
    }
}
