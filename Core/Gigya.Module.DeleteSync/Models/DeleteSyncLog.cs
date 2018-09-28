using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gigya.Module.DeleteSync.Models
{
    public class DeleteSyncLog
    {
        public string Key { get; set; }
        public DateTime DateCreated { get; set; }
        public int Total { get; set; }
        public int Success { get; set; }
        public int Errors { get; set; }
    }
}
