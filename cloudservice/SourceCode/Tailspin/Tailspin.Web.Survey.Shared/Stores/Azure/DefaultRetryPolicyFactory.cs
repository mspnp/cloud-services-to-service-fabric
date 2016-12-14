namespace Tailspin.Web.Survey.Shared.Stores.Azure
{
    using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;

    public class DefaultRetryPolicyFactory : IRetryPolicyFactory
    {
        public RetryPolicy GetDefaultSqlCommandRetryPolicy()
        {
            return new RetryPolicy(new SqlDatabaseTransientErrorDetectionStrategy(), 3);
        }

        public RetryPolicy GetDefaultSqlConnectionRetryPolicy()
        {
            return new RetryPolicy(new SqlDatabaseTransientErrorDetectionStrategy(), 3);
        }
    }
}
