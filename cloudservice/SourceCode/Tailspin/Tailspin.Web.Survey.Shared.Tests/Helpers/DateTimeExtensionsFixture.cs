namespace Tailspin.Web.Survey.Shared.Tests.Helpers
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Tailspin.Web.Survey.Shared.Helpers;
    
    [TestClass]
    public class DateTimeExtensionsFixture
    {
        [TestMethod]
        public void DateReturnsGetFormatedTicks()
        {
            Assert.AreEqual("0000000000000001001", new DateTime(1001).GetFormatedTicks());
        }
    }
}