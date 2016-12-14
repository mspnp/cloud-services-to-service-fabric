namespace Tailspin.Web.Survey.Shared.Stores.Azure
{
    using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
    using Tailspin.Web.Survey.Shared.Helpers;

    public abstract class AzureObjectWithRetryPolicyFactory : IAzureObjectWithRetryPolicyFactory
    {
        public IRetryPolicyFactory RetryPolicyFactory { get; set; }

        public virtual IRetryPolicyFactory GetRetryPolicyFactoryInstance()
        {
            return this.RetryPolicyFactory ?? new DefaultRetryPolicyFactory();
        }

        protected virtual void RetryPolicyTrace(object sender, RetryingEventArgs args)
        {
            var msg = string.Format(
                 "Retry - Count:{0}, Delay:{1}, Exception:{2}",
                 args.CurrentRetryCount,
                 args.Delay,
                 args.LastException);
            TraceHelper.TraceInformation(msg);
        }
    }
}
