namespace Tailspin.SurveyAnalysisService.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Models;
    using Newtonsoft.Json;

    [TestClass]
    public class SurveyAnswersSummaryFixture
    {
        [TestMethod]
        public void AddNewAnswerCallsMergeWith1AsTotalAnswers()
        {
            var surveyAnswersSummary = new StubSurveyAnswersSummary();
            var surveyAnswer = new SurveyAnswer();

            surveyAnswersSummary.AddNewAnswer(surveyAnswer);

            Assert.AreEqual(1, surveyAnswersSummary.MergeParameter.TotalAnswers);
        }

        [TestMethod]
        public void AddNew5StarsAnswerCallsMergeWithNewSummary()
        {
            var surveyAnswersSummary = new StubSurveyAnswersSummary();
            var questionAnswer = new QuestionAnswer
            {
                QuestionType = Enum.GetName(typeof(QuestionType), QuestionType.FiveStars),
                QuestionText = "5 stars",
                Answer = "1"
            };
            var surveyAnswer = new SurveyAnswer();
            surveyAnswer.QuestionAnswers.Add(questionAnswer);

            surveyAnswersSummary.AddNewAnswer(surveyAnswer);

            var questionAnswersSummary = surveyAnswersSummary.MergeParameter.QuestionAnswersSummaries.First();
            Assert.AreEqual("5 stars", questionAnswersSummary.QuestionText);
            Assert.AreEqual(QuestionType.FiveStars, questionAnswersSummary.QuestionType);
            Assert.AreEqual("1.00", questionAnswersSummary.AnswersSummary);
        }

        [TestMethod]
        public void AddNewMultipleChoiceAnswerCallsMergeWithNewSummary()
        {
            var surveyAnswersSummary = new StubSurveyAnswersSummary();
            var questionAnswer = new QuestionAnswer
            {
                QuestionType = Enum.GetName(typeof(QuestionType), QuestionType.MultipleChoice),
                QuestionText = "multiple choice",
                Answer = "choice 1",
                PossibleAnswers = "possible answers"
            };
            var surveyAnswer = new SurveyAnswer();
            surveyAnswer.QuestionAnswers.Add(questionAnswer);

            surveyAnswersSummary.AddNewAnswer(surveyAnswer);

            var questionAnswersSummary = surveyAnswersSummary.MergeParameter.QuestionAnswersSummaries.First();
            Assert.AreEqual("multiple choice", questionAnswersSummary.QuestionText);
            Assert.AreEqual("possible answers", questionAnswersSummary.PossibleAnswers);
            Assert.AreEqual(QuestionType.MultipleChoice, questionAnswersSummary.QuestionType);
            var actualSummary = JsonConvert.DeserializeObject<Dictionary<string, int>>(questionAnswersSummary.AnswersSummary);
            Assert.AreEqual(1, actualSummary.Keys.Count);
            Assert.AreEqual(1, actualSummary["choice 1"]);
        }

        [TestMethod]
        public void MergeCopiesTheNewAnswersSummariesWhenThereAreNoPreviousAnswers()
        {
            var existingSurveyAnswersSummary = new SurveyAnswersSummary
            {
                TotalAnswers = 0
            };
            var newSurveyAnswersSummary = new SurveyAnswersSummary
            {
                TotalAnswers = 1
            };
            newSurveyAnswersSummary.QuestionAnswersSummaries.Add(new QuestionAnswersSummary());

            existingSurveyAnswersSummary.MergeWith(newSurveyAnswersSummary);

            Assert.AreEqual(newSurveyAnswersSummary.QuestionAnswersSummaries, existingSurveyAnswersSummary.QuestionAnswersSummaries);
        }

        [TestMethod]
        public void MergeCopiesTotalAnswersWhenThereAreNoPreviousAnswers()
        {
            var existingSurveyAnswersSummary = new SurveyAnswersSummary
            {
                TotalAnswers = 0
            };
            var newSurveyAnswersSummary = new SurveyAnswersSummary
            {
                TotalAnswers = 1
            };
            newSurveyAnswersSummary.QuestionAnswersSummaries.Add(new QuestionAnswersSummary());

            existingSurveyAnswersSummary.MergeWith(newSurveyAnswersSummary);

            Assert.AreEqual(1, existingSurveyAnswersSummary.TotalAnswers);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void MergeThrowsWhenSlugNamesAreNotEqual()
        {
            var existingSurveyAnswersSummary = new SurveyAnswersSummary
            {
                SlugName = "slug-name"
            };
            var newSurveyAnswersSummary = new SurveyAnswersSummary
            {
                SlugName = "other-slug-name"
            };

            existingSurveyAnswersSummary.MergeWith(newSurveyAnswersSummary);
        }

        [TestMethod]
        public void Merge5StarsAnswersCalculatesRatingAverageFor2Answers()
        {
            var existingSurveyAnswersSummary = new SurveyAnswersSummary
            {
                TotalAnswers = 1
            };
            var existingQuestionAnswersSummary = new QuestionAnswersSummary
            {
                QuestionType = QuestionType.FiveStars,
                AnswersSummary = "1",
                QuestionText = "question to merge"
            };
            existingSurveyAnswersSummary.QuestionAnswersSummaries.Add(existingQuestionAnswersSummary);

            var newSurveyAnswersSummary = new SurveyAnswersSummary
            {
                TotalAnswers = 1
            };
            var newQuestionAnswersSummary = new QuestionAnswersSummary
            {
                QuestionType = QuestionType.FiveStars,
                AnswersSummary = "2",
                QuestionText = "question to merge"
            };
            newSurveyAnswersSummary.QuestionAnswersSummaries.Add(newQuestionAnswersSummary);

            existingSurveyAnswersSummary.MergeWith(newSurveyAnswersSummary);

            Assert.AreEqual(2, existingSurveyAnswersSummary.TotalAnswers);
            Assert.AreEqual(1, existingSurveyAnswersSummary.QuestionAnswersSummaries.Count);
            var questionAnswersSummary = existingSurveyAnswersSummary.QuestionAnswersSummaries.First();
            Assert.AreEqual("question to merge", questionAnswersSummary.QuestionText);
            Assert.AreEqual(QuestionType.FiveStars, questionAnswersSummary.QuestionType);
            Assert.AreEqual("1.50", questionAnswersSummary.AnswersSummary);
        }

        [TestMethod]
        public void Merge5StarsAnswersCalculatesRatingAverageFor3Answers()
        {
            var existingSurveyAnswersSummary = new SurveyAnswersSummary
            {
                TotalAnswers = 2
            };
            var existingQuestionAnswersSummary = new QuestionAnswersSummary
            {
                QuestionType = QuestionType.FiveStars,
                AnswersSummary = "1",
                QuestionText = "question to merge"
            };
            existingSurveyAnswersSummary.QuestionAnswersSummaries.Add(existingQuestionAnswersSummary);

            var newSurveyAnswersSummary = new SurveyAnswersSummary
            {
                TotalAnswers = 1
            };
            var newQuestionAnswersSummary = new QuestionAnswersSummary
            {
                QuestionType = QuestionType.FiveStars,
                AnswersSummary = "2",
                QuestionText = "question to merge"
            };
            newSurveyAnswersSummary.QuestionAnswersSummaries.Add(newQuestionAnswersSummary);

            existingSurveyAnswersSummary.MergeWith(newSurveyAnswersSummary);

            Assert.AreEqual(3, existingSurveyAnswersSummary.TotalAnswers);
            Assert.AreEqual(1, existingSurveyAnswersSummary.QuestionAnswersSummaries.Count);
            var questionAnswersSummary = existingSurveyAnswersSummary.QuestionAnswersSummaries.First();
            Assert.AreEqual("question to merge", questionAnswersSummary.QuestionText);
            Assert.AreEqual(QuestionType.FiveStars, questionAnswersSummary.QuestionType);
            Assert.AreEqual("1.33", questionAnswersSummary.AnswersSummary);
        }

        [TestMethod]
        public void MergeMultipleChoiceAnswersFor2DifferentAnswers()
        {
            var existingSurveyAnswersSummary = new SurveyAnswersSummary
            {
                TotalAnswers = 1
            };
            var existingQuestionAnswersSummary = new QuestionAnswersSummary
            {
                QuestionType = QuestionType.MultipleChoice,
                AnswersSummary = JsonConvert.SerializeObject(new Dictionary<string, int> { { "choice 1", 1 } }),
                QuestionText = "question to merge"
            };
            existingSurveyAnswersSummary.QuestionAnswersSummaries.Add(existingQuestionAnswersSummary);

            var newSurveyAnswersSummary = new SurveyAnswersSummary
            {
                TotalAnswers = 1
            };
            var newQuestionAnswersSummary = new QuestionAnswersSummary
            {
                QuestionType = QuestionType.MultipleChoice,
                AnswersSummary = JsonConvert.SerializeObject(new Dictionary<string, int> { { "choice 2", 1 } }),
                QuestionText = "question to merge"
            };
            newSurveyAnswersSummary.QuestionAnswersSummaries.Add(newQuestionAnswersSummary);

            existingSurveyAnswersSummary.MergeWith(newSurveyAnswersSummary);

            Assert.AreEqual(2, existingSurveyAnswersSummary.TotalAnswers);
            Assert.AreEqual(1, existingSurveyAnswersSummary.QuestionAnswersSummaries.Count);
            var questionAnswersSummary = existingSurveyAnswersSummary.QuestionAnswersSummaries.First();
            Assert.AreEqual("question to merge", questionAnswersSummary.QuestionText);
            Assert.AreEqual(QuestionType.MultipleChoice, questionAnswersSummary.QuestionType);
            var actualSummary = JsonConvert.DeserializeObject<Dictionary<string, int>>(questionAnswersSummary.AnswersSummary);
            Assert.AreEqual(2, actualSummary.Keys.Count);
            Assert.AreEqual(1, actualSummary["choice 1"]);
            Assert.AreEqual(1, actualSummary["choice 2"]);
        }

        [TestMethod]
        public void MergeMultipleChoiceAnswersFor2SameAnswers()
        {
            var existingSurveyAnswersSummary = new SurveyAnswersSummary
            {
                TotalAnswers = 1
            };
            var existingQuestionAnswersSummary = new QuestionAnswersSummary
            {
                QuestionType = QuestionType.MultipleChoice,
                AnswersSummary = JsonConvert.SerializeObject(new Dictionary<string, int> { { "choice 1", 1 } }),
                QuestionText = "question to merge"
            };
            existingSurveyAnswersSummary.QuestionAnswersSummaries.Add(existingQuestionAnswersSummary);

            var newSurveyAnswersSummary = new SurveyAnswersSummary
            {
                TotalAnswers = 1
            };
            var newQuestionAnswersSummary = new QuestionAnswersSummary
            {
                QuestionType = QuestionType.MultipleChoice,
                AnswersSummary = JsonConvert.SerializeObject(new Dictionary<string, int> { { "choice 1", 1 } }),
                QuestionText = "question to merge"
            };
            newSurveyAnswersSummary.QuestionAnswersSummaries.Add(newQuestionAnswersSummary);

            existingSurveyAnswersSummary.MergeWith(newSurveyAnswersSummary);

            Assert.AreEqual(2, existingSurveyAnswersSummary.TotalAnswers);
            Assert.AreEqual(1, existingSurveyAnswersSummary.QuestionAnswersSummaries.Count);
            var questionAnswersSummary = existingSurveyAnswersSummary.QuestionAnswersSummaries.First();
            Assert.AreEqual("question to merge", questionAnswersSummary.QuestionText);
            Assert.AreEqual(QuestionType.MultipleChoice, questionAnswersSummary.QuestionType);
            var actualSummary = JsonConvert.DeserializeObject<Dictionary<string, int>>(questionAnswersSummary.AnswersSummary);
            Assert.AreEqual(1, actualSummary.Keys.Count);
            Assert.AreEqual(2, actualSummary["choice 1"]);
        }

        public class StubSurveyAnswersSummary : SurveyAnswersSummary
        {
            public SurveyAnswersSummary MergeParameter { get; set; }

            public override void MergeWith(SurveyAnswersSummary surveyAnswersSummary)
            {
                this.MergeParameter = surveyAnswersSummary;
            }
        }
    }
}