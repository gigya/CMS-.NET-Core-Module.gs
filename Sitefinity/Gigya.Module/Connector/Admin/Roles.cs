using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.Security;
using Telerik.Sitefinity.Security.Claims;

namespace Gigya.Module.Connector.Admin
{
    public class Roles
    {
        /// <summary>
        /// Creates the Gigya-Admin role which is used to determine if the Sitefinity user should see the masked Application Secret.
        /// </summary>
        public static void CreateRole()
        {
            var roleManager = RoleManager.GetManager();

            using (new ElevatedModeRegion(roleManager))
            {
                if (!roleManager.GetRoles().Where(r => r.Name == Constants.Roles.GigyaAdmin).Any())
                {
                    roleManager.CreateRole(Constants.Roles.GigyaAdmin);
                }

                roleManager.SaveChanges();
            }
        }

        /// <summary>
        /// Removes the Gigya-Admin role.
        /// </summary>
        public static void RemoveRole()
        {
            var roleManager = RoleManager.GetManager();

            using (new ElevatedModeRegion(roleManager))
            {
                var role = roleManager.GetRoles().Where(r => r.Name == Constants.Roles.GigyaAdmin).FirstOrDefault();
                if (role != null)
                {
                    roleManager.Delete(role);
                }

                roleManager.SaveChanges();
            }
        }

        /// <summary>
        /// Checks if the Sitefinity user with id of <paramref name="userId"/> has the Gigya-Admin role.
        /// </summary>
        /// <param name="userId">The id of the user to check.</param>
        /// <returns>True if the user has the required Gigya-Admin role.</returns>
        public static bool HasRole(ClaimsIdentityProxy identity)
        {
            if (identity.IsUnrestricted)
            {
                return true;
            }

            var roleManager = RoleManager.GetManager();

            var roles = roleManager.GetRolesForUser(identity.UserId).ToList();
            var allRoles = roleManager.GetRoles().ToList();

            return roleManager.IsUserInRole(identity.UserId, Constants.Roles.GigyaAdmin);
        }
    }
}
