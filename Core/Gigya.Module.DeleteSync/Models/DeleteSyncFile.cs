using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gigya.Module.DeleteSync.Models
{
    public class DeleteSyncFile
    {
        public string Key { get; set; }
        public List<string> UIDs { get; set; }
    }
}
