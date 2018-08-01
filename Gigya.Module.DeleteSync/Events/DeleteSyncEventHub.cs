using Gigya.Module.DeleteSync.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gigya.Module.DeleteSync.Events
{
    public class DeleteSyncEventHub
    {
        private static readonly Lazy<DeleteSyncEventHub> _instance = new Lazy<DeleteSyncEventHub>(() => new DeleteSyncEventHub());

        /// <summary>
        /// Called when deleting a user from the CMS.
        /// </summary>
        public event EventHandler<DeleteSyncEventArgs> DeletingUser;

        /// <summary>
        /// Called when a user is deleted from the CMS.
        /// </summary>
        public event EventHandler<DeleteSyncUserDeletedEventArgs> DeletedUser;

        public void RaiseDeletingUser(object sender, DeleteSyncEventArgs e)
        {
            DeletingUser?.Invoke(sender, e);
        }

        public void RaiseDeletedUser(object sender, DeleteSyncUserDeletedEventArgs e)
        {
            DeletedUser?.Invoke(sender, e);
        }

        private DeleteSyncEventHub()
        {
        }

        public static DeleteSyncEventHub Instance { get { return _instance.Value; } }
    }
}
