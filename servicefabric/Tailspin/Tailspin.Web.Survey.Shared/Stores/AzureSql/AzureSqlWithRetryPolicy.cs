namespace Tailspin.Web.Survey.Shared.Stores.AzureSql
{
    using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
    using Tailspin.Web.Survey.Shared.Stores.Azure;

    public class AzureSqlWithRetryPolicy : AzureObjectWithRetryPolicyFactory
    {
        protected RetryPolicy CommandRetryPolicy
        {
            get
            {
                var retryPolicy = this.GetRetryPolicyFactoryInstance().GetDefaultSqlCommandRetryPolicy();
                retryPolicy.Retrying += new System.EventHandler<RetryingEventArgs>(RetryPolicyTrace);
                return retryPolicy;
            }
        }

        protected RetryPolicy ConnectionRetryPolicy
        {
            get
            {
                var retryPolicy = this.GetRetryPolicyFactoryInstance().GetDefaultSqlConnectionRetryPolicy();
                retryPolicy.Retrying += new System.EventHandler<RetryingEventArgs>(RetryPolicyTrace);
                return retryPolicy;
            }
        }
    }
}
