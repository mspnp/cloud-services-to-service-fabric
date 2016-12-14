namespace Tailspin.Web.Tests.Area.Survey.Models
{
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Extensions;

    [TestClass]
    public class SummaryExtensionsFixture
    {
        [TestMethod]
        public void ShouldCalculatePercentageForSummary()
        {
            int percent = 2.PercentOf(4);

            Assert.AreEqual(50, percent);
        }

        [TestMethod]
        public void ShouldReturnZeroWhenDivideByZero()
        {
            int percent = 4.PercentOf(0);

            Assert.AreEqual(0, percent);
        }

        [TestMethod]
        public void ShouldReturnZeroIfZeroDivisor()
        {
            int percent = 0.PercentOf(4);

            Assert.AreEqual(0, percent);
        }

        [TestMethod]
        public void WillSumEmptyDictionaryOfSummaries()
        {
            var values = new Dictionary<string, int>();

            int total = values.Total();

            Assert.AreEqual(0, total);
        }

        [TestMethod]
        public void WillSumDictionaryOfSummaries()
        {
            var values = new Dictionary<string, int>();
            values.Add("1", 1);
            values.Add("2", 2);

            int total = values.Total();

            Assert.AreEqual(3, total);
        }
    }
}