using Gigya.Module.Core.Connector.Logging;
using Gigya.Module.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gigya.Module.DeleteSync.Models
{
    public class DeleteSyncUserDeletedEventArgs : EventArgs
    {
        public string Uid { get; set; }
        public DeleteSyncAction Action { get; set; }
        public Guid CmsUid { get; set; }
    }
}
