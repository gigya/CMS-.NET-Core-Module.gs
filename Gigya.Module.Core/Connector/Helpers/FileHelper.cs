using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace Gigya.Module.Core.Connector.Helpers
{
    public static class FileHelper
    {
        private static Dictionary<string, bool> _checkedPaths = new Dictionary<string, bool>();

        /// <summary>
        /// Gets a path to a file. If <paramref name="localPath"/> exists, it is given priority of the <paramref name="fallbackPath"/>.
        /// </summary>
        public static string GetPath(string localPath, string fallbackPath)
        {
            // faster to maintain an in-memory cache rather than checking the file system each time
            bool exists = false;
            if (_checkedPaths.TryGetValue(localPath, out exists))
            {
                return exists ? localPath : fallbackPath;
            }
            
            var path = fallbackPath;
            var mappedPath = HostingEnvironment.MapPath(localPath);
            if (File.Exists(mappedPath))
            {
                exists = true;
                path = localPath;
            }

            try
            {
                // might fail if 2 requests come in at the same time
                _checkedPaths.Add(localPath, exists);
            }
            catch { }

            return path;
        }
    }
}