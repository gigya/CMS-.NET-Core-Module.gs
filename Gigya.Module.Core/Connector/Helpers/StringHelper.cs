using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gigya.Module.Core.Connector.Helpers
{
    public static class StringHelper
    {
        public static string MaskInput(string input, string maskingChar, int firstChars, int lastChars)
        {
            if (string.IsNullOrEmpty(input) || input.Length < (firstChars + lastChars))
            {
                return null;
            }

            var masked = new StringBuilder(string.Concat(Enumerable.Repeat(maskingChar, input.Length)));
            masked = masked.Remove(0, firstChars);
            masked = masked.Remove(masked.Length - lastChars, lastChars);
            masked.Insert(0, input.Substring(0, firstChars));
            masked.Append(input.Substring(input.Length - lastChars));

            return masked.ToString();
        }
    }
}
