using Gigya.Module.Core.Connector.Logging;
using Gigya.Module.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gigya.Module.DeleteSync.Models
{
    public class DeleteSyncEventArgs : EventArgs
    {
        public string Uid { get; set; }
        public DeleteSyncAction Action { get; set; }
        public Guid CmsUid { get; set; }
        public bool ContinueWithDeletion { get; set; } = true;
    }
}
