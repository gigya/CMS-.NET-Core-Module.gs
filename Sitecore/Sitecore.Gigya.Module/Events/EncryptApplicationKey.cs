using Gigya.Module.Core.Connector.Logging;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Gigya.Module.Encryption;
using Sitecore.Gigya.Module.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Web;

namespace Sitecore.Gigya.Module.Events
{
    public class EncryptApplicationKey
    {
        private static readonly ConcurrentDictionary<Data.ID, bool> _inProcess = new ConcurrentDictionary<Data.ID, bool>();

        public void OnItemSaving(object sender, EventArgs args)
        {
            var logger = new Logger(new SitecoreLogger());
            if (Context.ContentDatabase == null)
            {
                logger.Debug("ContentDatabase is null so aborting EncryptApplicationKey.OnItemSaving");
                return;
            }

            var eventArgs = args as Sitecore.Events.SitecoreEventArgs;
            Assert.IsNotNull(eventArgs, "eventArgs");

            Item updatedItem = eventArgs.Parameters[0] as Item;
            Assert.IsNotNull(updatedItem, "item");
            
            if (updatedItem.Database == null || updatedItem.Database.Name != Context.ContentDatabase.Name)
            {
                logger.Debug("Updated Database is null so aborting EncryptApplicationKey.OnItemSaving");
                return;
            }

            if (updatedItem.TemplateID != Constants.Templates.GigyaSettings)
            {
                return;
            }
            
            var secretKey = updatedItem.Fields[Constants.Fields.ApplicationSecret]?.Value;
            if (string.IsNullOrEmpty(secretKey))
            {
                return;
            }

            if (_inProcess.ContainsKey(updatedItem.ID))
            {
                return;
            }

            // avoid a loop
            _inProcess.TryAdd(updatedItem.ID, true);

            try
            {
                updatedItem.Fields[Constants.Fields.ApplicationSecret].Value = SitecoreEncryptionService.Instance.Encrypt(secretKey);
            }
            finally
            {
                bool removedValue;
                _inProcess.TryRemove(updatedItem.ID, out removedValue);
            }
        }
    }
}