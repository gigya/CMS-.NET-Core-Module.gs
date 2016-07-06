using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Sitefinity.Localization;

namespace Gigya.Module.Mvc.Models
{
    public class LoginModel : RequestModel
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string Signature { get; set; }
        [Required]
        public string SignatureTimestamp { get; set; }
    }

    public class RequestModel
    {
        [Required]
        public Guid SiteId { get; set; }
    }

    public class LoginResponseModel : ResponseModel
    {
    }

    public class ResponseModel
    {
        public string RedirectUrl { get; set; }
        public string ErrorMessage { get; set; }
        public ResponseStatus Status { get; set; }
    }

    public enum ResponseStatus
    {
        Error = 0,
        Success = 1,
        AlreadyLoggedIn = 2
    }
}
