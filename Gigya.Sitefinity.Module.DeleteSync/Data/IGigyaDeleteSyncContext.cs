using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gigya.Sitefinity.Module.DeleteSync.Data
{
    public interface IGigyaDeleteSyncContext : IDisposable
    {
        void Add(object entity);
        void SaveChanges();
    }
}
