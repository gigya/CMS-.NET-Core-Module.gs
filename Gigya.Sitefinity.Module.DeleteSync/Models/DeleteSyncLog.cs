using Gigya.Module.DeleteSync.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gigya.Sitefinity.Module.DeleteSync.Models
{
    /// <summary>
    /// SitefinityDeleteSyncLog
    /// </summary>
    /// <remarks>Annoyingly Telerik's OpenAccess doesn't support inheritance.</remarks>
    public class SitefinityDeleteSyncLog// : DeleteSyncLog
    {
        public string Key { get; set; }
        public DateTime DateCreated { get; set; }
        public int Total { get; set; }
        public int Success { get; set; }
        public int Errors { get; set; }
    }
}
