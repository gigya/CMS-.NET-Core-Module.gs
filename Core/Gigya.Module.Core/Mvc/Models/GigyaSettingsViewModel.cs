﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Gigya.Module.Core.Mvc.Models
{
    public class GigyaSettingsViewModel
    {
        public object Id { get; set; }
        public string ApiKey { get; set; }
        public string DataCenter { get; set; }
        public dynamic Settings { get; set; }
        public HtmlString SettingsJson { get; set; }
        public bool DebugMode { get; set; }
        public bool RenderScript { get; set; }
        public string GigyaScriptPath { get; set; }
        public string ErrorMessage { get; set; }
        public string LoggedInRedirectUrl { get; set; }
        public string LogoutUrl { get; set; }
        public bool IsLoggedIn { get; set; }
        public bool IsGetInfoRequired { get; set; }
        public bool EnableSSOToken { get; set; }
        public bool SyncSSOGroup { get; set; }
    }
}
