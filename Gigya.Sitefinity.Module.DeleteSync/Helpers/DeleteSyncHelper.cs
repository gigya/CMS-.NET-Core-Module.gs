using Gigya.Module.DeleteSync.Models;
using Gigya.Sitefinity.Module.DeleteSync.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Sitefinity.Security;
using Telerik.Sitefinity.Security.Model;

namespace Gigya.Sitefinity.Module.DeleteSync.Helpers
{
    public class DeleteSyncHelper
    {
        private readonly UserManager _userManager = UserManager.GetManager();
        private readonly UserProfileManager _profileManager = UserProfileManager.GetManager();

        public void Process(DeleteSyncSettings settings, List<DeleteSyncFile> files)
        {
            // logging etc

            // call hooks

            foreach (var file in files)
            {
                foreach (var id in file.UIDs)
                {
                    switch (settings.Action)
                    {
                        case DeleteSyncAction.FullUserDeletion:
                            DeleteUser(id);
                            break;
                        case DeleteSyncAction.DeleteNotification:
                            MarkUserAsDeleted(id);
                            break;
                    }
                }
            }
        }

        private void MarkUserAsDeleted(string username)
        {
            throw new NotImplementedException();
        }

        private void DeleteUser(string username)
        {
            var user = _userManager.GetUser(username);
            if (user == null)
            {
                return;
            }

            SitefinityProfile userProfile = _profileManager.GetUserProfile<SitefinityProfile>(user);

            if (userProfile != null)
            {
                _profileManager.Delete(userProfile);
            }

            _userManager.Delete(user);

            _profileManager.SaveChanges();
            _userManager.SaveChanges();
        }
    }
}
