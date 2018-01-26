namespace Tailspin.Web.Survey.Public.Tests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Tailspin.SurveyAnswerService.Client;
    using Tailspin.SurveyManagementService.Client;
    using Tailspin.Web.Survey.Public.Controllers;
    using Tailspin.Web.Survey.Public.Models;
    using Tailspin.Web.Shared.Models;
    using ClientModels = Tailspin.Shared.Models.Client;
    using Utility;
    using Tailspin.SurveyResponseService.Client;

    [TestClass]
    public class SurveysControllerFixture
    {
        [TestMethod]
        public async Task DisplayCallsStoreWithSlugForSurveyToGet()
        {
            var mockSurveyManagementService = new Mock<ISurveyManagementService>();
            mockSurveyManagementService.Setup(r => r.GetSurveyAsync(It.IsAny<string>()))
                .ReturnsAsync(new ClientModels.Survey());

            using (var controller = new SurveysController(
                mockSurveyManagementService.Object,
                new Mock<ISurveyAnswerService>().Object,
                new Mock<ISurveyResponseService>().Object))
            {
                await controller.Display("slug");
            }

            mockSurveyManagementService.Verify(
                r => r.GetSurveyAsync(
                    It.Is<string>(s => "slug" == s)),
                    Times.Once());
        }

        [TestMethod]
        public async Task DisplayReturnsEmptyViewName()
        {
            var mockSurveyManagementService = new Mock<ISurveyManagementService>();
            mockSurveyManagementService.Setup(r => r.GetSurveyAsync(It.IsAny<string>()))
                .ReturnsAsync(new ClientModels.Survey());

            using (var controller = new SurveysController(
                mockSurveyManagementService.Object,
                new Mock<ISurveyAnswerService>().Object,
                new Mock<ISurveyResponseService>().Object))
            {
                var result = await controller.Display(string.Empty) as ViewResult;

                Assert.IsNull(result.ViewName);
            }
        }

        [TestMethod]
        public async Task DisplayCopiesTheSurveyTitleToTheSurveyAnswerReturnedInTheContentModel()
        {
            var survey = new ClientModels.Survey()
            {
                Title = "title to be copied"
            };

            var mockSurveyManagementService = new Mock<ISurveyManagementService>();
            mockSurveyManagementService.Setup(r => r.GetSurveyAsync(It.IsAny<string>()))
                .ReturnsAsync(survey);

            using (var controller = new SurveysController(
                mockSurveyManagementService.Object,
                new Mock<ISurveyAnswerService>().Object,
                new Mock<ISurveyResponseService>().Object))
            {
                var result = await controller.Display(string.Empty) as ViewResult;

                var model = result.ViewData.Model as PageViewData<SurveyAnswer>;
                Assert.AreEqual("title to be copied", model.ContentModel.Title);
            }
        }

        [TestMethod]
        public async Task DisplayCopiesTheSurveyQuestionTextToTheSurveyAnswerReturnedInTheContentModel()
        {
            var survey = new ClientModels.Survey()
            {
                Questions = new List<ClientModels.Question>()
                {
                    new ClientModels.Question()
                    {
                        Text = "question text to copy"
                    }
                }
            };

            var mockSurveyManagementService = new Mock<ISurveyManagementService>();
            mockSurveyManagementService.Setup(r => r.GetSurveyAsync(It.IsAny<string>()))
                .ReturnsAsync(survey);

            using (var controller = new SurveysController(
                mockSurveyManagementService.Object,
                new Mock<ISurveyAnswerService>().Object,
                new Mock<ISurveyResponseService>().Object))
            {
                var result = await controller.Display(string.Empty) as ViewResult;

                var model = result.ViewData.Model as PageViewData<SurveyAnswer>;
                Assert.AreEqual("question text to copy", model.ContentModel.QuestionAnswers.First().QuestionText);
            }
        }

        [TestMethod]
        public async Task DisplayCopiesTheSurveyQuestionTypeToTheQuestionAnswerReturnedInTheContentModel()
        {
            var survey = new ClientModels.Survey()
            {
                Questions = new List<ClientModels.Question>()
                {
                    new ClientModels.Question()
                    {
                        Type = ClientModels.QuestionType.SimpleText
                    },
                }
            };

            var mockSurveyManagementService = new Mock<ISurveyManagementService>();
            mockSurveyManagementService.Setup(r => r.GetSurveyAsync(It.IsAny<string>()))
                .ReturnsAsync(survey);

            using (var controller = new SurveysController(
                mockSurveyManagementService.Object,
                new Mock<ISurveyAnswerService>().Object,
                new Mock<ISurveyResponseService>().Object))
            {
                var result = await controller.Display(string.Empty) as ViewResult;

                var model = result.ViewData.Model as PageViewData<SurveyAnswer>;
                Assert.AreEqual(QuestionType.SimpleText, model.ContentModel.QuestionAnswers.First().QuestionType);
            }
        }

        [TestMethod]
        public async Task DisplayTransformsAllTheSurveyQuestionsToQuestionAnswerRetrnedInTheContentModel()
        {
            var survey = new ClientModels.Survey()
            {
                Questions = new List<ClientModels.Question>()
                {
                    new ClientModels.Question(),
                    new ClientModels.Question()
                }
            };

            var mockSurveyManagementService = new Mock<ISurveyManagementService>();
            mockSurveyManagementService.Setup(r => r.GetSurveyAsync(It.IsAny<string>()))
                .ReturnsAsync(survey);

            using (var controller = new SurveysController(
                mockSurveyManagementService.Object,
                new Mock<ISurveyAnswerService>().Object,
                new Mock<ISurveyResponseService>().Object))
            {
                var result = await controller.Display(string.Empty) as ViewResult;

                var model = result.ViewData.Model as PageViewData<SurveyAnswer>;
                Assert.AreEqual(2, model.ContentModel.QuestionAnswers.Count());
            }
        }

        [TestMethod]
        public async Task DisplayReturnsSurveyTitleAsTitleInTheModel()
        {
            var survey = new ClientModels.Survey { Title = "title" };

            var mockSurveyManagementService = new Mock<ISurveyManagementService>();
            mockSurveyManagementService.Setup(r => r.GetSurveyAsync(It.IsAny<string>()))
                .ReturnsAsync(survey);

            using (var controller = new SurveysController(
                mockSurveyManagementService.Object,
                new Mock<ISurveyAnswerService>().Object,
                new Mock<ISurveyResponseService>().Object))
            {
                var result = await controller.Display(string.Empty) as ViewResult;

                var model = result.ViewData.Model as MasterPageViewData;
                Assert.AreSame("title", model.Title);
            }
        }

        [TestMethod]
        public async Task DisplayWhenPostReturnsRedirectToThankYouAction()
        {
            var mockSurveyManagementService = new Mock<ISurveyManagementService>();
            mockSurveyManagementService.Setup(r => r.GetSurveyAsync(It.IsAny<string>()))
                .ReturnsAsync(new ClientModels.Survey());

            using (var controller = new SurveysController(
                mockSurveyManagementService.Object,
                new Mock<ISurveyAnswerService>().Object,
                new Mock<ISurveyResponseService>().Object))
            {
                var result = await controller.Display(string.Empty, new SurveyAnswer()) as RedirectToActionResult;
                Assert.AreEqual("ThankYou", result.ActionName);
                Assert.AreEqual(null, result.ControllerName);
            }
        }

        [TestMethod]
        public async Task DisplayWhenPostCallsStoreWithSlugForSurveyToGet()
        {
            var mockSurveyManagementService = new Mock<ISurveyManagementService>();
            mockSurveyManagementService.Setup(r => r.GetSurveyAsync(It.IsAny<string>()))
                .ReturnsAsync(new ClientModels.Survey());

            using (var controller = new SurveysController(
                mockSurveyManagementService.Object,
                new Mock<ISurveyAnswerService>().Object,
                new Mock<ISurveyResponseService>().Object))
            {
                await controller.Display("slug", new SurveyAnswer());
            }

            mockSurveyManagementService.Verify(
                r => r.GetSurveyAsync(
                    It.Is<string>(s => "slug" == s)),
                    Times.Once());
        }

        [TestMethod]
        public async Task DisplayWhenPostCallsSaveSurveyAnswerFromSurveyAnswerStoreWithTheSlugParameterWhenModelIsValid()
        {
            var survey = new ClientModels.Survey()
            {
                SlugName = "slug"
            };

            var mockSurveyManagementService = new Mock<ISurveyManagementService>();
            mockSurveyManagementService.Setup(r => r.GetSurveyAsync(It.IsAny<string>()))
                .ReturnsAsync(survey);
            var mockSurveyAnswerService = new Mock<ISurveyAnswerService>();
            var mockSurveyResponseService = new Mock<ISurveyResponseService>();

            using (var controller = new SurveysController(
                mockSurveyManagementService.Object,
                mockSurveyAnswerService.Object,
                mockSurveyResponseService.Object))
            {
                await controller.Display("slug", new SurveyAnswer());
            }

            mockSurveyResponseService.Verify(r => r.SaveSurveyResponseAsync(
                It.Is<ClientModels.SurveyAnswer>(
                    sa => "slug" == sa.SlugName)),
                Times.Once);
        }

        [TestMethod]
        public async Task DisplayWhenPostCallsSaveSurveyAnswerFromSurveyAnswerStoreWithQuestionTextReadFromTheSurveyWhenModelIsValid()
        {
            var survey = new ClientModels.Survey
            {
                Questions = new List<ClientModels.Question>()
                {
                    new ClientModels.Question { Text = "question text" },
                }
            };

            var mockSurveyManagementService = new Mock<ISurveyManagementService>();
            mockSurveyManagementService.Setup(r => r.GetSurveyAsync(It.IsAny<string>()))
                .ReturnsAsync(survey);
            var mockSurveyAnswerService = new Mock<ISurveyAnswerService>();
            var mockSurveyResponseService = new Mock<ISurveyResponseService>();
            var surveyAnswer = new SurveyAnswer
            {
                QuestionAnswers = new List<QuestionAnswer>()
                {
                    new QuestionAnswer()
                }
            };

            using (var controller = new SurveysController(
                mockSurveyManagementService.Object,
                mockSurveyAnswerService.Object,
                mockSurveyResponseService.Object))
            {
                await controller.Display(string.Empty, surveyAnswer);
            }

            mockSurveyResponseService.Verify(r => r.SaveSurveyResponseAsync(
                It.Is<ClientModels.SurveyAnswer>(
                    sa => "question text" == sa.QuestionAnswers[0].QuestionText)),
                Times.Once);
        }

        [TestMethod]
        public async Task DisplayWhenPostCallsSaveSurveyAnswerFromSurveyAnswerStoreWithQuestionTypeReadFromTheSurveyWhenModelIsValid()
        {
            var survey = new ClientModels.Survey
            {
                Questions = new List<ClientModels.Question>
                {
                    new ClientModels.Question
                    {
                        Type = ClientModels.QuestionType.SimpleText
                    },
                }
            };

            var mockSurveyManagementService = new Mock<ISurveyManagementService>();
            mockSurveyManagementService.Setup(r => r.GetSurveyAsync(It.IsAny<string>()))
                .ReturnsAsync(survey);
            var mockSurveyAnswerService = new Mock<ISurveyAnswerService>();
            var mockSurveyResponseService = new Mock<ISurveyResponseService>();
            var surveyAnswer = new SurveyAnswer()
            {
                QuestionAnswers = new List<QuestionAnswer>()
                {
                    new QuestionAnswer()
                }
            };

            using (var controller = new SurveysController(
                mockSurveyManagementService.Object,
                mockSurveyAnswerService.Object,
                mockSurveyResponseService.Object))
            {
                await controller.Display(string.Empty, surveyAnswer);
            }

            mockSurveyResponseService.Verify(r => r.SaveSurveyResponseAsync(
                It.Is<ClientModels.SurveyAnswer>(
                    sa => ClientModels.QuestionType.SimpleText == sa.QuestionAnswers[0].QuestionType)),
                Times.Once);
        }

        [TestMethod]
        public async Task DisplayWhenPostCallsSaveSurveyAnswerFromSurveyAnswerStoreWithAnswerReadFromTheParameterWhenModelIsValid()
        {
            var survey = new ClientModels.Survey()
            {
                Questions = new List<ClientModels.Question>()
                {
                    new ClientModels.Question()
                }
            };

            var mockSurveyManagementService = new Mock<ISurveyManagementService>();
            mockSurveyManagementService.Setup(r => r.GetSurveyAsync(It.IsAny<string>()))
                .ReturnsAsync(survey);
            var mockSurveyAnswerService = new Mock<ISurveyAnswerService>();
            var mockSurveyResponseService = new Mock<ISurveyResponseService>();
            var surveyAnswer = new SurveyAnswer()
            {
                QuestionAnswers = new List<QuestionAnswer>()
                {
                    new QuestionAnswer()
                    {
                        Answer = "answer"
                    }
                }
            };

            using (var controller = new SurveysController(
                mockSurveyManagementService.Object,
                mockSurveyAnswerService.Object,
                mockSurveyResponseService.Object))
            {
                await controller.Display(string.Empty, surveyAnswer);
            }

            mockSurveyResponseService.Verify(r => r.SaveSurveyResponseAsync(
                It.Is<ClientModels.SurveyAnswer>(sa => "answer" == sa.QuestionAnswers[0].Answer)),
                Times.Once);
        }

        [TestMethod]
        public async Task DisplayWhenPostThrowsIfSaveSurveyAnswersReadFromSurveyStoreHaveDifferentCountToTheSurveyAnswerParameter()
        {
            var surveyWith1Question = new ClientModels.Survey()
            {
                Questions = new List<ClientModels.Question>()
                {
                    new ClientModels.Question()
                }
            };

            var mockSurveyManagementService = new Mock<ISurveyManagementService>();
            mockSurveyManagementService.Setup(r => r.GetSurveyAsync(It.IsAny<string>()))
                .ReturnsAsync(surveyWith1Question);
            var surveyAnswerWithoutQuestions = new SurveyAnswer();

            using (var controller = new SurveysController(
                mockSurveyManagementService.Object,
                new Mock<ISurveyAnswerService>().Object,
                new Mock<ISurveyResponseService>().Object))
            {
                await AssertEx.ThrowsExceptionAsync<ArgumentException>(
                    async () => await controller.Display(string.Empty, surveyAnswerWithoutQuestions),
                    string.Empty, null);
            }
        }

        [TestMethod]
        public async Task DisplayWhenPostReturnsModelWithTheAnswersReadFromTheParameterWhenModelIsNotValid()
        {
            var mockSurveyManagementService = new Mock<ISurveyManagementService>();
            var survey = new ClientModels.Survey()
            {
                Questions = new List<ClientModels.Question>()
                {
                    new ClientModels.Question()
                }
            };

            mockSurveyManagementService.Setup(r => r.GetSurveyAsync(It.IsAny<string>()))
                .ReturnsAsync(survey);
            var surveyAnswer = new SurveyAnswer
            {
                QuestionAnswers = new List<QuestionAnswer>()
                {
                    new QuestionAnswer
                    {
                        Answer = "answer"
                    }
                }
            };

            using (var controller = new SurveysController(
                mockSurveyManagementService.Object,
                new Mock<ISurveyAnswerService>().Object,
                new Mock<ISurveyResponseService>().Object))
            {
                controller.ModelState.AddModelError("error for test", "invalid model state");

                var result = await controller.Display(string.Empty, surveyAnswer) as ViewResult;

                var model = result.ViewData.Model as PageViewData<SurveyAnswer>;
                Assert.AreEqual("answer", model.ContentModel.QuestionAnswers.First().Answer);
            }
        }

        [TestMethod]
        public async Task DisplayWhenPostReturnsEmptyViewNameWhenModelIsNotValid()
        {
            var mockSurveyManagementService = new Mock<ISurveyManagementService>();
            var survey = new ClientModels.Survey()
            {
                Questions = new List<ClientModels.Question>()
                {
                    new ClientModels.Question()
                }
            };
            mockSurveyManagementService.Setup(r => r.GetSurveyAsync(It.IsAny<string>()))
                .ReturnsAsync(survey);
            var surveyAnswer = new SurveyAnswer()
            {
                QuestionAnswers = new List<QuestionAnswer>()
                {
                    new QuestionAnswer()
                }
            };

            using (var controller = new SurveysController(
                mockSurveyManagementService.Object,
                new Mock<ISurveyAnswerService>().Object,
                new Mock<ISurveyResponseService>().Object))
            {
                controller.ModelState.AddModelError("error for test", "invalid model state");

                var result = await controller.Display(string.Empty, surveyAnswer) as ViewResult;

                Assert.IsNull(result.ViewName);
            }
        }

        [TestMethod]
        public async Task DisplayWhenPostReturnsSurveyTitleAsTitleInTheModelWhenModelIsNotValid()
        {
            var mockSurveyManagementService = new Mock<ISurveyManagementService>();
            var survey = new ClientModels.Survey()
            {
                Title = "title",
                Questions = new List<ClientModels.Question>()
                {
                    new ClientModels.Question()
                }
            };

            mockSurveyManagementService.Setup(r => r.GetSurveyAsync(It.IsAny<string>()))
                .ReturnsAsync(survey);

            var surveyAnswer = new SurveyAnswer()
            {
                QuestionAnswers = new List<QuestionAnswer>()
                {
                    new QuestionAnswer()
                }
            };

            using (var controller = new SurveysController(
                mockSurveyManagementService.Object,
                new Mock<ISurveyAnswerService>().Object,
                new Mock<ISurveyResponseService>().Object))
            {
                controller.ModelState.AddModelError("error for test", "invalid model state");

                var result = await controller.Display(string.Empty, surveyAnswer) as ViewResult;

                var model = result.ViewData.Model as MasterPageViewData;
                Assert.AreSame("title", model.Title);
            }
        }

        [TestMethod]
        public async Task IndexReturnsTitleInTheModel()
        {
            var mockSurveyManagementService = new Mock<ISurveyManagementService>();
            var surveys = new List<ClientModels.SurveyInformation>();
            mockSurveyManagementService.Setup(r => r.GetLatestSurveysAsync()).ReturnsAsync(surveys);

            using (var controller = new SurveysController(
                mockSurveyManagementService.Object,
                new Mock<ISurveyAnswerService>().Object,
                new Mock<ISurveyResponseService>().Object))
            {
                var result = await controller.Index() as ViewResult;

                var model = result.ViewData.Model as MasterPageViewData;
                Assert.AreEqual("Existing surveys", model.Title);
            }
        }

        [TestMethod]
        public async Task IndexGetsRecentSurveysFromStore()
        {
            var mockSurveyManagementService = new Mock<ISurveyManagementService>();
            var surveys = new List<ClientModels.SurveyInformation>();
            mockSurveyManagementService.Setup(r => r.GetLatestSurveysAsync()).ReturnsAsync(surveys);

            using (var controller = new SurveysController(
                mockSurveyManagementService.Object,
                new Mock<ISurveyAnswerService>().Object,
                new Mock<ISurveyResponseService>().Object))
            {
                await controller.Index();
            }

            mockSurveyManagementService.Verify(r => r.GetLatestSurveysAsync(), Times.Once());
        }

        [TestMethod]
        public async Task IndexReturnsRecentSurveysFromStoreInTheModel()
        {
            var mockSurveyManagementService = new Mock<ISurveyManagementService>();
            var surveys = new List<ClientModels.SurveyInformation>();
            mockSurveyManagementService.Setup(r => r.GetLatestSurveysAsync()).ReturnsAsync(surveys);

            using (var controller = new SurveysController(
                mockSurveyManagementService.Object,
                new Mock<ISurveyAnswerService>().Object,
                new Mock<ISurveyResponseService>().Object))
            {
                var result = await controller.Index() as ViewResult;

                var model = result.ViewData.Model as PageViewData<IEnumerable<Survey>>;
                Assert.IsNotNull(model.ContentModel);
            }
        }

        [TestMethod]
        public void ThankYouReturnsModel()
        {
            using (var controller = new SurveysController(
                new Mock<ISurveyManagementService>().Object,
                new Mock<ISurveyAnswerService>().Object,
                new Mock<ISurveyResponseService>().Object))
            {
                var result = controller.ThankYou() as ViewResult;

                var model = result.ViewData.Model as MasterPageViewData;
                Assert.AreEqual("Thank you for filling out the survey", model.Title);
            }
        }
    }
}
