namespace Tailspin.Web.Tests.Area.Survey.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Tailspin.Web.Areas.Survey.Controllers;
    using Tailspin.Web.Areas.Survey.Models;
    using Tailspin.Web.Models;
    using Tailspin.Web.Survey.Shared.Models;
    using Tailspin.Web.Survey.Shared.Stores;
    using System.Threading.Tasks;

    [TestClass]
    public class SurveysControllerFixture
    {
        [TestMethod]
        public async Task IndexReturnsEmptyViewName()
        {
            var mockTenantStore = new Mock<ITenantStore>();
            mockTenantStore.Setup(r => r.GetTenantAsync(It.IsAny<string>()))
                .ReturnsAsync(new Tenant());

            using (var controller = new SurveysController(new Mock<ISurveyStore>().Object, null, null, mockTenantStore.Object, null))
            {
                var result = await controller.Index() as ViewResult;

                Assert.AreEqual(string.Empty, result.ViewName);
            }
        }

        [TestMethod]
        public async Task IndexReturnsMySurveysAsTitleInTheModel()
        {
            var mockTenantStore = new Mock<ITenantStore>();
            mockTenantStore.Setup(r => r.GetTenantAsync(It.IsAny<string>()))
                .ReturnsAsync(new Tenant());

            using (var controller = new SurveysController(new Mock<ISurveyStore>().Object, null, null, mockTenantStore.Object, null))
            {
                var result = await controller.Index() as ViewResult;

                var model = result.ViewData.Model as TenantMasterPageViewData;
                Assert.AreSame("My Surveys", model.Title);
            }
        }

        [TestMethod]
        public async Task IndexCallsGetAllSurveysByTenantFromSurveyStore()
        {
            var mockSurveyStore = new Mock<ISurveyStore>();
            var mockTenantStore = new Mock<ITenantStore>();
            mockTenantStore.Setup(r => r.GetTenantAsync(It.IsAny<string>()))
                .ReturnsAsync(new Tenant());

            using (var controller = new SurveysController(mockSurveyStore.Object, null, null, mockTenantStore.Object, null))
            {
                controller.TenantId = "tenant";

                await controller.Index();
            }

            mockSurveyStore.Verify(r => r.GetSurveysByTenantAsync(It.Is<string>(actual => "tenant" == actual)), Times.Once());
        }

        [TestMethod]
        public async Task IndexReturnsTheSurveysForTheTenantInTheModel()
        {
            var mockSurveyStore = new Mock<ISurveyStore>();
            mockSurveyStore.Setup(r => r.GetSurveysByTenantAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<Survey>() { new Survey() { Title = "title" } });

            var mockTenantStore = new Mock<ITenantStore>();
            mockTenantStore.Setup(r => r.GetTenantAsync(It.IsAny<string>()))
                .ReturnsAsync(new Tenant() { SubscriptionKind = SubscriptionKind.Standard });

            using (var controller = new SurveysController(mockSurveyStore.Object, null, null, mockTenantStore.Object, null))
            {
                var result = await controller.Index() as ViewResult;

                var model = result.ViewData.Model as TenantPageViewData<IEnumerable<SurveyModel>>;

                Assert.AreEqual(1, model.ContentModel.Count());
                Assert.AreEqual("title", model.ContentModel.First().Title);
            }
        }

        [TestMethod]
        public async Task NewWhenHttpVerbIsGetReturnsEmptyViewName()
        {
            var mockTenantStore = new Mock<ITenantStore>();
            mockTenantStore.Setup(r => r.GetTenantAsync(It.IsAny<string>()))
                .ReturnsAsync(new Tenant() { SubscriptionKind = SubscriptionKind.Standard });

            using (var controller = new SurveysController(new Mock<ISurveyStore>().Object, null, null, mockTenantStore.Object, null))
            {
                var result = await controller.New() as ViewResult;

                Assert.AreEqual(string.Empty, result.ViewName);
            }
        }

        [TestMethod]
        public async Task NewWhenHttpVerbIsGetReturnsNewSurveyAsTitleInTheModel()
        {
            var mockTenantStore = new Mock<ITenantStore>();
            mockTenantStore.Setup(r => r.GetTenantAsync(It.IsAny<string>()))
                .ReturnsAsync(new Tenant() { SubscriptionKind = SubscriptionKind.Standard });

            using (var controller = new SurveysController(new Mock<ISurveyStore>().Object, null, null, mockTenantStore.Object, null))
            {
                var result = await controller.New() as ViewResult;

                var model = result.ViewData.Model as TenantMasterPageViewData;
                Assert.AreSame("New Survey", model.Title);
            }
        }

        [TestMethod]
        public async Task NewWhenHttpVerbIsGetReturnsANewSurveyInTheModelWhenCachedSurveyDoesNotExistInTempData()
        {
            var mockTenantStore = new Mock<ITenantStore>();
            mockTenantStore.Setup(r => r.GetTenantAsync(It.IsAny<string>()))
                .ReturnsAsync(new Tenant() { SubscriptionKind = SubscriptionKind.Standard });

            using (var controller = new SurveysController(new Mock<ISurveyStore>().Object, null, null, mockTenantStore.Object, null))
            {

                controller.TempData[SurveysController.CachedSurvey] = null;

                var result = await controller.New() as ViewResult;

                var model = result.ViewData.Model as TenantPageViewData<SurveyModel>;

                Assert.IsInstanceOfType(model.ContentModel, typeof(SurveyModel));
            }
        }

        [TestMethod]
        public async Task NewWhenHttpVerbIsGetReturnsTheCachedSurveyInTheModelWhenCachedSurveyExistsInTempData()
        {
            var mockTenantStore = new Mock<ITenantStore>();
            mockTenantStore.Setup(r => r.GetTenantAsync(It.IsAny<string>()))
                .ReturnsAsync(new Tenant() { SubscriptionKind = SubscriptionKind.Standard });

            using (var controller = new SurveysController(new Mock<ISurveyStore>().Object, null, null, mockTenantStore.Object, null))
            {
                var survey = new SurveyModel();
                controller.TempData[SurveysController.CachedSurvey] = survey;

                var result = await controller.New() as ViewResult;

                var model = result.ViewData.Model as TenantPageViewData<SurveyModel>;

                Assert.AreSame(survey, model.ContentModel);
            }
        }

        [TestMethod]
        public async Task NewWhenHttpVerbIsPostCallsSaveFromSurveyStoreWithSurveyParameterWhenModelStateIsValid()
        {
            var mockSurveyStore = new Mock<ISurveyStore>();

            var survey = new SurveyModel("slug-name");
            var cachedSurvey = new SurveyModel();
            cachedSurvey.Questions.Add(new Question());

            var mockTenantStore = new Mock<ITenantStore>();
            mockTenantStore.Setup(r => r.GetTenantAsync(It.IsAny<string>()))
                .ReturnsAsync(new Tenant() { SubscriptionKind = SubscriptionKind.Standard });

            using (var controller = new SurveysController(mockSurveyStore.Object, null, null, mockTenantStore.Object, null))
            {
                controller.TenantId = "Tenant";
                controller.TempData[SurveysController.CachedSurvey] = cachedSurvey;

                await controller.New(survey);
            }

            mockSurveyStore.Verify(r => r.SaveSurveyAsync(It.Is<Survey>(s => s.TenantId == "Tenant" && s.SlugName == "slug-name")), Times.Once());
        }

        [TestMethod]
        public async Task NewWhenHttpVerbIsPostSavedTenantIdIsSameAsControllerWhenModelStateIsValid()
        {
            var mockSurveyStore = new Mock<ISurveyStore>();

            var cachedSurvey = new SurveyModel();
            cachedSurvey.Questions.Add(new Question());

            var mockTenantStore = new Mock<ITenantStore>();
            mockTenantStore.Setup(r => r.GetTenantAsync(It.IsAny<string>()))
                .ReturnsAsync(new Tenant() { SubscriptionKind = SubscriptionKind.Standard });

            using (var controller = new SurveysController(mockSurveyStore.Object, null, null, mockTenantStore.Object, null))
            {
                controller.TenantId = "Tenant";
                controller.TempData[SurveysController.CachedSurvey] = cachedSurvey;

                await controller.New(new SurveyModel());
            }

            mockSurveyStore.Verify(r => r.SaveSurveyAsync(It.Is<Survey>(s => s.TenantId == "Tenant")), Times.Once());
        }

        [TestMethod]
        public async Task NewWhenHttpVerbIsPostCopiesQuestionsFromCachedSurveyToSurveyWhenCallingSaveFromSurveyStoreWhenModelStateIsValid()
        {
            var mockSurveyStore = new Mock<ISurveyStore>();

            var survey = new SurveyModel("slug-name");
            var questionsToBeCopied = new List<Question>();
            questionsToBeCopied.Add(new Question());
            var cachedSurvey = new SurveyModel { Questions = questionsToBeCopied };

            var mockTenantStore = new Mock<ITenantStore>();
            mockTenantStore.Setup(r => r.GetTenantAsync(It.IsAny<string>()))
                .ReturnsAsync(new Tenant() { SubscriptionKind = SubscriptionKind.Standard });

            using (var controller = new SurveysController(mockSurveyStore.Object, null, null, mockTenantStore.Object, null))
            {
                controller.TenantId = "Tenant";
                controller.TempData[SurveysController.CachedSurvey] = cachedSurvey;

                await controller.New(survey);
            }

            mockSurveyStore.Verify(r => r.SaveSurveyAsync(It.Is<Survey>(actual => questionsToBeCopied == actual.Questions)));
        }

        [TestMethod]
        public async Task NewWhenHttpVerbIsPostCleansCachedSurveyWhenModelStateIsValid()
        {
            var cachedSurvey = new SurveyModel();
            cachedSurvey.Questions.Add(new Question());

            var mockTenantStore = new Mock<ITenantStore>();
            mockTenantStore.Setup(r => r.GetTenantAsync(It.IsAny<string>()))
                .ReturnsAsync(new Tenant() { SubscriptionKind = SubscriptionKind.Standard });

            using (var controller = new SurveysController(new Mock<ISurveyStore>().Object, null, null, mockTenantStore.Object, null))
            {
                controller.TenantId = "Tenant";
                controller.TempData[SurveysController.CachedSurvey] = cachedSurvey;

                await controller.New(new SurveyModel());

                Assert.IsNull(controller.TempData[SurveysController.CachedSurvey]);
            }
        }

        [TestMethod]
        public async Task NewWhenHttpVerbIsPostReturnsRedirectToMySurveysWhenModelStateIsValid()
        {
            var cachedSurvey = new SurveyModel();
            cachedSurvey.Questions.Add(new Question());

            var mockTenantStore = new Mock<ITenantStore>();
            mockTenantStore.Setup(r => r.GetTenantAsync(It.IsAny<string>()))
                .ReturnsAsync(new Tenant() { SubscriptionKind = SubscriptionKind.Standard });

            using (var controller = new SurveysController(new Mock<ISurveyStore>().Object, null, null, mockTenantStore.Object, null))
            {
                controller.TenantId = "Tenant";
                controller.TempData[SurveysController.CachedSurvey] = cachedSurvey;

                var result = await controller.New(new SurveyModel()) as RedirectToRouteResult;

                Assert.AreEqual("Index", result.RouteValues["action"]);
                Assert.AreEqual(null, result.RouteValues["controller"]);
            }
        }

        [TestMethod]
        public async Task NewWhenHttpVerbIsPostReturnsRedirectToTheNewActionWhenCachedSurveyIsNull()
        {
            using (var controller = new SurveysController(new Mock<ISurveyStore>().Object, null, null, null, null))
            {
                controller.TempData[SurveysController.CachedSurvey] = null;

                var result = await controller.New(new SurveyModel()) as RedirectToRouteResult;

                Assert.AreEqual("New", result.RouteValues["action"]);
                Assert.AreEqual(null, result.RouteValues["controller"]);
            }
        }

        [TestMethod]
        public async Task NewWhenHttpVerbIsPostReturnsEmptyViewNameWhenModelStateIsNotValid()
        {
            var mockTenantStore = new Mock<ITenantStore>();
            mockTenantStore.Setup(r => r.GetTenantAsync(It.IsAny<string>()))
                .ReturnsAsync(new Tenant());

            using (var controller = new SurveysController(new Mock<ISurveyStore>().Object, null, null, mockTenantStore.Object, null))
            {
                controller.ModelState.AddModelError("error for test", "invalid model state");
                controller.TempData[SurveysController.CachedSurvey] = new SurveyModel();

                var result = await controller.New(new SurveyModel()) as ViewResult;

                Assert.AreEqual(string.Empty, result.ViewName);
            }
        }

        [TestMethod]
        public async Task NewWhenHttpVerbIsPostReturnsTheSameModelWhenModelStateIsNotValid()
        {
            var mockTenantStore = new Mock<ITenantStore>();
            mockTenantStore.Setup(r => r.GetTenantAsync(It.IsAny<string>()))
                .ReturnsAsync(new Tenant());

            var survey = new SurveyModel();

            using (var controller = new SurveysController(new Mock<ISurveyStore>().Object, null, null, mockTenantStore.Object, null))
            {
                controller.ModelState.AddModelError("error for test", "invalid model state");
                controller.TempData[SurveysController.CachedSurvey] = new SurveyModel();

                var result = await controller.New(survey) as ViewResult;

                var model = result.ViewData.Model as TenantPageViewData<SurveyModel>;

                Assert.AreSame(survey, model.ContentModel);
            }
        }

        [TestMethod]
        public async Task NewWhenHttpVerbIsPostReturnsTitleInTheModelWhenModelStateIsNotValid()
        {
            var mockTenantStore = new Mock<ITenantStore>();
            mockTenantStore.Setup(r => r.GetTenantAsync(It.IsAny<string>()))
                .ReturnsAsync(new Tenant());

            var survey = new SurveyModel();

            using (var controller = new SurveysController(new Mock<ISurveyStore>().Object, null, null, mockTenantStore.Object, null))
            {
                controller.ModelState.AddModelError("error for test", "invalid model state");
                controller.TempData[SurveysController.CachedSurvey] = new SurveyModel();

                var result = await controller.New(survey) as ViewResult;

                var model = result.ViewData.Model as TenantMasterPageViewData;
                Assert.AreSame("New Survey", model.Title);
            }
        }

        [TestMethod]
        public async Task NewWhenHttpVerbIsPostSavesCachedSurveyInTempDataWhenModelStateIsNotValid()
        {
            var cachedSurvey = new SurveyModel();

            var mockTenantStore = new Mock<ITenantStore>();
            mockTenantStore.Setup(r => r.GetTenantAsync(It.IsAny<string>()))
                .ReturnsAsync(new Tenant());

            using (var controller = new SurveysController(new Mock<ISurveyStore>().Object, null, null, mockTenantStore.Object, null))
            {
                controller.ModelState.AddModelError("error for test", "invalid model state");
                controller.TempData[SurveysController.CachedSurvey] = cachedSurvey;

                var result = await controller.New(new SurveyModel()) as ViewResult;

                var model = result.ViewData.Model as TenantPageViewData<Survey>;
                Assert.AreSame(cachedSurvey, controller.TempData[SurveysController.CachedSurvey]);
            }
        }

        [TestMethod]
        public async Task NewWhenHttpVerbIsPostCopiesQuestionsFromCachedSurveyToSurveyWhenModelStateIsNotValid()
        {
            var mockSurveyStore = new Mock<ISurveyStore>();

            var questionsToBeCopied = new List<Question>();
            questionsToBeCopied.Add(new Question());
            var cachedSurvey = new SurveyModel { Questions = questionsToBeCopied };

            var mockTenantStore = new Mock<ITenantStore>();
            mockTenantStore.Setup(r => r.GetTenantAsync(It.IsAny<string>()))
                .ReturnsAsync(new Tenant());

            using (var controller = new SurveysController(mockSurveyStore.Object, null, null, mockTenantStore.Object, null))
            {
                controller.ModelState.AddModelError("error for test", "invalid model state");
                controller.TempData[SurveysController.CachedSurvey] = cachedSurvey;

                var result = await controller.New(new SurveyModel()) as ViewResult;

                var model = result.ViewData.Model as TenantPageViewData<SurveyModel>;

                Assert.AreSame(model.ContentModel.Questions, questionsToBeCopied);
            }
        }

        [TestMethod]
        public async Task NewWhenHttpVerbIsPostReturnsErrorInModelStateWhenCachedSurveyHasNoQuestions()
        {
            var mockTenantStore = new Mock<ITenantStore>();
            mockTenantStore.Setup(r => r.GetTenantAsync(It.IsAny<string>()))
                .ReturnsAsync(new Tenant());

            var cachedSurvey = new SurveyModel { Questions = new List<Question>() };

            using (var controller = new SurveysController(new Mock<ISurveyStore>().Object, null, null, mockTenantStore.Object, null))
            {
                controller.TempData[SurveysController.CachedSurvey] = cachedSurvey;

                var result = await controller.New(new SurveyModel()) as ViewResult;

                Assert.IsTrue(controller.ModelState.Keys.Contains("ContentModel.Questions"));
            }
        }

        [TestMethod]
        public async Task NewWhenHttpVerbIsPostReturnsTheSameModelWhenCachedSurveyHasNoQuestions()
        {
            var mockTenantStore = new Mock<ITenantStore>();
            mockTenantStore.Setup(r => r.GetTenantAsync(It.IsAny<string>()))
                .ReturnsAsync(new Tenant());

            var cachedSurvey = new SurveyModel { Questions = new List<Question>() };
            var survey = new SurveyModel();

            using (var controller = new SurveysController(new Mock<ISurveyStore>().Object, null, null, mockTenantStore.Object, null))
            {
                controller.TempData[SurveysController.CachedSurvey] = cachedSurvey;

                var result = await controller.New(survey) as ViewResult;
                var model = result.ViewData.Model as TenantPageViewData<SurveyModel>;

                Assert.AreSame(survey, model.ContentModel);
            }
        }

        [TestMethod]
        public async Task NewWhenHttpVerbIsPostReturnsNewSurveyAsTitleInTheModelWhenCachedSurveyHasNoQuestions()
        {
            var mockTenantStore = new Mock<ITenantStore>();
            mockTenantStore.Setup(r => r.GetTenantAsync(It.IsAny<string>()))
                .ReturnsAsync(new Tenant());

            var cachedSurvey = new SurveyModel { Questions = new List<Question>() };

            using (var controller = new SurveysController(new Mock<ISurveyStore>().Object, null, null, mockTenantStore.Object, null))
            {
                controller.TempData[SurveysController.CachedSurvey] = cachedSurvey;

                var result = await controller.New(new SurveyModel()) as ViewResult;
                var model = result.ViewData.Model as TenantMasterPageViewData;

                Assert.AreSame("New Survey", model.Title);
            }
        }

        [TestMethod]
        public async Task NewWhenHttpVerbIsPostSavesCachedSurveyInTempDataWhenCachedSurveyHasNoQuestions()
        {
            var mockTenantStore = new Mock<ITenantStore>();
            mockTenantStore.Setup(r => r.GetTenantAsync(It.IsAny<string>()))
                .ReturnsAsync(new Tenant());

            var cachedSurvey = new SurveyModel { Questions = new List<Question>() };

            using (var controller = new SurveysController(new Mock<ISurveyStore>().Object, null, null, mockTenantStore.Object, null))
            {
                controller.TempData[SurveysController.CachedSurvey] = cachedSurvey;

                var result = await controller.New(new SurveyModel()) as ViewResult;

                var model = result.ViewData.Model as TenantPageViewData<Survey>;
                Assert.AreSame(cachedSurvey, controller.TempData[SurveysController.CachedSurvey]);
            }
        }

        [TestMethod]
        public async Task NewQuestionReturnsEmptyViewName()
        {
            var mockTenantStore = new Mock<ITenantStore>();
            mockTenantStore.Setup(r => r.GetTenantAsync(It.IsAny<string>()))
                .ReturnsAsync(new Tenant());

            using (var controller = new SurveysController(null, null, null, mockTenantStore.Object, null))
            {
                controller.TempData[SurveysController.CachedSurvey] = new SurveyModel();

                var result = await controller.NewQuestion(new SurveyModel()) as ViewResult;

                Assert.AreEqual(string.Empty, result.ViewName);
            }
        }

        [TestMethod]
        public async Task NewQuestionReturnsNewQuestionAsTitleInTheModel()
        {
            var mockTenantStore = new Mock<ITenantStore>();
            mockTenantStore.Setup(r => r.GetTenantAsync(It.IsAny<string>()))
                .ReturnsAsync(new Tenant());

            using (var controller = new SurveysController(null, null, null, mockTenantStore.Object, null))
            {
                controller.TempData[SurveysController.CachedSurvey] = new SurveyModel();

                var result = await controller.NewQuestion(new SurveyModel()) as ViewResult;

                var model = result.ViewData.Model as TenantMasterPageViewData;
                Assert.AreSame("New Question", model.Title);
            }
        }

        [TestMethod]
        public async Task NewQuestionReturnsNewQuestionInTheModel()
        {
            var mockTenantStore = new Mock<ITenantStore>();
            mockTenantStore.Setup(r => r.GetTenantAsync(It.IsAny<string>()))
                .ReturnsAsync(new Tenant());

            using (var controller = new SurveysController(null, null, null, mockTenantStore.Object, null))
            {
                controller.TempData[SurveysController.CachedSurvey] = new SurveyModel();

                var result = await controller.NewQuestion(new SurveyModel()) as ViewResult;

                var model = result.ViewData.Model as TenantPageViewData<Question>;
                Assert.IsInstanceOfType(model.ContentModel, typeof(Question));
            }
        }

        [TestMethod]
        public async Task NewQuestionRedirectToTheNewActionWhenCachedSurveyIsNull()
        {
            using (var controller = new SurveysController(new Mock<ISurveyStore>().Object, null, null, null, null))
            {
                controller.TempData[SurveysController.CachedSurvey] = null;

                var result = await controller.NewQuestion(new SurveyModel()) as RedirectToRouteResult;

                Assert.AreEqual("New", result.RouteValues["action"]);
                Assert.AreEqual(null, result.RouteValues["controller"]);
            }
        }

        [TestMethod]
        public async Task NewQuestionCopiesSurveyTitleToCachedSurveyThatIsReturnedInViewData()
        {
            var mockTenantStore = new Mock<ITenantStore>();
            mockTenantStore.Setup(r => r.GetTenantAsync(It.IsAny<string>()))
                .ReturnsAsync(new Tenant());

            var survey = new SurveyModel { Title = "title" };

            using (var controller = new SurveysController(null, null, null, mockTenantStore.Object, null))
            {
                controller.TempData[SurveysController.CachedSurvey] = new SurveyModel();

                var result = await controller.NewQuestion(survey) as ViewResult;

                var cachedSurvey = result.TempData[SurveysController.CachedSurvey] as SurveyModel;
                Assert.AreSame(survey.Title, cachedSurvey.Title);
            }
        }

        [TestMethod]
        public async Task AddQuestionReturnsRedirectToNewSurveyWhenModelIsValid()
        {
            using (var controller = new SurveysController(null, null, null, null, null))
            {
                controller.TempData[SurveysController.CachedSurvey] = new SurveyModel();

                var result = await controller.AddQuestion(new Question()) as RedirectToRouteResult;

                Assert.AreEqual("New", result.RouteValues["action"]);
                Assert.AreEqual(null, result.RouteValues["controller"]);
            }
        }

        [TestMethod]
        public async Task AddQuestionAddsTheNewQuestionToTheCachedSurveyWhenModelIsValid()
        {
            var cachedSurvey = new SurveyModel();
            cachedSurvey.Questions.Add(new Question());
            var question = new Question();

            using (var controller = new SurveysController(null, null, null, null, null))
            {
                controller.TempData[SurveysController.CachedSurvey] = cachedSurvey;
                await controller.AddQuestion(question);

                var actualQuestions = (controller.TempData[SurveysController.CachedSurvey] as SurveyModel).Questions;

                Assert.AreEqual(2, actualQuestions.Count);
                Assert.IsTrue(actualQuestions.Contains(question));
            }
        }

        [TestMethod]
        public async Task AddQuestionReplacesCarriageReturnsInPossibleAnswersWhenModelIsValid()
        {
            using (var controller = new SurveysController(null, null, null, null, null))
            {
                var question = new Question { PossibleAnswers = "possible answers\r\n" };
                controller.TempData[SurveysController.CachedSurvey] = new SurveyModel();

                await controller.AddQuestion(question);

                var cachedSurvey = controller.TempData[SurveysController.CachedSurvey] as SurveyModel;

                var actualQuestion = cachedSurvey.Questions.Single(q => q == question);
                Assert.AreEqual("possible answers\n", actualQuestion.PossibleAnswers);
            }
        }

        [TestMethod]
        public async Task AddQuestionReturnsNewQuestionViewWhenModelIsNotValid()
        {
            var mockTenantStore = new Mock<ITenantStore>();
            mockTenantStore.Setup(r => r.GetTenantAsync(It.IsAny<string>()))
                .ReturnsAsync(new Tenant());

            using (var controller = new SurveysController(null, null, null, mockTenantStore.Object, null))
            {
                controller.ModelState.AddModelError("error for test", "invalid model state");

                var result = await controller.AddQuestion(null) as ViewResult;

                Assert.AreSame("NewQuestion", result.ViewName);
            }
        }

        [TestMethod]
        public async Task AddQuestionReturnsQuestionAsModelWhenModelIsNotValid()
        {
            var mockTenantStore = new Mock<ITenantStore>();
            mockTenantStore.Setup(r => r.GetTenantAsync(It.IsAny<string>()))
                .ReturnsAsync(new Tenant());

            using (var controller = new SurveysController(null, null, null, mockTenantStore.Object, null))
            {
                controller.TempData[SurveysController.CachedSurvey] = new SurveyModel();
                controller.ModelState.AddModelError("error for test", "invalid model state");
                var question = new Question();

                var result = await controller.AddQuestion(question) as ViewResult;

                var model = result.ViewData.Model as TenantPageViewData<Question>;

                Assert.AreSame(question, model.ContentModel);
            }
        }

        [TestMethod]
        public async Task AddQuestionReturnsNewQuestionAsModelWhenModelIsNotValidAndQuestionIsNull()
        {
            var mockTenantStore = new Mock<ITenantStore>();
            mockTenantStore.Setup(r => r.GetTenantAsync(It.IsAny<string>()))
                .ReturnsAsync(new Tenant());

            using (var controller = new SurveysController(null, null, null, mockTenantStore.Object, null))
            {
                controller.TempData[SurveysController.CachedSurvey] = new SurveyModel();
                controller.ModelState.AddModelError("error for test", "invalid model state");

                var result = await controller.AddQuestion(null) as ViewResult;

                var model = result.ViewData.Model as TenantPageViewData<Question>;

                Assert.IsInstanceOfType(model.ContentModel, typeof(Question));
            }
        }

        [TestMethod]
        public async Task AddQuestionCopiesCachedSurveyToTempDataWhenModelIsNotValid()
        {
            var mockTenantStore = new Mock<ITenantStore>();
            mockTenantStore.Setup(r => r.GetTenantAsync(It.IsAny<string>()))
                .ReturnsAsync(new Tenant());

            using (var controller = new SurveysController(null, null, null, mockTenantStore.Object, null))
            {
                controller.ModelState.AddModelError("error for test", "invalid model state");
                var cachedSurvey = new SurveyModel();
                controller.TempData[SurveysController.CachedSurvey] = cachedSurvey;

                var result = await controller.AddQuestion(null) as ViewResult;

                var cachedSurveyReturnedInTempData = result.TempData[SurveysController.CachedSurvey] as SurveyModel;

                Assert.AreSame(cachedSurvey, cachedSurveyReturnedInTempData);
            }
        }

        [TestMethod]
        public async Task AddQuestionReturnsNewQuestionAsTitleInTheModelWhenModelIsNotValid()
        {
            var mockTenantStore = new Mock<ITenantStore>();
            mockTenantStore.Setup(r => r.GetTenantAsync(It.IsAny<string>()))
                .ReturnsAsync(new Tenant());

            using (var controller = new SurveysController(null, null, null, mockTenantStore.Object, null))
            {
                controller.ModelState.AddModelError("error for test", "invalid model state");

                var result = await controller.AddQuestion(null) as ViewResult;

                var model = result.ViewData.Model as TenantMasterPageViewData;
                Assert.AreSame("New Question", model.Title);
            }
        }

        [TestMethod]
        public async Task DeleteCallsDeleteSurveyByTenantAndSlugNameFromSurveyStore()
        {
            var mockSurveyStore = new Mock<ISurveyStore>();

            using (var controller = new SurveysController(mockSurveyStore.Object, new Mock<ISurveyAnswerStore>().Object, new Mock<ISurveyAnswersSummaryStore>().Object, null, null))
            {
                await controller.Delete("tenant", "survey-slug");
            }

            mockSurveyStore.Verify(
                r => r.DeleteSurveyByTenantAndSlugNameAsync(
                    It.Is<string>(t => "tenant" == t),
                    It.Is<string>(s => "survey-slug" == s)),
                Times.Once());
        }

        [TestMethod]
        public async Task DeleteCallsDeleteSurveyAnswersStore()
        {
            var mockSurveyAnswerStore = new Mock<ISurveyAnswerStore>();

            using (var controller = new SurveysController(new Mock<ISurveyStore>().Object, mockSurveyAnswerStore.Object, new Mock<ISurveyAnswersSummaryStore>().Object, null, null))
            {
                await controller.Delete("tenant", "survey-slug");
            }

            mockSurveyAnswerStore.Verify(r => r.DeleteSurveyAnswersAsync("tenant", "survey-slug"), Times.Once());
        }

        [TestMethod]
        public async Task DeleteCallsDeleteSurveyAnswersSummariesStore()
        {
            var mockSurveyAnswersSummaryStore = new Mock<ISurveyAnswersSummaryStore>();

            using (var controller = new SurveysController(new Mock<ISurveyStore>().Object, new Mock<ISurveyAnswerStore>().Object, mockSurveyAnswersSummaryStore.Object, null, null))
            {
                await controller.Delete("tenant", "survey-slug");
            }

            mockSurveyAnswersSummaryStore.Verify(r => r.DeleteSurveyAnswersSummaryAsync("tenant", "survey-slug"), Times.Once());
        }

        [TestMethod]
        public async Task DeleteReturnsRedirectToMySurveys()
        {
            using (var controller = new SurveysController(new Mock<ISurveyStore>().Object, new Mock<ISurveyAnswerStore>().Object, new Mock<ISurveyAnswersSummaryStore>().Object, null, null))
            {
                var result = await controller.Delete(string.Empty, string.Empty) as RedirectToRouteResult;

                Assert.AreEqual("Index", result.RouteValues["action"]);
                Assert.AreEqual(null, result.RouteValues["controller"]);
            }
        }

        [TestMethod]
        public async Task BrowseResponsesReturnSlugNameAsTheTitle()
        {
            var mockSurveyAnswerStore = new Mock<ISurveyAnswerStore>();
            mockSurveyAnswerStore.Setup(r => r.GetSurveyAnswerBrowsingContextAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                                      .ReturnsAsync(new SurveyAnswerBrowsingContext { PreviousId = string.Empty, NextId = string.Empty });

            var mockTenantStore = new Mock<ITenantStore>();
            mockTenantStore.Setup(r => r.GetTenantAsync(It.IsAny<string>()))
                .ReturnsAsync(new Tenant());

            using (var controller = new SurveysController(null, mockSurveyAnswerStore.Object, null, mockTenantStore.Object, null))
            {
                var result = await controller.BrowseResponses(string.Empty, "slug-name", string.Empty) as ViewResult;

                var model = result.ViewData.Model as TenantMasterPageViewData;
                Assert.AreSame("slug-name", model.Title);
            }
        }

        [TestMethod]
        public async Task BrowseResponsesGetsTheAnswerFromTheStoreWhenAnswerIdIsNotEmpty()
        {
            var mockSurveyAnswerStore = new Mock<ISurveyAnswerStore>();
            mockSurveyAnswerStore.Setup(r => r.GetSurveyAnswerBrowsingContextAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                                      .ReturnsAsync(new SurveyAnswerBrowsingContext { PreviousId = string.Empty, NextId = string.Empty });

            var mockTenantStore = new Mock<ITenantStore>();
            mockTenantStore.Setup(r => r.GetTenantAsync(It.IsAny<string>()))
                .ReturnsAsync(new Tenant());

            using (var controller = new SurveysController(null, mockSurveyAnswerStore.Object, null, mockTenantStore.Object, null))
            {
                await controller.BrowseResponses("tenant", "survey-slug", "answer id");
            }

            mockSurveyAnswerStore.Verify(r => r.GetSurveyAnswerAsync("tenant", "survey-slug", "answer id"));
        }

        [TestMethod]
        public async Task BrowseResponsesGetsTheFirstAnswerIdFromTheStoreWhenAnswerIdIsEmpty()
        {
            var mockSurveyAnswerStore = new Mock<ISurveyAnswerStore>();
            mockSurveyAnswerStore.Setup(r => r.GetSurveyAnswerBrowsingContextAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                                      .ReturnsAsync(new SurveyAnswerBrowsingContext { PreviousId = string.Empty, NextId = string.Empty });

            var mockTenantStore = new Mock<ITenantStore>();
            mockTenantStore.Setup(r => r.GetTenantAsync(It.IsAny<string>()))
                .ReturnsAsync(new Tenant());

            using (var controller = new SurveysController(null, mockSurveyAnswerStore.Object, null, mockTenantStore.Object, null))
            {
                await controller.BrowseResponses("tenant", "survey-slug", string.Empty);
            }

            mockSurveyAnswerStore.Verify(r => r.GetFirstSurveyAnswerIdAsync("tenant", "survey-slug"), Times.Once());
        }

        [TestMethod]
        public async Task BrowseResponsesGetsSurveyAnswerWithTheIdReturnedFromTheStoreWhenAnswerIdIsEmpty()
        {
            var mockSurveyAnswerStore = new Mock<ISurveyAnswerStore>();
            mockSurveyAnswerStore.Setup(r => r.GetSurveyAnswerBrowsingContextAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                                      .ReturnsAsync(new SurveyAnswerBrowsingContext { PreviousId = string.Empty, NextId = string.Empty });
            mockSurveyAnswerStore.Setup(r => r.GetFirstSurveyAnswerIdAsync("tenant", "survey-slug"))
                                      .ReturnsAsync("id");

            var mockTenantStore = new Mock<ITenantStore>();
            mockTenantStore.Setup(r => r.GetTenantAsync(It.IsAny<string>()))
                .ReturnsAsync(new Tenant());

            using (var controller = new SurveysController(null, mockSurveyAnswerStore.Object, null, mockTenantStore.Object, null))
            {
                await controller.BrowseResponses("tenant", "survey-slug", string.Empty);
            }

            mockSurveyAnswerStore.Verify(r => r.GetSurveyAnswerAsync("tenant", "survey-slug", "id"));
        }

        [TestMethod]
        public async Task BrowseResponsesSetsTheAnswerFromTheStoreInTheModel()
        {
            var mockSurveyAnswerStore = new Mock<ISurveyAnswerStore>();
            var surveyAnswer = new SurveyAnswer();
            mockSurveyAnswerStore.Setup(r => r.GetSurveyAnswerBrowsingContextAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                                      .ReturnsAsync(new SurveyAnswerBrowsingContext { PreviousId = string.Empty, NextId = string.Empty });
            mockSurveyAnswerStore.Setup(r => r.GetSurveyAnswerAsync("tenant", "survey-slug", "answer id"))
                                      .ReturnsAsync(surveyAnswer);

            var mockTenantStore = new Mock<ITenantStore>();
            mockTenantStore.Setup(r => r.GetTenantAsync(It.IsAny<string>()))
                .ReturnsAsync(new Tenant());

            using (var controller = new SurveysController(null, mockSurveyAnswerStore.Object, null, mockTenantStore.Object, null))
            {
                var result = await controller.BrowseResponses("tenant", "survey-slug", "answer id") as ViewResult;

                var model = result.ViewData.Model as TenantPageViewData<BrowseResponseModel>;
                Assert.AreSame(surveyAnswer, model.ContentModel.SurveyAnswer);
            }
        }

        [TestMethod]
        public async Task BrowseResponsesCallsGetSurveyAnswerBrowsingContextFromStore()
        {
            var mockSurveyAnswerStore = new Mock<ISurveyAnswerStore>();
            mockSurveyAnswerStore.Setup(r => r.GetSurveyAnswerBrowsingContextAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                                      .ReturnsAsync(new SurveyAnswerBrowsingContext { PreviousId = string.Empty, NextId = string.Empty });

            var mockTenantStore = new Mock<ITenantStore>();
            mockTenantStore.Setup(r => r.GetTenantAsync(It.IsAny<string>()))
                .ReturnsAsync(new Tenant());

            using (var controller = new SurveysController(null, mockSurveyAnswerStore.Object, null, mockTenantStore.Object, null))
            {
                await controller.BrowseResponses("tenant", "survey-slug", "answer id");
            }

            mockSurveyAnswerStore.Verify(
                r => r.GetSurveyAnswerBrowsingContextAsync("tenant", "survey-slug", "answer id"),
                Times.Once());
        }

        [TestMethod]
        public async Task BrowseResponsesSetsPreviousAndNextIdsInModel()
        {
            var mockSurveyAnswerStore = new Mock<ISurveyAnswerStore>();
            mockSurveyAnswerStore.Setup(r => r.GetSurveyAnswerBrowsingContextAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                                      .ReturnsAsync(new SurveyAnswerBrowsingContext { PreviousId = "PreviousId", NextId = "NextId" });

            var mockTenantStore = new Mock<ITenantStore>();
            mockTenantStore.Setup(r => r.GetTenantAsync(It.IsAny<string>()))
                .ReturnsAsync(new Tenant());

            using (var controller = new SurveysController(null, mockSurveyAnswerStore.Object, null, mockTenantStore.Object, null))
            {
                var result = await controller.BrowseResponses("tenant", "survey-slug", "answer id") as ViewResult;

                var model = result.ViewData.Model as TenantPageViewData<BrowseResponseModel>;
                Assert.AreEqual("PreviousId", model.ContentModel.PreviousAnswerId);
                Assert.AreEqual("NextId", model.ContentModel.NextAnswerId);
            }
        }

        [TestMethod]
        public async Task AnalyzeReturnSlugNameAsTheTitle()
        {
            var mockTenantStore = new Mock<ITenantStore>();
            mockTenantStore.Setup(r => r.GetTenantAsync(It.IsAny<string>()))
                .ReturnsAsync(new Tenant());

            using (var controller = new SurveysController(null, null, new Mock<ISurveyAnswersSummaryStore>().Object, mockTenantStore.Object, null))
            {
                var result = await controller.Analyze(string.Empty, "slug-name") as ViewResult;

                var model = result.ViewData.Model as TenantMasterPageViewData;
                Assert.AreSame("slug-name", model.Title);
            }
        }

        [TestMethod]
        public async Task AnalyzeGetsTheSummaryFromTheStore()
        {
            var mockSurveyAnswersSummaryStore = new Mock<ISurveyAnswersSummaryStore>();

            var mockTenantStore = new Mock<ITenantStore>();
            mockTenantStore.Setup(r => r.GetTenantAsync(It.IsAny<string>()))
                .ReturnsAsync(new Tenant());

            using (var controller = new SurveysController(null, null, mockSurveyAnswersSummaryStore.Object, mockTenantStore.Object, null))
            {
                await controller.Analyze("tenant", "slug-name");
            }

            mockSurveyAnswersSummaryStore.Verify(r => r.GetSurveyAnswersSummaryAsync("tenant", "slug-name"), Times.Once());
        }

        [TestMethod]
        public async Task AnalyzeReturnsTheSummaryGetFromTheStoreInTheModel()
        {
            var mockSurveyAnswersSummaryStore = new Mock<ISurveyAnswersSummaryStore>();
            var surveyAnswersSummary = new SurveyAnswersSummary();
            mockSurveyAnswersSummaryStore.Setup(r => r.GetSurveyAnswersSummaryAsync(It.IsAny<string>(), It.IsAny<string>()))
                                              .ReturnsAsync(surveyAnswersSummary);

            var mockTenantStore = new Mock<ITenantStore>();
            mockTenantStore.Setup(r => r.GetTenantAsync(It.IsAny<string>()))
                .ReturnsAsync(new Tenant());

            using (var controller = new SurveysController(null, null, mockSurveyAnswersSummaryStore.Object, mockTenantStore.Object, null))
            {
                var result = await controller.Analyze(string.Empty, string.Empty) as ViewResult;

                var model = result.ViewData.Model as TenantPageViewData<SurveyAnswersSummary>;
                Assert.AreSame(surveyAnswersSummary, model.ContentModel);
            }
        }

        [TestMethod]
        public async Task ExportGetsTheTenantInformationAndPutsInModel()
        {
            var tenant = new Tenant();

            var mockTenantStore = new Mock<ITenantStore>();
            var mockSurveyAnswerStore = new Mock<ISurveyAnswerStore>();
            mockTenantStore.Setup(r => r.GetTenantAsync(It.IsAny<string>())).ReturnsAsync(tenant);
            mockSurveyAnswerStore.Setup(r => r.GetFirstSurveyAnswerIdAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(string.Empty);

            using (var controller = new SurveysController(null, mockSurveyAnswerStore.Object, null, mockTenantStore.Object, null))
            {
                var result = await controller.ExportResponses(string.Empty) as ViewResult;

                var model = result.ViewData.Model as TenantPageViewData<ExportResponseModel>;

                Assert.AreSame(tenant, model.ContentModel.Tenant);
            }
        }

        [TestMethod]
        public async Task ExportDeterminesIfThereAreResponsesToExport()
        {
            var mockTenantStore = new Mock<ITenantStore>();
            var mockSurveyAnswerStore = new Mock<ISurveyAnswerStore>();
            var tenant = new Tenant();
            mockTenantStore.Setup(r => r.GetTenantAsync(It.IsAny<string>())).ReturnsAsync(tenant);
            mockSurveyAnswerStore.Setup(r => r.GetFirstSurveyAnswerIdAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(string.Empty);

            using (var controller = new SurveysController(null, mockSurveyAnswerStore.Object, null, mockTenantStore.Object, null))
            {
                var result = await controller.ExportResponses(string.Empty) as ViewResult;

                var model = result.ViewData.Model as TenantPageViewData<ExportResponseModel>;
                Assert.AreEqual(false, model.ContentModel.HasResponses);
            }
        }

        [TestMethod]
        public async Task ExportPostCallsTransferPostWithTenantAndSlugName()
        {
            var mockSurveyTransferStore = new Mock<ISurveyTransferStore>();

            using (var controller = new SurveysController(null, null, null, null, mockSurveyTransferStore.Object))
            {
                await controller.ExportResponses("tenant", "slugName");
            }

            mockSurveyTransferStore.Verify(r => r.TransferAsync("tenant", "slugName"), Times.Once());
        }

        [TestMethod]
        public async Task ExportPostRedirectsToBrowseAction()
        {
            var mockSurveyTransferStore = new Mock<ISurveyTransferStore>();

            using (var controller = new SurveysController(null, null, null, null, mockSurveyTransferStore.Object))
            {
                var result = await controller.ExportResponses("tenant", "slugName") as RedirectToRouteResult;

                Assert.AreEqual(result.RouteValues["action"], "BrowseResponses");
            }
        }

    }
}