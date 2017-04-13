using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tailspin.SurveyAnswerService.Tests.Models
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Tailspin.SurveyAnswerService.Models;

    [TestClass]
    public class MappingExtensionsFixture
    {
        [TestMethod]
        public void ToSurveyAnswerReturnsTitleAndSlug()
        {
            var surveyAnswer = MappingExtensions.ToSurveyAnswer(new Client.Models.SurveyAnswer { Title = "testtitle", SlugName = "slugname" });

            Assert.AreEqual("testtitle", surveyAnswer.Title);
            Assert.AreEqual("slugname", surveyAnswer.SlugName);
        }

        [TestMethod]
        public void ToClientSurveyAnswerReturnsTitleAndSlug()
        {
            var clientSurveyAnswer = MappingExtensions.ToSurveyAnswer(new SurveyAnswer { Title = "testtitle", SlugName = "slugname" });

            Assert.AreEqual("testtitle", clientSurveyAnswer.Title);
            Assert.AreEqual("slugname", clientSurveyAnswer.SlugName);
        }

        [TestMethod]
        public void ToQuestionAnswerReturnsAnswer()
        {
            var questionAnswer = MappingExtensions.ToQuestionAnswer(new Client.Models.QuestionAnswer
                                                                            { QuestionText = "questiontext",
                                                                              Answer = "answervalue",
                                                                              QuestionType = Client.Models.QuestionType.SimpleText });

            Assert.AreEqual("questiontext", questionAnswer.QuestionText);
            Assert.AreEqual("answervalue", questionAnswer.Answer);
            Assert.AreEqual("SimpleText", questionAnswer.QuestionType);
        }

        [TestMethod]
        public void ToClientQuestionAnswerReturnsAnswer()
        {
            var clientQuestionAnswer = MappingExtensions.ToQuestionAnswer(new QuestionAnswer
                                                                                { QuestionText = "questiontext",
                                                                                  Answer = "answervalue",
                                                                                  QuestionType = "SimpleText" });

            Assert.AreEqual("questiontext", clientQuestionAnswer.QuestionText);
            Assert.AreEqual("answervalue", clientQuestionAnswer.Answer);
            Assert.AreEqual(Client.Models.QuestionType.SimpleText, clientQuestionAnswer.QuestionType);
        }
    }
}
