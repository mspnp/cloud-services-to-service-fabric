namespace Tailspin.Web.Survey.Public.Tests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Tailspin.Web.Survey.Public.Controllers;
    using Tailspin.Web.Survey.Public.Models;
    using Tailspin.Web.Survey.Shared.Models;
    using Tailspin.Web.Survey.Shared.Stores;
    using System.Threading.Tasks;

    [TestClass]
    public class SurveysControllerFixture
    {
        [TestMethod]
        public async Task DisplayCallsStoreWithTenantAndIdForSurveyToGet()
        {
            var mockSurveyStore = new Mock<ISurveyStore>();
            mockSurveyStore.Setup(r => r.GetSurveyByTenantAndSlugNameAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                                              .ReturnsAsync(new Survey());

            using (var controller = new SurveysController(mockSurveyStore.Object, default(ISurveyAnswerStore)))
            {
                await controller.Display("tenant", "slug");
            }

            mockSurveyStore.Verify(
                r => r.GetSurveyByTenantAndSlugNameAsync(
                    It.Is<string>(t => "tenant" == t),
                    It.Is<string>(s => "slug" == s),
                    It.Is<bool>(b => b)),
                    Times.Once());
        }

        [TestMethod]
        public async Task DisplayReturnsEmptyViewName()
        {
            var mockSurveyStore = new Mock<ISurveyStore>();
            mockSurveyStore.Setup(r => r.GetSurveyByTenantAndSlugNameAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                                              .ReturnsAsync(new Survey());

            using (var controller = new SurveysController(mockSurveyStore.Object, default(ISurveyAnswerStore)))
            {
                var result = await controller.Display("tenant", string.Empty) as ViewResult;

                Assert.AreEqual(string.Empty, result.ViewName);
            }
        }

        [TestMethod]
        public async Task DisplayCopiesTheSurveyTitleToTheSurveyAnswerReturnedInTheContentModel()
        {
            var survey = new Survey { Title = "title to be copied" };

            var mockSurveyStore = new Mock<ISurveyStore>();
            mockSurveyStore.Setup(r => r.GetSurveyByTenantAndSlugNameAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                                              .ReturnsAsync(survey);

            using (var controller = new SurveysController(mockSurveyStore.Object, default(ISurveyAnswerStore)))
            {
                var result = await controller.Display("tenant", string.Empty) as ViewResult;

                var model = result.ViewData.Model as TenantPageViewData<SurveyAnswer>;
                Assert.AreEqual("title to be copied", model.ContentModel.Title);
            }
        }

        [TestMethod]
        public async Task DisplayCopiesTheSurveyQuestionTextToTheSurveyAnswerReturnedInTheContentModel()
        {
            var survey = new Survey
            {
                Questions = new List<Question>(
                    new[]
                                     {
                                        new Question { Text = "question text to copy" },
                                     })
            };

            var mockSurveyStore = new Mock<ISurveyStore>();
            mockSurveyStore.Setup(r => r.GetSurveyByTenantAndSlugNameAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(survey);

            using (var controller = new SurveysController(mockSurveyStore.Object, default(ISurveyAnswerStore)))
            {
                var result = await controller.Display("tenant", string.Empty) as ViewResult;

                var model = result.ViewData.Model as TenantPageViewData<SurveyAnswer>;
                Assert.AreEqual("question text to copy", model.ContentModel.QuestionAnswers.First().QuestionText);
            }
        }

        [TestMethod]
        public async Task DisplayCopiesTheSurveyQuestionTypeToTheQuestionAnswerReturnedInTheContentModel()
        {
            var survey = new Survey
            {
                Questions = new List<Question>(
                    new[]
                                     {
                                        new Question { Type = QuestionType.SimpleText },
                                     })
            };
            
            var mockSurveyStore = new Mock<ISurveyStore>();
            mockSurveyStore.Setup(r => r.GetSurveyByTenantAndSlugNameAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                                              .ReturnsAsync(survey);

            using (var controller = new SurveysController(mockSurveyStore.Object, default(ISurveyAnswerStore)))
            {
                var result = await controller.Display("tenant", string.Empty) as ViewResult;

                var model = result.ViewData.Model as TenantPageViewData<SurveyAnswer>;
                Assert.AreEqual(QuestionType.SimpleText, model.ContentModel.QuestionAnswers.First().QuestionType);
            }
        }

        [TestMethod]
        public async Task DisplayTransformsAllTheSurveyQuestionsToQuestionAnswerRetrnedInTheContentModel()
        {
            var survey = new Survey
            {
                Questions = new List<Question>(
                    new[]
                                     {
                                        new Question(),
                                        new Question()
                                     })
            };
            
            var mockSurveyStore = new Mock<ISurveyStore>();
            mockSurveyStore.Setup(r => r.GetSurveyByTenantAndSlugNameAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                                              .ReturnsAsync(survey);

            using (var controller = new SurveysController(mockSurveyStore.Object, default(ISurveyAnswerStore)))
            {
                var result = await controller.Display("tenant", string.Empty) as ViewResult;

                var model = result.ViewData.Model as TenantPageViewData<SurveyAnswer>;
                Assert.AreEqual(2, model.ContentModel.QuestionAnswers.Count());
            }
        }

        [TestMethod]
        public async Task DisplayReturnsSurveyTitleAsTitleInTheModel()
        {
            var survey = new Survey { Title = "title" };

            var mockSurveyStore = new Mock<ISurveyStore>();
            mockSurveyStore.Setup(r => r.GetSurveyByTenantAndSlugNameAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                                              .ReturnsAsync(survey);

            using (var controller = new SurveysController(mockSurveyStore.Object, default(ISurveyAnswerStore)))
            {
                var result = await controller.Display("tenant", string.Empty) as ViewResult;

                var model = result.ViewData.Model as TenantMasterPageViewData;
                Assert.AreSame("title", model.Title);
            }
        }

        [TestMethod]
        public async Task DisplayWhenPostReturnsRedirectToThankYouAction()
        {
            var mockSurveyStore = new Mock<ISurveyStore>();
            mockSurveyStore.Setup(r => r.GetSurveyByTenantAndSlugNameAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                                              .ReturnsAsync(new Survey());
            var mockSurveyAnswerStore = new Mock<ISurveyAnswerStore>();

            using (var controller = new SurveysController(mockSurveyStore.Object, mockSurveyAnswerStore.Object))
            {
                var result = await controller.Display("tenant", string.Empty, new SurveyAnswer()) as RedirectToRouteResult;

                Assert.AreEqual("ThankYou", result.RouteValues["action"]);
                Assert.AreEqual(null, result.RouteValues["controller"]);
            }
        }

        [TestMethod]
        public async Task DisplayWhenPostCallsStoreWithTenantAndIdForSurveyToGet()
        {
            var mockSurveyStore = new Mock<ISurveyStore>();
            mockSurveyStore.Setup(r => r.GetSurveyByTenantAndSlugNameAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                                              .ReturnsAsync(new Survey());
            var mockSurveyAnswerStore = new Mock<ISurveyAnswerStore>();

            using (var controller = new SurveysController(mockSurveyStore.Object, mockSurveyAnswerStore.Object))
            {
                await controller.Display("tenant", "slug", new SurveyAnswer());
            }

            mockSurveyStore.Verify(
                r => r.GetSurveyByTenantAndSlugNameAsync(
                    It.Is<string>(t => "tenant" == t),
                    It.Is<string>(s => "slug" == s),
                    It.Is<bool>(b => b)),
                    Times.Once());
        }

        [TestMethod]
        public async Task DisplayWhenPostCallsSaveSurveyAnswerFromSurveyAnswerStoreWithTheTenantReadFromTheControllerWhenModelIsValid()
        {
            var survey = new Survey();

            var mockSurveyStore = new Mock<ISurveyStore>();
            mockSurveyStore.Setup(r => r.GetSurveyByTenantAndSlugNameAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                                              .ReturnsAsync(survey);
            var mockSurveyAnswerStore = new Mock<ISurveyAnswerStore>();

            using (var controller = new SurveysController(mockSurveyStore.Object, mockSurveyAnswerStore.Object))
            {
                await controller.Display("tenant", string.Empty, new SurveyAnswer());
            }

            mockSurveyAnswerStore.Verify(
                r => r.SaveSurveyAnswerAsync(
                    It.Is<SurveyAnswer>(sa => "tenant" == sa.TenantId)),
                Times.Once());
        }

        [TestMethod]
        public async Task DisplayWhenPostCallsSaveSurveyAnswerFromSurveyAnswerStoreWithTheSlugParameterWhenModelIsValid()
        {
            var survey = new Survey();

            var mockSurveyStore = new Mock<ISurveyStore>();
            mockSurveyStore.Setup(r => r.GetSurveyByTenantAndSlugNameAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                                              .ReturnsAsync(survey);
            var mockSurveyAnswerStore = new Mock<ISurveyAnswerStore>();

            using (var controller = new SurveysController(mockSurveyStore.Object, mockSurveyAnswerStore.Object))
            {
                await controller.Display(string.Empty, "slug", new SurveyAnswer());
            }

            mockSurveyAnswerStore.Verify(
                r => r.SaveSurveyAnswerAsync(
                    It.Is<SurveyAnswer>(sa => "slug" == sa.SlugName)),
                Times.Once());
        }

        [TestMethod]
        public async Task DisplayWhenPostCallsSaveSurveyAnswerFromSurveyAnswerStoreWithQuestionTextReadFromTheSurveyWhenModelIsValid()
        {
            var survey = new Survey
            {
                Questions = new List<Question>(
                    new[]
                                     {
                                        new Question { Text = "question text" },
                                     })
            };

            var mockSurveyStore = new Mock<ISurveyStore>();
            mockSurveyStore.Setup(r => r.GetSurveyByTenantAndSlugNameAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                                              .ReturnsAsync(survey);
            var mockSurveyAnswerStore = new Mock<ISurveyAnswerStore>();
            var surveyAnswer = new SurveyAnswer
            {
                QuestionAnswers = new List<QuestionAnswer>(new[] { new QuestionAnswer() })
            };

            using (var controller = new SurveysController(mockSurveyStore.Object, mockSurveyAnswerStore.Object))
            {
                await controller.Display(string.Empty, string.Empty, surveyAnswer);
            }

            mockSurveyAnswerStore.Verify(
                r => r.SaveSurveyAnswerAsync(
                    It.Is<SurveyAnswer>(sa => "question text" == sa.QuestionAnswers[0].QuestionText)),
                Times.Once());
        }

        [TestMethod]
        public async Task DisplayWhenPostCallsSaveSurveyAnswerFromSurveyAnswerStoreWithQuestionTypeReadFromTheSurveyWhenModelIsValid()
        {
            var survey = new Survey
            {
                Questions = new List<Question>(
                    new[]
                                     {
                                        new Question { Type = QuestionType.SimpleText },
                                     })
            }; 
            
            var mockSurveyStore = new Mock<ISurveyStore>();
            mockSurveyStore.Setup(r => r.GetSurveyByTenantAndSlugNameAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                                              .ReturnsAsync(survey);
            var mockSurveyAnswerStore = new Mock<ISurveyAnswerStore>();
            var surveyAnswer = new SurveyAnswer
            {
                QuestionAnswers = new List<QuestionAnswer>(new[] { new QuestionAnswer() })
            };

            using (var controller = new SurveysController(mockSurveyStore.Object, mockSurveyAnswerStore.Object))
            {
                await controller.Display(string.Empty, string.Empty, surveyAnswer);
            }

            mockSurveyAnswerStore.Verify(
                r => r.SaveSurveyAnswerAsync(
                    It.Is<SurveyAnswer>(sa => QuestionType.SimpleText == sa.QuestionAnswers[0].QuestionType)),
                Times.Once());
        }

        [TestMethod]
        public async Task DisplayWhenPostCallsSaveSurveyAnswerFromSurveyAnswerStoreWithAnswerReadFromTheParameterWhenModelIsValid()
        {
            var survey = new Survey
            {
                Questions = new List<Question>(new[] { new Question() })
            }; 
            
            var mockSurveyStore = new Mock<ISurveyStore>();
            mockSurveyStore.Setup(r => r.GetSurveyByTenantAndSlugNameAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                                              .ReturnsAsync(survey);
            var mockSurveyAnswerStore = new Mock<ISurveyAnswerStore>();
            var surveyAnswer = new SurveyAnswer
            {
                QuestionAnswers = new List<QuestionAnswer>(new[] { new QuestionAnswer { Answer = "answer" } })
            };

            using (var controller = new SurveysController(mockSurveyStore.Object, mockSurveyAnswerStore.Object))
            {
                await controller.Display(string.Empty, string.Empty, surveyAnswer);
            }

            mockSurveyAnswerStore.Verify(
                r => r.SaveSurveyAnswerAsync(
                    It.Is<SurveyAnswer>(sa => "answer" == sa.QuestionAnswers[0].Answer)),
                Times.Once());
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task DisplayWhenPostThrowsIfSaveSurveyAnswersReadFromSurveyStoreHaveDifferentCountToTheSurveyAnswerParameter()
        {
            var surveyWith1Question = new Survey
            {
                Questions = new List<Question>(new[] { new Question() })
            }; 
            
            var mockSurveyStore = new Mock<ISurveyStore>();
            mockSurveyStore.Setup(r => r.GetSurveyByTenantAndSlugNameAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                                              .ReturnsAsync(surveyWith1Question);
            var surveyAnswerWithoutQuestions = new SurveyAnswer();

            using (var controller = new SurveysController(mockSurveyStore.Object, default(ISurveyAnswerStore)))
            {
                await controller.Display(string.Empty, string.Empty, surveyAnswerWithoutQuestions);
            }
        }

        [TestMethod]
        public async Task DisplayWhenPostReturnsModelWithTheAnswersReadFromTheParameterWhenModelIsNotValid()
        {
            var mockSurveyStore = new Mock<ISurveyStore>();
            var survey = new Survey
            {
                Questions = new List<Question>(new[] { new Question() })
            };
            mockSurveyStore.Setup(r => r.GetSurveyByTenantAndSlugNameAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                                              .ReturnsAsync(survey);
            var surveyAnswer = new SurveyAnswer
            {
                QuestionAnswers = new List<QuestionAnswer>(new[] { new QuestionAnswer { Answer = "answer" } })
            };

            using (var controller = new SurveysController(mockSurveyStore.Object, default(ISurveyAnswerStore)))
            {
                controller.ModelState.AddModelError("error for test", "invalid model state");

                var result = await controller.Display(string.Empty, string.Empty, surveyAnswer) as ViewResult;

                var model = result.ViewData.Model as TenantPageViewData<SurveyAnswer>;
                Assert.AreEqual("answer", model.ContentModel.QuestionAnswers.First().Answer);
            }
        }

        [TestMethod]
        public async Task DisplayWhenPostReturnsEmptyViewNameWhenModelIsNotValid()
        {
            var mockSurveyStore = new Mock<ISurveyStore>();
            var survey = new Survey
            {
                Questions = new List<Question>(new[] { new Question() })
            };
            mockSurveyStore.Setup(r => r.GetSurveyByTenantAndSlugNameAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                                              .ReturnsAsync(survey);
            var surveyAnswer = new SurveyAnswer
            {
                QuestionAnswers = new List<QuestionAnswer>(new[] { new QuestionAnswer() })
            };

            using (var controller = new SurveysController(mockSurveyStore.Object, default(ISurveyAnswerStore)))
            {
                controller.ModelState.AddModelError("error for test", "invalid model state");

                var result = await controller.Display(string.Empty, string.Empty, surveyAnswer) as ViewResult;

                Assert.AreEqual(string.Empty, result.ViewName);
            }
        }

        [TestMethod]
        public async Task DisplayWhenPostReturnsSurveyTitleAsTitleInTheModelWhenModelIsNotValid()
        {
            var mockSurveyStore = new Mock<ISurveyStore>();
            var survey = new Survey
            {
                Title = "title",
                Questions = new List<Question>(new[] { new Question() })
            };
            mockSurveyStore.Setup(r => r.GetSurveyByTenantAndSlugNameAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                                              .ReturnsAsync(survey);
            var surveyAnswer = new SurveyAnswer
            {
                QuestionAnswers = new List<QuestionAnswer>(new[] { new QuestionAnswer() })
            };

            using (var controller = new SurveysController(mockSurveyStore.Object, default(ISurveyAnswerStore)))
            {
                controller.ModelState.AddModelError("error for test", "invalid model state");

                var result = await controller.Display(string.Empty, string.Empty, surveyAnswer) as ViewResult;

                var model = result.ViewData.Model as TenantMasterPageViewData;
                Assert.AreSame("title", model.Title);
            }
        }

        [TestMethod]
        public async Task IndexReturnsTitleInTheModel()
        {
            var mockSurveyStore = new Mock<ISurveyStore>();

            using (var controller = new SurveysController(mockSurveyStore.Object, default(ISurveyAnswerStore)))
            {
                var result = await controller.Index() as ViewResult;

                var model = result.ViewData.Model as TenantMasterPageViewData;
                Assert.AreEqual("Existing surveys", model.Title);
            }
        }

        [TestMethod]
        public async Task IndexGetsRecentSurveysFromStore()
        {
            var mockSurveyStore = new Mock<ISurveyStore>();

            using (var controller = new SurveysController(mockSurveyStore.Object, default(ISurveyAnswerStore)))
            {
                await controller.Index();
            }

            mockSurveyStore.Verify(r => r.GetRecentSurveysAsync(), Times.Once());
        }

        [TestMethod]
        public async Task IndexReturnsRecentSurveysFromStoreInTheModel()
        {
            var mockSurveyStore = new Mock<ISurveyStore>();
            var surveys = new List<Survey>();
            mockSurveyStore.Setup(r => r.GetRecentSurveysAsync()).ReturnsAsync(surveys);

            using (var controller = new SurveysController(mockSurveyStore.Object, default(ISurveyAnswerStore)))
            {
                var result = await controller.Index() as ViewResult;

                var model = result.ViewData.Model as TenantPageViewData<IEnumerable<Survey>>;
                Assert.AreSame(surveys, model.ContentModel);
            }
        }

        [TestMethod]
        public void ThankYouReturnsModel()
        {
            using (var controller = new SurveysController(null, null))
            {
                var result = controller.ThankYou() as ViewResult;

                var model = result.ViewData.Model as TenantMasterPageViewData;
                Assert.AreEqual("Thank you for filling the survey", model.Title);
            }
        }
    }
}
