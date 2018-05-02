using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gigya.Umbraco.Module.v621.Mvc.Models
{
    public class GigyaConfigModel
    {
        public List<GigyaLanguageModel> LanguageOptions { get; set; }
        public List<GigyaLanguageModel> Languages { get; set; }
    }
}
