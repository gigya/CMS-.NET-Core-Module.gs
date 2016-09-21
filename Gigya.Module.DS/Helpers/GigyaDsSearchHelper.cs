using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gigya.Module.DS.Helpers
{
    public class GigyaDsSearchHelper
    {
        public static string BuildQuery(string uid, string dsType, IEnumerable<string> fields = null)
        {
            var builder = new StringBuilder();

            var fieldValue = fields != null && fields.Any() ? string.Join(",", fields.Select(PrefixField)) : "*";
            builder.AppendFormat("SELECT {0} FROM {1} WHERE UID = '{2}'", fieldValue, dsType, uid);
            return builder.ToString();
        }

        private static string PrefixField(string field)
        {
            return field == "uid" ? field: string.Concat("data.", field);
        }
    }
}
