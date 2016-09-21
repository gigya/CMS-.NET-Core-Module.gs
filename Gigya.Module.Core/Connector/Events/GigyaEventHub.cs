using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gigya.Module.Core.Connector.Events
{
    public sealed class GigyaEventHub
    {
        private static readonly Lazy<GigyaEventHub> _instance = new Lazy<GigyaEventHub>(() => new GigyaEventHub());

        public event EventHandler<GetAccountInfoCompletedEventArgs> GetAccountInfoCompleted;
        public event EventHandler<AccountInfoMergeCompletedEventArgs> AccountInfoMergeCompleted;

        public void RaiseGetAccountInfoCompleted(object sender, GetAccountInfoCompletedEventArgs e)
        {
            GetAccountInfoCompleted?.Invoke(sender, e);
        }

        public void RaiseAccountInfoMergeCompleted(object sender, AccountInfoMergeCompletedEventArgs e)
        {
            AccountInfoMergeCompleted?.Invoke(sender, e);
        }

        private GigyaEventHub()
        {
        }

        public static GigyaEventHub Instance { get { return _instance.Value; } }
    }
}
