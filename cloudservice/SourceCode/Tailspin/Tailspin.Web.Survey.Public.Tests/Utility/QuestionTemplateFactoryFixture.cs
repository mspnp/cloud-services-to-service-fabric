namespace Tailspin.Web.Survey.Public.Tests.Utility
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Tailspin.Web.Survey.Public.Utility;
    using Tailspin.Web.Survey.Shared.Models;

    [TestClass]
    public class QuestionTemplateFactoryFixture
    {
        [TestMethod]
        public void CreateForSimpleText()
        {
            Assert.AreEqual(QuestionType.SimpleText.ToString(), QuestionTemplateFactory.Create(new QuestionAnswer { QuestionType = QuestionType.SimpleText }));
        }

        [TestMethod]
        public void CreateForMultipleChoice()
        {
            Assert.AreEqual(QuestionType.MultipleChoice.ToString(), QuestionTemplateFactory.Create(new QuestionAnswer { QuestionType = QuestionType.MultipleChoice }));
        }

        [TestMethod]
        public void CreateForFiveStars()
        {
            Assert.AreEqual(QuestionType.FiveStars.ToString(), QuestionTemplateFactory.Create(new QuestionAnswer { QuestionType = QuestionType.FiveStars }));
        }
    }
}
