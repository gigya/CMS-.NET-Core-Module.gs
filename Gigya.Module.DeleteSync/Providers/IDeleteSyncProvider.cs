using Gigya.Module.DeleteSync.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gigya.Module.DeleteSync
{
    public interface IDeleteSyncProvider
    {
        Task<List<DeleteSyncFile>> GetUids();
    }
}
