namespace Tailspin.Web.Survey.Shared.Stores.Azure
{
    using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;

    public class ConfiguredRetryPolicyFactory : IRetryPolicyFactory
    {
        public RetryPolicy GetDefaultSqlCommandRetryPolicy()
        {
            return RetryPolicyFactory.GetDefaultSqlCommandRetryPolicy();
        }

        public RetryPolicy GetDefaultSqlConnectionRetryPolicy()
        {
            return RetryPolicyFactory.GetDefaultSqlConnectionRetryPolicy();
        }
    }
}
