namespace Tailspin.Web.Survey.Shared.Tests.Stores.AzureSql
{
    using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Tailspin.Web.Survey.Shared.Stores.Azure;
    using Tailspin.Web.Survey.Shared.Stores.AzureSql;

    [TestClass]
    public class AzureSqlWithRetryPolicyFixture
    {
        [TestMethod]
        public void CommandRetryPolicyPropertyReturnsPolicy()
        {
            Assert.IsInstanceOfType(
                new TestAzureSqlWithRetryPolicy().GetCommandRetryPolicy(),
                new DefaultRetryPolicyFactory().GetDefaultSqlCommandRetryPolicy().GetType());
        }

        [TestMethod]
        public void ConnectionRetryPolicyPropertyReturnsPolicy()
        {
            Assert.IsInstanceOfType(
                new TestAzureSqlWithRetryPolicy().GetConnectionRetryPolicy(),
                new DefaultRetryPolicyFactory().GetDefaultSqlConnectionRetryPolicy().GetType());
        }

        private class TestAzureSqlWithRetryPolicy : AzureSqlWithRetryPolicy
        {
            public RetryPolicy GetCommandRetryPolicy()
            {
                return this.CommandRetryPolicy;
            }

            public RetryPolicy GetConnectionRetryPolicy()
            {
                return this.ConnectionRetryPolicy;
            }
        }
    }
}
