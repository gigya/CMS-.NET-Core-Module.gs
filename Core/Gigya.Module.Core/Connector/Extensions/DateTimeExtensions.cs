using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gigya.Module.Core.Connector.Extensions
{
    public static class DateTimeExtensions
    {
        public static long DateTimeToUnixTimestamp(this DateTime dateTime)
        {
            var ms = (TimeZoneInfo.ConvertTimeToUtc(dateTime) -
                   new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalMilliseconds;

            // convert back to seconds so we don't get 12345.45 seconds
            return (long)ms / 1000L;
        }
    }
}
