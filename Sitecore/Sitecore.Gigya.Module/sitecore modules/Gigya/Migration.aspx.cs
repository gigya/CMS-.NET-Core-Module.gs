using Sitecore.Gigya.Migration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Sitecore.Gigya.Module.sitecore_modules.Gigya
{
    public partial class Migration : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Request.IsAuthenticated || !Sitecore.Context.User.IsAdministrator)
            {
                throw new UnauthorizedAccessException();
            }
        }

        protected void Migrate_Click(object sender, EventArgs e)
        {
            var migrator = new ModuleMigration();
            var response = migrator.DoIt();

            Messages.Text = response.Messages.ToString();
        }
    }
}