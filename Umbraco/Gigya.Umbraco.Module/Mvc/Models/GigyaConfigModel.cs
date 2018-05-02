using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gigya.Umbraco.Module.Mvc.Models
{
    public class GigyaConfigModel
    {
        public List<GigyaLanguageModel> LanguageOptions { get; set; }
        public List<GigyaLanguageModel> Languages { get; set; }
        public List<GigyaMemberPropertyViewModel> MemberProperties { get; set; }
    }

    public class GigyaMemberPropertyViewModel
    {
        public string Alias { get; set; }
        public string Name { get; set; }
    }
}
