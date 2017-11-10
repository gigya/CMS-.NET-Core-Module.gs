using Gigya.Module.Connector.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gigya.Module.HttpModules
{
    public class GigyaRequestModule : IHttpModule
    {
        public void Dispose()
        {
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += Context_BeginRequest;
        }

        private void Context_BeginRequest(object sender, EventArgs e)
        {
            var context = HttpContext.Current;
            var request = new HttpRequestWrapper(context.Request);

            if (!request.IsAjaxRequest())
            {
                // currently we only care about ajax requests since full page loads will go through the normal CMS pipeline
                return;
            }
            
            GigyaAccountHelper.ProcessRequestChecks(context);
        }
    }
}