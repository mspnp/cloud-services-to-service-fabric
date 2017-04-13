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
    using Utility;

    [TestClass]
    public class SurveysControllerFixture
    {
        [TestMethod]
        public async Task DisplayCallsStoreWithSlugForSurveyToGet()
        {
            var mockSurveyManagementService = new Mock<ISurveyManagementService>();
            mockSurveyManagementService.Setup(r => r.GetSurveyAsync(It.IsAny<string>()))
                .ReturnsAsync(new Tailspin.SurveyManagementService.Client.Models.Survey());

            using (var controller = new SurveysController(mockSurveyManagementService.Object, new Mock<ISurveyAnswerService>().Object))
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
                .ReturnsAsync(new Tailspin.SurveyManagementService.Client.Models.Survey());

            using (var controller = new SurveysController(mockSurveyManagementService.Object, new Mock<ISurveyAnswerService>().Object))
            {
                var result = await controller.Display(string.Empty) as ViewResult;

                Assert.IsNull(result.ViewName);
            }
        }

        [TestMethod]
        public async Task DisplayCopiesTheSurveyTitleToTheSurveyAnswerReturnedInTheContentModel()
        {
            var survey = new Tailspin.SurveyManagementService.Client.Models.Survey()
            {
                Title = "title to be copied"
            };

            var mockSurveyManagementService = new Mock<ISurveyManagementService>();
            mockSurveyManagementService.Setup(r => r.GetSurveyAsync(It.IsAny<string>()))
                .ReturnsAsync(survey);

            using (var controller = new SurveysController(mockSurveyManagementService.Object, new Mock<ISurveyAnswerService>().Object))
            {
                var result = await controller.Display(string.Empty) as ViewResult;

                var model = result.ViewData.Model as PageViewData<SurveyAnswer>;
                Assert.AreEqual("title to be copied", model.ContentModel.Title);
            }
        }

        [TestMethod]
        public async Task DisplayCopiesTheSurveyQuestionTextToTheSurveyAnswerReturnedInTheContentModel()
        {
            var survey = new Tailspin.SurveyManagementService.Client.Models.Survey()
            {
                Questions = new List<Tailspin.SurveyManagementService.Client.Models.Question>()
                {
                    new Tailspin.SurveyManagementService.Client.Models.Question()
                    {
                        Text = "question text to copy"
                    }
                }
            };

            var mockSurveyManagementService = new Mock<ISurveyManagementService>();
            mockSurveyManagementService.Setup(r => r.GetSurveyAsync(It.IsAny<string>()))
                .ReturnsAsync(survey);

            using (var controller = new SurveysController(mockSurveyManagementService.Object, new Mock<ISurveyAnswerService>().Object))
            {
                var result = await controller.Display(string.Empty) as ViewResult;

                var model = result.ViewData.Model as PageViewData<SurveyAnswer>;
                Assert.AreEqual("question text to copy", model.ContentModel.QuestionAnswers.First().QuestionText);
            }
        }

        [TestMethod]
        public async Task DisplayCopiesTheSurveyQuestionTypeToTheQuestionAnswerReturnedInTheContentModel()
        {
            var survey = new Tailspin.SurveyManagementService.Client.Models.Survey()
            {
                Questions = new List<Tailspin.SurveyManagementService.Client.Models.Question>()
                {
                    new Tailspin.SurveyManagementService.Client.Models.Question()
                    {
                        Type = Tailspin.SurveyManagementService.Client.Models.QuestionType.SimpleText
                    },
                }
            };

            var mockSurveyManagementService = new Mock<ISurveyManagementService>();
            mockSurveyManagementService.Setup(r => r.GetSurveyAsync(It.IsAny<string>()))
                .ReturnsAsync(survey);

            using (var controller = new SurveysController(mockSurveyManagementService.Object, new Mock<ISurveyAnswerService>().Object))
            {
                var result = await controller.Display(string.Empty) as ViewResult;

                var model = result.ViewData.Model as PageViewData<SurveyAnswer>;
                Assert.AreEqual(QuestionType.SimpleText, model.ContentModel.QuestionAnswers.First().QuestionType);
            }
        }

        [TestMethod]
        public async Task DisplayTransformsAllTheSurveyQuestionsToQuestionAnswerRetrnedInTheContentModel()
        {
            var survey = new Tailspin.SurveyManagementService.Client.Models.Survey()
            {
                Questions = new List<Tailspin.SurveyManagementService.Client.Models.Question>()
                {
                    new Tailspin.SurveyManagementService.Client.Models.Question(),
                    new Tailspin.SurveyManagementService.Client.Models.Question()
                }
            };

            var mockSurveyManagementService = new Mock<ISurveyManagementService>();
            mockSurveyManagementService.Setup(r => r.GetSurveyAsync(It.IsAny<string>()))
                .ReturnsAsync(survey);

            using (var controller = new SurveysController(mockSurveyManagementService.Object, new Mock<ISurveyAnswerService>().Object))
            {
                var result = await controller.Display(string.Empty) as ViewResult;

                var model = result.ViewData.Model as PageViewData<SurveyAnswer>;
                Assert.AreEqual(2, model.ContentModel.QuestionAnswers.Count());
            }
        }

        [TestMethod]
        public async Task DisplayReturnsSurveyTitleAsTitleInTheModel()
        {
            var survey = new Tailspin.SurveyManagementService.Client.Models.Survey { Title = "title" };

            var mockSurveyManagementService = new Mock<ISurveyManagementService>();
            mockSurveyManagementService.Setup(r => r.GetSurveyAsync(It.IsAny<string>()))
                .ReturnsAsync(survey);

            using (var controller = new SurveysController(mockSurveyManagementService.Object, new Mock<ISurveyAnswerService>().Object))
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
                .ReturnsAsync(new Tailspin.SurveyManagementService.Client.Models.Survey());

            using (var controller = new SurveysController(mockSurveyManagementService.Object, new Mock<ISurveyAnswerService>().Object))
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
                .ReturnsAsync(new Tailspin.SurveyManagementService.Client.Models.Survey());

            using (var controller = new SurveysController(mockSurveyManagementService.Object, new Mock<ISurveyAnswerService>().Object))
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
            var survey = new Tailspin.SurveyManagementService.Client.Models.Survey()
            {
                SlugName = "slug"
            };

            var mockSurveyManagementService = new Mock<ISurveyManagementService>();
            mockSurveyManagementService.Setup(r => r.GetSurveyAsync(It.IsAny<string>()))
                .ReturnsAsync(survey);
            var mockSurveyAnswerService = new Mock<ISurveyAnswerService>();

            using (var controller = new SurveysController(mockSurveyManagementService.Object, mockSurveyAnswerService.Object))
            {
                await controller.Display("slug", new SurveyAnswer());
            }

            mockSurveyAnswerService.Verify(r => r.SaveSurveyAnswerAsync(
                It.Is<Tailspin.SurveyAnswerService.Client.Models.SurveyAnswer>(
                    sa => "slug" == sa.SlugName)),
                Times.Once);
        }

        [TestMethod]
        public async Task DisplayWhenPostCallsSaveSurveyAnswerFromSurveyAnswerStoreWithQuestionTextReadFromTheSurveyWhenModelIsValid()
        {
            var survey = new Tailspin.SurveyManagementService.Client.Models.Survey
            {
                Questions = new List<Tailspin.SurveyManagementService.Client.Models.Question>()
                {
                    new Tailspin.SurveyManagementService.Client.Models.Question { Text = "question text" },
                }
            };

            var mockSurveyManagementService = new Mock<ISurveyManagementService>();
            mockSurveyManagementService.Setup(r => r.GetSurveyAsync(It.IsAny<string>()))
                .ReturnsAsync(survey);
            var mockSurveyAnswerService = new Mock<ISurveyAnswerService>();
            var surveyAnswer = new SurveyAnswer
            {
                QuestionAnswers = new List<QuestionAnswer>()
                {
                    new QuestionAnswer()
                }
            };

            using (var controller = new SurveysController(mockSurveyManagementService.Object, mockSurveyAnswerService.Object))
            {
                await controller.Display(string.Empty, surveyAnswer);
            }

            mockSurveyAnswerService.Verify(r => r.SaveSurveyAnswerAsync(
                It.Is<Tailspin.SurveyAnswerService.Client.Models.SurveyAnswer>(
                    sa => "question text" == sa.QuestionAnswers[0].QuestionText)),
                Times.Once);
        }

        [TestMethod]
        public async Task DisplayWhenPostCallsSaveSurveyAnswerFromSurveyAnswerStoreWithQuestionTypeReadFromTheSurveyWhenModelIsValid()
        {
            var survey = new Tailspin.SurveyManagementService.Client.Models.Survey
            {
                Questions = new List<Tailspin.SurveyManagementService.Client.Models.Question>
                {
                    new Tailspin.SurveyManagementService.Client.Models.Question
                    {
                        Type = Tailspin.SurveyManagementService.Client.Models.QuestionType.SimpleText
                    },
                }
            };

            var mockSurveyManagementService = new Mock<ISurveyManagementService>();
            mockSurveyManagementService.Setup(r => r.GetSurveyAsync(It.IsAny<string>()))
                .ReturnsAsync(survey);
            var mockSurveyAnswerService = new Mock<ISurveyAnswerService>();
            var surveyAnswer = new SurveyAnswer()
            {
                QuestionAnswers = new List<QuestionAnswer>()
                {
                    new QuestionAnswer()
                }
            };

            using (var controller = new SurveysController(mockSurveyManagementService.Object, mockSurveyAnswerService.Object))
            {
                await controller.Display(string.Empty, surveyAnswer);
            }

            mockSurveyAnswerService.Verify(r => r.SaveSurveyAnswerAsync(
                It.Is<Tailspin.SurveyAnswerService.Client.Models.SurveyAnswer>(
                    sa => Tailspin.SurveyAnswerService.Client.Models.QuestionType.SimpleText == sa.QuestionAnswers[0].QuestionType)),
                Times.Once);
        }

        [TestMethod]
        public async Task DisplayWhenPostCallsSaveSurveyAnswerFromSurveyAnswerStoreWithAnswerReadFromTheParameterWhenModelIsValid()
        {
            var survey = new Tailspin.SurveyManagementService.Client.Models.Survey()
            {
                Questions = new List<Tailspin.SurveyManagementService.Client.Models.Question>()
                {
                    new Tailspin.SurveyManagementService.Client.Models.Question()
                }
            };

            var mockSurveyManagementService = new Mock<ISurveyManagementService>();
            mockSurveyManagementService.Setup(r => r.GetSurveyAsync(It.IsAny<string>()))
                .ReturnsAsync(survey);
            var mockSurveyAnswerService = new Mock<ISurveyAnswerService>();
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

            using (var controller = new SurveysController(mockSurveyManagementService.Object, mockSurveyAnswerService.Object))
            {
                await controller.Display(string.Empty, surveyAnswer);
            }

            mockSurveyAnswerService.Verify(r => r.SaveSurveyAnswerAsync(
                It.Is<Tailspin.SurveyAnswerService.Client.Models.SurveyAnswer>(sa => "answer" == sa.QuestionAnswers[0].Answer)),
                Times.Once);
        }

        [TestMethod]
        public async Task DisplayWhenPostThrowsIfSaveSurveyAnswersReadFromSurveyStoreHaveDifferentCountToTheSurveyAnswerParameter()
        {
            var surveyWith1Question = new Tailspin.SurveyManagementService.Client.Models.Survey()
            {
                Questions = new List<Tailspin.SurveyManagementService.Client.Models.Question>()
                {
                    new Tailspin.SurveyManagementService.Client.Models.Question()
                }
            };

            var mockSurveyManagementService = new Mock<ISurveyManagementService>();
            mockSurveyManagementService.Setup(r => r.GetSurveyAsync(It.IsAny<string>()))
                .ReturnsAsync(surveyWith1Question);
            var surveyAnswerWithoutQuestions = new SurveyAnswer();

            using (var controller = new SurveysController(mockSurveyManagementService.Object, new Mock<ISurveyAnswerService>().Object))
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
            var survey = new Tailspin.SurveyManagementService.Client.Models.Survey()
            {
                Questions = new List<Tailspin.SurveyManagementService.Client.Models.Question>()
                {
                    new Tailspin.SurveyManagementService.Client.Models.Question()
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

            using (var controller = new SurveysController(mockSurveyManagementService.Object, new Mock<ISurveyAnswerService>().Object))
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
            var survey = new Tailspin.SurveyManagementService.Client.Models.Survey()
            {
                Questions = new List<Tailspin.SurveyManagementService.Client.Models.Question>()
                {
                    new Tailspin.SurveyManagementService.Client.Models.Question()
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

            using (var controller = new SurveysController(mockSurveyManagementService.Object, new Mock<ISurveyAnswerService>().Object))
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
            var survey = new Tailspin.SurveyManagementService.Client.Models.Survey()
            {
                Title = "title",
                Questions = new List<Tailspin.SurveyManagementService.Client.Models.Question>()
                {
                    new Tailspin.SurveyManagementService.Client.Models.Question()
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

            using (var controller = new SurveysController(mockSurveyManagementService.Object, new Mock<ISurveyAnswerService>().Object))
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
            var surveys = new List<Tailspin.SurveyManagementService.Client.Models.SurveyInformation>();
            mockSurveyManagementService.Setup(r => r.GetLatestSurveysAsync()).ReturnsAsync(surveys);

            using (var controller = new SurveysController(mockSurveyManagementService.Object, new Mock<ISurveyAnswerService>().Object))
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
            var surveys = new List<Tailspin.SurveyManagementService.Client.Models.SurveyInformation>();
            mockSurveyManagementService.Setup(r => r.GetLatestSurveysAsync()).ReturnsAsync(surveys);

            using (var controller = new SurveysController(mockSurveyManagementService.Object, new Mock<ISurveyAnswerService>().Object))
            {
                await controller.Index();
            }

            mockSurveyManagementService.Verify(r => r.GetLatestSurveysAsync(), Times.Once());
        }

        [TestMethod]
        public async Task IndexReturnsRecentSurveysFromStoreInTheModel()
        {
            var mockSurveyManagementService = new Mock<ISurveyManagementService>();
            var surveys = new List<Tailspin.SurveyManagementService.Client.Models.SurveyInformation>();
            mockSurveyManagementService.Setup(r => r.GetLatestSurveysAsync()).ReturnsAsync(surveys);

            using (var controller = new SurveysController(mockSurveyManagementService.Object, new Mock<ISurveyAnswerService>().Object))
            {
                var result = await controller.Index() as ViewResult;

                var model = result.ViewData.Model as PageViewData<IEnumerable<Survey>>;
                Assert.IsNotNull(model.ContentModel);
            }
        }

        [TestMethod]
        public void ThankYouReturnsModel()
        {
            using (var controller = new SurveysController(new Mock<ISurveyManagementService>().Object, new Mock<ISurveyAnswerService>().Object))
            {
                var result = controller.ThankYou() as ViewResult;

                var model = result.ViewData.Model as MasterPageViewData;
                Assert.AreEqual("Thank you for filling out the survey", model.Title);
            }
        }
    }
}
