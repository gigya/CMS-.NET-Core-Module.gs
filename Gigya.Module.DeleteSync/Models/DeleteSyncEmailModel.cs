using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gigya.Module.DeleteSync.Models
{
    public class DeleteSyncEmailModel
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public string Domain { get; set; }
        public DateTime DateStarted { get; set; }
        public DateTime DateCompleted { get; set; }
        public List<string> ProcessedFilenames { get; set; } = new List<string>();
        public List<string> UpdatedUids { get; set; } = new List<string>();
        public List<string> DeletedUids { get; set; } = new List<string>();
        public List<string> FailedUpdatedUids { get; set; } = new List<string>();
        public List<string> FailedDeletedUids { get; set; } = new List<string>();
    }
}
