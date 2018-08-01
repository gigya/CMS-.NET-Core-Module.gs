using Amazon.S3;
using Amazon.S3.Model;
using Gigya.Module.DeleteSync.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gigya.Module.DeleteSync
{
    public class DeleteSyncService : IDeleteSyncService
    {
        private readonly IDeleteSyncProvider _provider;

        public DeleteSyncService(IDeleteSyncProvider provider)
        {
            _provider = provider;
        }

        public Task<List<DeleteSyncFile>> GetUids(Dictionary<string, DeleteSyncLog> processedFiles)
        {
            return _provider.GetUids(processedFiles);
        }
    }
}
