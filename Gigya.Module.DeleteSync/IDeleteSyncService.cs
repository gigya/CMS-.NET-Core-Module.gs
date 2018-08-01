using Gigya.Module.DeleteSync.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gigya.Module.DeleteSync
{
    public interface IDeleteSyncService
    {
        Task<List<DeleteSyncFile>> GetUids(Dictionary<string, DeleteSyncLog> processedFiles);
    }
}
