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

        /// <summary>
        /// Called after getAccountInfo data has been retrieved.
        /// </summary>
        public event EventHandler<GetAccountInfoCompletedEventArgs> GetAccountInfoCompleted;
        /// <summary>
        /// Called after DS data has been merged with getAccountInfo.
        /// </summary>
        public event EventHandler<AccountInfoMergeCompletedEventArgs> AccountInfoMergeCompleted;
        /// <summary>
        /// Called after DS data is retrieved.
        /// </summary>
        public event EventHandler<FetchDSCompletedEventArgs> FetchDSCompleted;
        /// <summary>
        /// Called when retrieving an individual field for mapping to the CMS.
        /// </summary>
        public event EventHandler<MapGigyaFieldEventArgs> GettingGigyaValue;

        public void RaiseGettingGigyaValue(object sender, MapGigyaFieldEventArgs e)
        {
            GettingGigyaValue?.Invoke(sender, e);
        }

        public void RaiseGetAccountInfoCompleted(object sender, GetAccountInfoCompletedEventArgs e)
        {
            GetAccountInfoCompleted?.Invoke(sender, e);
        }

        public void RaiseAccountInfoMergeCompleted(object sender, AccountInfoMergeCompletedEventArgs e)
        {
            AccountInfoMergeCompleted?.Invoke(sender, e);
        }

        public void RaiseFetchDSCompleted(object sender, FetchDSCompletedEventArgs e)
        {
            FetchDSCompleted?.Invoke(sender, e);
        }

        private GigyaEventHub()
        {
        }

        public static GigyaEventHub Instance { get { return _instance.Value; } }
    }
}
