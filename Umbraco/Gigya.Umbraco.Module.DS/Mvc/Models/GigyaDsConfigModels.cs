using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gigya.Umbraco.Module.DS.Mvc.Models
{
    public class GigyaDsConfigViewModel
    {
        public List<GigyaDsMemberPropertyViewModel> MemberProperties { get; set; }
    }

    public class GigyaDsMemberPropertyViewModel
    {
        public string Alias { get; set; }
        public string Name { get; set; }
    }
}
