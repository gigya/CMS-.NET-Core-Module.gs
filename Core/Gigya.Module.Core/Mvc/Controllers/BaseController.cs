using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Gigya.Module.Core.Mvc.Controllers
{
    public abstract class BaseController : Controller
    {
        public JsonNetResult JsonNetResult(object data)
        {
            return new JsonNetResult { Data = data };
        }
    }
}
