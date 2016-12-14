namespace Tailspin.Web.Survey.Shared.Tests.Stores.Azure
{
    using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Tailspin.Web.Survey.Shared.Stores.Azure;

    [TestClass]
    public class DefaultRetryPolicyFactoryFixture
    {
        [TestMethod]
        public void GetDefaultSqlCommandRetryPolicyReturnsConfigured()
        {
            var policy = new DefaultRetryPolicyFactory().GetDefaultSqlCommandRetryPolicy();
            Assert.IsInstanceOfType(policy.ErrorDetectionStrategy, typeof(SqlDatabaseTransientErrorDetectionStrategy));
            Assert.IsInstanceOfType(policy.RetryStrategy, typeof(FixedInterval));
        }

        [TestMethod]
        public void GetDefaultSqlConnectionRetryPolicyReturnsConfigured()
        {
            var policy = new DefaultRetryPolicyFactory().GetDefaultSqlConnectionRetryPolicy();
            Assert.IsInstanceOfType(policy.ErrorDetectionStrategy, typeof(SqlDatabaseTransientErrorDetectionStrategy));
            Assert.IsInstanceOfType(policy.RetryStrategy, typeof(FixedInterval));
        }
    }
}
