namespace Tailspin.Web.Survey.Shared.Stores.Azure
{
    using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;

    public interface IRetryPolicyFactory
    {
        RetryPolicy GetDefaultSqlCommandRetryPolicy();

        RetryPolicy GetDefaultSqlConnectionRetryPolicy();
    }
}
