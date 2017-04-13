namespace Tailspin.Web.Tests.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Newtonsoft.Json;
    using Tailspin.SurveyAnswerService.Client;
    using Tailspin.SurveyManagementService.Client;
    using Tailspin.SurveyAnalysisService.Client;
    using Tailspin.Web.Controllers;
    using Tailspin.Web.Models;
    using Tailspin.Web.Shared.Models;

    [TestClass]
    public class SurveysControllerFixture
    {
        [TestMethod]
        public async Task IndexReturnsEmptyViewName()
        {
            var surveyManagementService = new Mock<ISurveyManagementService>();
            surveyManagementService.Setup(s => s.ListSurveysAsync())
                .ReturnsAsync(new List<Tailspin.SurveyManagementService.Client.Models.SurveyInformation>());
            using (var controller = new SurveysController(surveyManagementService.Object, new Mock<ISurveyAnswerService>().Object, new Mock<ISurveyAnalysisService>().Object))
            {
                var mockTempDataDictionary = new Mock<ITempDataDictionary>();
                controller.TempData = mockTempDataDictionary.Object;

                var result = await controller.Index() as ViewResult;

                Assert.IsNull(result.ViewName);
            }
        }

        [TestMethod]
        public async Task IndexReturnsMySurveysAsTitleInTheModel()
        {
            var surveyManagementService = new Mock<ISurveyManagementService>();
            surveyManagementService.Setup(s => s.ListSurveysAsync())
                .ReturnsAsync(new List<Tailspin.SurveyManagementService.Client.Models.SurveyInformation>());
            using (var controller = new SurveysController(surveyManagementService.Object, new Mock<ISurveyAnswerService>().Object, new Mock<ISurveyAnalysisService>().Object))
            {
                var mockTempDataDictionary = new Mock<ITempDataDictionary>();
                controller.TempData = mockTempDataDictionary.Object;

                var result = await controller.Index() as ViewResult;

                var model = result.ViewData.Model as MasterPageViewData;
                Assert.AreSame("My Surveys", model.Title);
            }
        }

        //[TestMethod]
        //public async Task IndexCallsGetAllSurveysByTenantFromSurveyStore()
        //{
        //    var mockSurveyStore = new Mock<ISurveyManagementService>();
        //    var mockTenantStore = new Mock<ITenantStore>();
        //    mockTenantStore.Setup(r => r.GetTenantAsync(It.IsAny<string>()))
        //        .ReturnsAsync(new Tenant());

        //    using (var controller = new SurveysController(mockSurveyStore.Object, null, null, mockTenantStore.Object, null))
        //    {
        //        var mockTempDataDictionary = new Mock<ITempDataDictionary>();
        //        controller.TempData = mockTempDataDictionary.Object;

        //        await controller.Index();
        //    }

        //    mockSurveyStore.Verify(r => r.GetSurveysByTenantAsync(It.Is<string>(actual => "adatum" == actual)), Times.Once());
        //}

        //[TestMethod]
        //public async Task IndexReturnsTheSurveysForTheTenantInTheModel()
        //{
        //    var mockSurveyStore = new Mock<ISurveyManagementService>();
        //    mockSurveyStore.Setup(r => r.GetSurveysByTenantAsync(It.IsAny<string>()))
        //        .ReturnsAsync(new List<Survey>() { new Survey() { Title = "title" } });

        //    var mockTenantStore = new Mock<ITenantStore>();
        //    mockTenantStore.Setup(r => r.GetTenantAsync(It.IsAny<string>()))
        //        .ReturnsAsync(new Tenant() { SubscriptionKind = SubscriptionKind.Standard });

        //    using (var controller = new SurveysController(mockSurveyStore.Object, null, null, mockTenantStore.Object, null))
        //    {
        //        var mockTempDataDictionary = new Mock<ITempDataDictionary>();
        //        controller.TempData = mockTempDataDictionary.Object;

        //        var result = await controller.Index() as ViewResult;

        //        var model = result.ViewData.Model as PageViewData<IEnumerable<SurveyModel>>;

        //        Assert.AreEqual(1, model.ContentModel.Count());
        //        Assert.AreEqual("title", model.ContentModel.First().Title);
        //    }
        //}

        [TestMethod]
        public void NewWhenHttpVerbIsGetReturnsEmptyViewName()
        {
            using (var controller = new SurveysController(new Mock<ISurveyManagementService>().Object, new Mock<ISurveyAnswerService>().Object, new Mock<ISurveyAnalysisService>().Object))
            {
                var mockTempDataDictionary = new Mock<ITempDataDictionary>();
                controller.TempData = mockTempDataDictionary.Object;

                var result = controller.New() as ViewResult;

                Assert.IsNull(result.ViewName);
            }
        }

        [TestMethod]
        public void NewWhenHttpVerbIsGetReturnsNewSurveyAsTitleInTheModel()
        {
            using (var controller = new SurveysController(new Mock<ISurveyManagementService>().Object, new Mock<ISurveyAnswerService>().Object, new Mock<ISurveyAnalysisService>().Object))
            {
                var mockTempDataDictionary = new Mock<ITempDataDictionary>();
                controller.TempData = mockTempDataDictionary.Object;

                var result = controller.New() as ViewResult;

                var model = result.ViewData.Model as MasterPageViewData;
                Assert.AreSame("New Survey", model.Title);
            }
        }

        [TestMethod]
        public void NewWhenHttpVerbIsGetReturnsANewSurveyInTheModelWhenCachedSurveyDoesNotExistInTempData()
        {
            using (var controller = new SurveysController(new Mock<ISurveyManagementService>().Object, new Mock<ISurveyAnswerService>().Object, new Mock<ISurveyAnalysisService>().Object))
            {
                var mockTempDataDictionary = new Mock<ITempDataDictionary>();
                mockTempDataDictionary.SetupGet(d => d[SurveysController.TemporarySurveyKey]).Returns(null);
                controller.TempData = mockTempDataDictionary.Object;

                var result = controller.New() as ViewResult;

                var model = result.ViewData.Model as PageViewData<SurveyModel>;

                Assert.IsInstanceOfType(model.ContentModel, typeof(SurveyModel));
            }
        }

        [TestMethod]
        public void NewWhenHttpVerbIsGetReturnsTheCachedSurveyInTheModelWhenCachedSurveyExistsInTempData()
        {
            var tempDataDictionary = new Dictionary<string, object>();

            using (var controller = new SurveysController(new Mock<ISurveyManagementService>().Object, new Mock<ISurveyAnswerService>().Object, new Mock<ISurveyAnalysisService>().Object))
            {
                var survey = new SurveyModel() { Title = "testtitle" };
                tempDataDictionary[SurveysController.TemporarySurveyKey] = JsonConvert.SerializeObject(survey);

                var mockTempDataDictionary = new Mock<ITempDataDictionary>();
                mockTempDataDictionary.Setup(d => d.ContainsKey(SurveysController.TemporarySurveyKey))
                                      .Returns(tempDataDictionary.ContainsKey(SurveysController.TemporarySurveyKey));
                mockTempDataDictionary.Setup(d => d[SurveysController.TemporarySurveyKey])
                                      .Returns((string key) => tempDataDictionary[SurveysController.TemporarySurveyKey]);
                mockTempDataDictionary.SetupSet(d => d[SurveysController.TemporarySurveyKey] = It.IsAny<string>())
                                      .Callback((string key, object value) => tempDataDictionary[key] = value);
                controller.TempData = mockTempDataDictionary.Object;
                var result = controller.New() as ViewResult;

                var model = result.ViewData.Model as PageViewData<SurveyModel>;

                Assert.AreEqual(survey.Title, model.ContentModel.Title);
            }
        }

        [TestMethod]
        public async Task NewWhenHttpVerbIsPostCallsSaveFromSurveyStoreWithSurveyParameterWhenModelStateIsValid()
        {
            var tempDataDictionary = new Dictionary<string, object>();

            var survey = new SurveyModel("slug-name");
            var cachedSurvey = new SurveyModel();
            cachedSurvey.Questions.Add(new Question());
            var surveyManagementService = new Mock<ISurveyManagementService>();
            using (var controller = new SurveysController(surveyManagementService.Object, new Mock<ISurveyAnswerService>().Object, new Mock<ISurveyAnalysisService>().Object))
            {
                tempDataDictionary[SurveysController.TemporarySurveyKey] = JsonConvert.SerializeObject(cachedSurvey);

                var mockTempDataDictionary = new Mock<ITempDataDictionary>();
                mockTempDataDictionary.Setup(d => d.ContainsKey(SurveysController.TemporarySurveyKey))
                                      .Returns(tempDataDictionary.ContainsKey(SurveysController.TemporarySurveyKey));
                mockTempDataDictionary.Setup(d => d[SurveysController.TemporarySurveyKey])
                                      .Returns((string key) => tempDataDictionary[SurveysController.TemporarySurveyKey]);
                mockTempDataDictionary.SetupSet(d => d[SurveysController.TemporarySurveyKey] = It.IsAny<string>())
                                      .Callback((string key, object value) => tempDataDictionary[key] = value);
                controller.TempData = mockTempDataDictionary.Object;

                await controller.New(survey);
            }

            surveyManagementService.Verify(r => r.PublishSurveyAsync(
                It.Is<Tailspin.SurveyManagementService.Client.Models.Survey>(s => s.SlugName == "slug-name")), Times.Once());
        }

        [TestMethod]
        public async Task NewWhenHttpVerbIsPostSavedTenantIdIsSameAsControllerWhenModelStateIsValid()
        {
            var tempDataDictionary = new Dictionary<string, object>();

            var surveyManagementService = new Mock<ISurveyManagementService>();

            var cachedSurvey = new SurveyModel();
            cachedSurvey.Questions.Add(new Question());

            using (var controller = new SurveysController(surveyManagementService.Object, new Mock<ISurveyAnswerService>().Object, new Mock<ISurveyAnalysisService>().Object))
            {
                tempDataDictionary[SurveysController.TemporarySurveyKey] = JsonConvert.SerializeObject(cachedSurvey);

                var mockTempDataDictionary = new Mock<ITempDataDictionary>();
                mockTempDataDictionary.Setup(d => d.ContainsKey(SurveysController.TemporarySurveyKey))
                                      .Returns(tempDataDictionary.ContainsKey(SurveysController.TemporarySurveyKey));
                mockTempDataDictionary.Setup(d => d[SurveysController.TemporarySurveyKey])
                                      .Returns((string key) => tempDataDictionary[SurveysController.TemporarySurveyKey]);
                mockTempDataDictionary.SetupSet(d => d[SurveysController.TemporarySurveyKey] = It.IsAny<string>())
                                      .Callback((string key, object value) => tempDataDictionary[key] = value);
                controller.TempData = mockTempDataDictionary.Object;

                await controller.New(new SurveyModel());
            }

            surveyManagementService.Verify(r => r.PublishSurveyAsync(
                It.Is<Tailspin.SurveyManagementService.Client.Models.Survey>(s => true)), Times.Once());
        }

        [TestMethod]
        public async Task NewWhenHttpVerbIsPostCopiesQuestionsFromCachedSurveyToSurveyWhenCallingSaveFromSurveyStoreWhenModelStateIsValid()
        {
            var tempDataDictionary = new Dictionary<string, object>();

            var surveyManagementService = new Mock<ISurveyManagementService>();

            var survey = new SurveyModel("slug-name");
            var questionsToBeCopied = new List<Question>();
            questionsToBeCopied.Add(new Question());
            var cachedSurvey = new SurveyModel { Questions = questionsToBeCopied };

            using (var controller = new SurveysController(surveyManagementService.Object, new Mock<ISurveyAnswerService>().Object, new Mock<ISurveyAnalysisService>().Object))
            {
                tempDataDictionary[SurveysController.TemporarySurveyKey] = JsonConvert.SerializeObject(cachedSurvey);

                var mockTempDataDictionary = new Mock<ITempDataDictionary>();
                mockTempDataDictionary.Setup(d => d.ContainsKey(SurveysController.TemporarySurveyKey))
                                      .Returns(tempDataDictionary.ContainsKey(SurveysController.TemporarySurveyKey));
                mockTempDataDictionary.Setup(d => d[SurveysController.TemporarySurveyKey])
                                      .Returns((string key) => tempDataDictionary[SurveysController.TemporarySurveyKey]);
                mockTempDataDictionary.SetupSet(d => d[SurveysController.TemporarySurveyKey] = It.IsAny<string>())
                                      .Callback((string key, object value) => tempDataDictionary[key] = value);
                controller.TempData = mockTempDataDictionary.Object;

                await controller.New(survey);
            }

            surveyManagementService.Verify(r => r.PublishSurveyAsync(
                It.Is<Tailspin.SurveyManagementService.Client.Models.Survey>(s => questionsToBeCopied.Count == s.Questions.Count)), Times.Once());
        }

        [TestMethod]
        public async Task NewWhenHttpVerbIsPostCleansCachedSurveyWhenModelStateIsValid()
        {
            var tempDataDictionary = new Dictionary<string, object>();

            bool isRemoveCalled = false;
            var cachedSurvey = new SurveyModel();
            cachedSurvey.Questions.Add(new Question());

            using (var controller = new SurveysController(new Mock<ISurveyManagementService>().Object, new Mock<ISurveyAnswerService>().Object, new Mock<ISurveyAnalysisService>().Object))
            {
                tempDataDictionary[SurveysController.TemporarySurveyKey] = JsonConvert.SerializeObject(cachedSurvey);

                var mockTempDataDictionary = new Mock<ITempDataDictionary>();
                mockTempDataDictionary.Setup(d => d.ContainsKey(SurveysController.TemporarySurveyKey))
                                      .Returns(tempDataDictionary.ContainsKey(SurveysController.TemporarySurveyKey));
                mockTempDataDictionary.Setup(d => d[SurveysController.TemporarySurveyKey])
                                      .Returns((string key) => tempDataDictionary[SurveysController.TemporarySurveyKey]);
                mockTempDataDictionary.SetupSet(d => d[SurveysController.TemporarySurveyKey] = It.IsAny<string>())
                                      .Callback((string key, object value) => tempDataDictionary[key] = value);
                mockTempDataDictionary.Setup(d => d.Remove(SurveysController.TemporarySurveyKey))
                                      .Callback(() => isRemoveCalled = true);
                controller.TempData = mockTempDataDictionary.Object;

                await controller.New(new SurveyModel());

                Assert.IsTrue(isRemoveCalled);
            }
        }

        [TestMethod]
        public async Task NewWhenHttpVerbIsPostReturnsRedirectToMySurveysWhenModelStateIsValid()
        {
            var tempDataDictionary = new Dictionary<string, object>();

            var cachedSurvey = new SurveyModel();
            cachedSurvey.Questions.Add(new Question());

            using (var controller = new SurveysController(new Mock<ISurveyManagementService>().Object, new Mock<ISurveyAnswerService>().Object, new Mock<ISurveyAnalysisService>().Object))
            {
                tempDataDictionary[SurveysController.TemporarySurveyKey] = JsonConvert.SerializeObject(cachedSurvey);

                var mockTempDataDictionary = new Mock<ITempDataDictionary>();
                mockTempDataDictionary.Setup(d => d.ContainsKey(SurveysController.TemporarySurveyKey))
                                      .Returns(tempDataDictionary.ContainsKey(SurveysController.TemporarySurveyKey));
                mockTempDataDictionary.Setup(d => d[SurveysController.TemporarySurveyKey])
                                      .Returns((string key) => tempDataDictionary[SurveysController.TemporarySurveyKey]);
                mockTempDataDictionary.SetupSet(d => d[SurveysController.TemporarySurveyKey] = It.IsAny<string>())
                                      .Callback((string key, object value) => tempDataDictionary[key] = value);
                controller.TempData = mockTempDataDictionary.Object;

                var result = await controller.New(new SurveyModel()) as RedirectToActionResult;

                Assert.AreEqual("Index", result.ActionName);
                Assert.AreEqual(null, result.ControllerName);
            }
        }

        [TestMethod]
        public async Task NewWhenHttpVerbIsPostReturnsRedirectToTheNewActionWhenCachedSurveyIsNull()
        {
            using (var controller = new SurveysController(new Mock<ISurveyManagementService>().Object, new Mock<ISurveyAnswerService>().Object, new Mock<ISurveyAnalysisService>().Object))
            {
                var mockTempDataDictionary = new Mock<ITempDataDictionary>();
                mockTempDataDictionary.SetupGet(d => d[SurveysController.TemporarySurveyKey]).Returns(null);
                controller.TempData = mockTempDataDictionary.Object;

                var result = await controller.New(new SurveyModel()) as RedirectToActionResult;

                Assert.AreEqual("New", result.ActionName);
                Assert.AreEqual(null, result.ControllerName);
            }
        }

        [TestMethod]
        public async Task NewWhenHttpVerbIsPostReturnsEmptyViewNameWhenModelStateIsNotValid()
        {
            var tempDataDictionary = new Dictionary<string, object>();

            using (var controller = new SurveysController(new Mock<ISurveyManagementService>().Object, new Mock<ISurveyAnswerService>().Object, new Mock<ISurveyAnalysisService>().Object))
            {
                tempDataDictionary[SurveysController.TemporarySurveyKey] = JsonConvert.SerializeObject(new SurveyModel());

                controller.ModelState.AddModelError("error for test", "invalid model state");
                var mockTempDataDictionary = new Mock<ITempDataDictionary>();
                mockTempDataDictionary.Setup(d => d.ContainsKey(SurveysController.TemporarySurveyKey))
                                      .Returns(tempDataDictionary.ContainsKey(SurveysController.TemporarySurveyKey));
                mockTempDataDictionary.Setup(d => d[SurveysController.TemporarySurveyKey])
                                      .Returns((string key) => tempDataDictionary[SurveysController.TemporarySurveyKey]);
                mockTempDataDictionary.SetupSet(d => d[SurveysController.TemporarySurveyKey] = It.IsAny<string>())
                                      .Callback((string key, object value) => tempDataDictionary[key] = value);
                controller.TempData = mockTempDataDictionary.Object;

                var result = await controller.New(new SurveyModel()) as ViewResult;

                Assert.IsNull(result.ViewName);
            }
        }

        [TestMethod]
        public async Task NewWhenHttpVerbIsPostReturnsTheSameModelWhenModelStateIsNotValid()
        {
            var tempDataDictionary = new Dictionary<string, object>();

            var survey = new SurveyModel();

            using (var controller = new SurveysController(new Mock<ISurveyManagementService>().Object, new Mock<ISurveyAnswerService>().Object, new Mock<ISurveyAnalysisService>().Object))
            {
                tempDataDictionary[SurveysController.TemporarySurveyKey] = JsonConvert.SerializeObject(survey);

                controller.ModelState.AddModelError("error for test", "invalid model state");
                var mockTempDataDictionary = new Mock<ITempDataDictionary>();
                mockTempDataDictionary.Setup(d => d.ContainsKey(SurveysController.TemporarySurveyKey))
                                      .Returns(tempDataDictionary.ContainsKey(SurveysController.TemporarySurveyKey));
                mockTempDataDictionary.Setup(d => d[SurveysController.TemporarySurveyKey])
                                      .Returns((string key) => tempDataDictionary[SurveysController.TemporarySurveyKey]);
                mockTempDataDictionary.SetupSet(d => d[SurveysController.TemporarySurveyKey] = It.IsAny<string>())
                                      .Callback((string key, object value) => tempDataDictionary[key] = value);
                controller.TempData = mockTempDataDictionary.Object;

                var result = await controller.New(survey) as ViewResult;

                var model = result.ViewData.Model as PageViewData<SurveyModel>;

                Assert.AreSame(survey, model.ContentModel);
            }
        }

        [TestMethod]
        public async Task NewWhenHttpVerbIsPostReturnsTitleInTheModelWhenModelStateIsNotValid()
        {
            var tempDataDictionary = new Dictionary<string, object>();

            var survey = new SurveyModel();

            using (var controller = new SurveysController(new Mock<ISurveyManagementService>().Object, new Mock<ISurveyAnswerService>().Object, new Mock<ISurveyAnalysisService>().Object))
            {
                tempDataDictionary[SurveysController.TemporarySurveyKey] = JsonConvert.SerializeObject(survey);

                controller.ModelState.AddModelError("error for test", "invalid model state");
                var mockTempDataDictionary = new Mock<ITempDataDictionary>();
                mockTempDataDictionary.Setup(d => d.ContainsKey(SurveysController.TemporarySurveyKey))
                                      .Returns(tempDataDictionary.ContainsKey(SurveysController.TemporarySurveyKey));
                mockTempDataDictionary.Setup(d => d[SurveysController.TemporarySurveyKey])
                                      .Returns((string key) => tempDataDictionary[SurveysController.TemporarySurveyKey]);
                mockTempDataDictionary.SetupSet(d => d[SurveysController.TemporarySurveyKey] = It.IsAny<string>())
                                      .Callback((string key, object value) => tempDataDictionary[key] = value);
                controller.TempData = mockTempDataDictionary.Object;

                var result = await controller.New(survey) as ViewResult;

                var model = result.ViewData.Model as MasterPageViewData;
                Assert.AreSame("New Survey", model.Title);
            }
        }

        [TestMethod]
        public async Task NewWhenHttpVerbIsPostSavesCachedSurveyInTempDataWhenModelStateIsNotValid()
        {
            var tempDataDictionary = new Dictionary<string, object>();

            var cachedSurvey = new SurveyModel() { Title = "testtitle" };

            using (var controller = new SurveysController(new Mock<ISurveyManagementService>().Object, new Mock<ISurveyAnswerService>().Object, new Mock<ISurveyAnalysisService>().Object))
            {
                tempDataDictionary[SurveysController.TemporarySurveyKey] = JsonConvert.SerializeObject(cachedSurvey);

                controller.ModelState.AddModelError("error for test", "invalid model state");
                var mockTempDataDictionary = new Mock<ITempDataDictionary>();
                mockTempDataDictionary.Setup(d => d.ContainsKey(SurveysController.TemporarySurveyKey))
                                      .Returns(tempDataDictionary.ContainsKey(SurveysController.TemporarySurveyKey));
                mockTempDataDictionary.Setup(d => d[SurveysController.TemporarySurveyKey])
                                      .Returns((string key) => tempDataDictionary[SurveysController.TemporarySurveyKey]);
                mockTempDataDictionary.SetupSet(d => d[SurveysController.TemporarySurveyKey] = It.IsAny<string>())
                                      .Callback((string key, object value) => tempDataDictionary[key] = value);
                controller.TempData = mockTempDataDictionary.Object;

                var result = await controller.New(new SurveyModel()) as ViewResult;

                var model = result.ViewData.Model as PageViewData<Survey>;
                Assert.AreEqual(cachedSurvey.Title, JsonConvert.DeserializeObject<SurveyModel>(controller.TempData[SurveysController.TemporarySurveyKey].ToString()).Title);
            }
        }

        [TestMethod]
        public async Task NewWhenHttpVerbIsPostCopiesQuestionsFromCachedSurveyToSurveyWhenModelStateIsNotValid()
        {
            var tempDataDictionary = new Dictionary<string, object>();

            var mockSurveyStore = new Mock<ISurveyManagementService>();

            var questionsToBeCopied = new List<Question>();
            questionsToBeCopied.Add(new Question());
            var cachedSurvey = new SurveyModel { Questions = questionsToBeCopied };

            using (var controller = new SurveysController(new Mock<ISurveyManagementService>().Object, new Mock<ISurveyAnswerService>().Object, new Mock<ISurveyAnalysisService>().Object))
            {
                tempDataDictionary[SurveysController.TemporarySurveyKey] = JsonConvert.SerializeObject(cachedSurvey);

                controller.ModelState.AddModelError("error for test", "invalid model state");
                var mockTempDataDictionary = new Mock<ITempDataDictionary>();
                mockTempDataDictionary.Setup(d => d.ContainsKey(SurveysController.TemporarySurveyKey))
                                      .Returns(tempDataDictionary.ContainsKey(SurveysController.TemporarySurveyKey));
                mockTempDataDictionary.Setup(d => d[SurveysController.TemporarySurveyKey])
                                      .Returns((string key) => tempDataDictionary[SurveysController.TemporarySurveyKey]);
                mockTempDataDictionary.SetupSet(d => d[SurveysController.TemporarySurveyKey] = It.IsAny<string>())
                                      .Callback((string key, object value) => tempDataDictionary[key] = value);
                controller.TempData = mockTempDataDictionary.Object;

                var result = await controller.New(new SurveyModel()) as ViewResult;

                var model = result.ViewData.Model as PageViewData<SurveyModel>;

                Assert.AreEqual(model.ContentModel.Questions.Count, questionsToBeCopied.Count);
            }
        }

        [TestMethod]
        public async Task NewWhenHttpVerbIsPostReturnsErrorInModelStateWhenCachedSurveyHasNoQuestions()
        {
            var tempDataDictionary = new Dictionary<string, object>();

            var cachedSurvey = new SurveyModel { Questions = new List<Question>() };

            using (var controller = new SurveysController(new Mock<ISurveyManagementService>().Object, new Mock<ISurveyAnswerService>().Object, new Mock<ISurveyAnalysisService>().Object))
            {
                tempDataDictionary[SurveysController.TemporarySurveyKey] = JsonConvert.SerializeObject(cachedSurvey);

                var mockTempDataDictionary = new Mock<ITempDataDictionary>();
                mockTempDataDictionary.Setup(d => d.ContainsKey(SurveysController.TemporarySurveyKey))
                                      .Returns(tempDataDictionary.ContainsKey(SurveysController.TemporarySurveyKey));
                mockTempDataDictionary.Setup(d => d[SurveysController.TemporarySurveyKey])
                                      .Returns((string key) => tempDataDictionary[SurveysController.TemporarySurveyKey]);
                mockTempDataDictionary.SetupSet(d => d[SurveysController.TemporarySurveyKey] = It.IsAny<string>())
                                      .Callback((string key, object value) => tempDataDictionary[key] = value);
                controller.TempData = mockTempDataDictionary.Object;

                var result = await controller.New(new SurveyModel()) as ViewResult;

                Assert.IsTrue(controller.ModelState.Keys.Contains("ContentModel.Questions"));
            }
        }

        [TestMethod]
        public async Task NewWhenHttpVerbIsPostReturnsTheSameModelWhenCachedSurveyHasNoQuestions()
        {
            var tempDataDictionary = new Dictionary<string, object>();

            var cachedSurvey = new SurveyModel { Questions = new List<Question>() };
            var survey = new SurveyModel();

            using (var controller = new SurveysController(new Mock<ISurveyManagementService>().Object, new Mock<ISurveyAnswerService>().Object, new Mock<ISurveyAnalysisService>().Object))
            {
                tempDataDictionary[SurveysController.TemporarySurveyKey] = JsonConvert.SerializeObject(cachedSurvey);

                var mockTempDataDictionary = new Mock<ITempDataDictionary>();
                mockTempDataDictionary.Setup(d => d.ContainsKey(SurveysController.TemporarySurveyKey))
                                      .Returns(tempDataDictionary.ContainsKey(SurveysController.TemporarySurveyKey));
                mockTempDataDictionary.Setup(d => d[SurveysController.TemporarySurveyKey])
                                      .Returns((string key) => tempDataDictionary[SurveysController.TemporarySurveyKey]);
                mockTempDataDictionary.SetupSet(d => d[SurveysController.TemporarySurveyKey] = It.IsAny<string>())
                                      .Callback((string key, object value) => tempDataDictionary[key] = value);
                controller.TempData = mockTempDataDictionary.Object;

                var result = await controller.New(survey) as ViewResult;
                var model = result.ViewData.Model as PageViewData<SurveyModel>;

                Assert.AreSame(survey, model.ContentModel);
            }
        }

        [TestMethod]
        public async Task NewWhenHttpVerbIsPostReturnsNewSurveyAsTitleInTheModelWhenCachedSurveyHasNoQuestions()
        {
            var tempDataDictionary = new Dictionary<string, object>();

            var cachedSurvey = new SurveyModel { Questions = new List<Question>() };

            using (var controller = new SurveysController(new Mock<ISurveyManagementService>().Object, new Mock<ISurveyAnswerService>().Object, new Mock<ISurveyAnalysisService>().Object))
            {
                tempDataDictionary[SurveysController.TemporarySurveyKey] = JsonConvert.SerializeObject(cachedSurvey);

                var mockTempDataDictionary = new Mock<ITempDataDictionary>();
                mockTempDataDictionary.Setup(d => d.ContainsKey(SurveysController.TemporarySurveyKey))
                                      .Returns(tempDataDictionary.ContainsKey(SurveysController.TemporarySurveyKey));
                mockTempDataDictionary.Setup(d => d[SurveysController.TemporarySurveyKey])
                                      .Returns((string key) => tempDataDictionary[SurveysController.TemporarySurveyKey]);
                mockTempDataDictionary.SetupSet(d => d[SurveysController.TemporarySurveyKey] = It.IsAny<string>())
                                      .Callback((string key, object value) => tempDataDictionary[key] = value);
                controller.TempData = mockTempDataDictionary.Object;

                var result = await controller.New(new SurveyModel()) as ViewResult;
                var model = result.ViewData.Model as MasterPageViewData;

                Assert.AreSame("New Survey", model.Title);
            }
        }

        [TestMethod]
        public async Task NewWhenHttpVerbIsPostSavesCachedSurveyInTempDataWhenCachedSurveyHasNoQuestions()
        {
            var tempDataDictionary = new Dictionary<string, object>();

            var cachedSurvey = new SurveyModel { Questions = new List<Question>(), Title="testtitle" };

            using (var controller = new SurveysController(new Mock<ISurveyManagementService>().Object, new Mock<ISurveyAnswerService>().Object, new Mock<ISurveyAnalysisService>().Object))
            {
                tempDataDictionary[SurveysController.TemporarySurveyKey] = JsonConvert.SerializeObject(cachedSurvey);

                var mockTempDataDictionary = new Mock<ITempDataDictionary>();
                mockTempDataDictionary.Setup(d => d.ContainsKey(SurveysController.TemporarySurveyKey))
                                      .Returns(tempDataDictionary.ContainsKey(SurveysController.TemporarySurveyKey));
                mockTempDataDictionary.Setup(d => d[SurveysController.TemporarySurveyKey])
                                      .Returns((string key) => tempDataDictionary[SurveysController.TemporarySurveyKey]);
                mockTempDataDictionary.SetupSet(d => d[SurveysController.TemporarySurveyKey] = It.IsAny<string>())
                                      .Callback((string key, object value) => tempDataDictionary[key] = value);
                controller.TempData = mockTempDataDictionary.Object;

                var result = await controller.New(new SurveyModel()) as ViewResult;

                var model = result.ViewData.Model as PageViewData<Survey>;
                Assert.AreEqual(cachedSurvey.Title, JsonConvert.DeserializeObject<SurveyModel>(controller.TempData[SurveysController.TemporarySurveyKey].ToString()).Title);
            }
        }

        [TestMethod]
        public void NewQuestionReturnsEmptyViewName()
        {
            var tempDataDictionary = new Dictionary<string, object>();

            using (var controller = new SurveysController(new Mock<ISurveyManagementService>().Object, new Mock<ISurveyAnswerService>().Object, new Mock<ISurveyAnalysisService>().Object))
            {
                tempDataDictionary[SurveysController.TemporarySurveyKey] = JsonConvert.SerializeObject(new SurveyModel());

                var mockTempDataDictionary = new Mock<ITempDataDictionary>();
                mockTempDataDictionary.Setup(d => d.ContainsKey(SurveysController.TemporarySurveyKey))
                                      .Returns(tempDataDictionary.ContainsKey(SurveysController.TemporarySurveyKey));
                mockTempDataDictionary.Setup(d => d[SurveysController.TemporarySurveyKey])
                                      .Returns((string key) => tempDataDictionary[SurveysController.TemporarySurveyKey]);
                mockTempDataDictionary.SetupSet(d => d[SurveysController.TemporarySurveyKey] = It.IsAny<string>())
                                      .Callback((string key, object value) => tempDataDictionary[key] = value);
                controller.TempData = mockTempDataDictionary.Object;

                var result = controller.NewQuestion(new SurveyModel()) as ViewResult;

                Assert.IsNull(result.ViewName);
            }
        }

        [TestMethod]
        public void NewQuestionReturnsNewQuestionAsTitleInTheModel()
        {
            var tempDataDictionary = new Dictionary<string, object>();

            using (var controller = new SurveysController(new Mock<ISurveyManagementService>().Object, new Mock<ISurveyAnswerService>().Object, new Mock<ISurveyAnalysisService>().Object))
            {
                tempDataDictionary[SurveysController.TemporarySurveyKey] = JsonConvert.SerializeObject(new SurveyModel());

                var mockTempDataDictionary = new Mock<ITempDataDictionary>();
                mockTempDataDictionary.Setup(d => d.ContainsKey(SurveysController.TemporarySurveyKey))
                                      .Returns(tempDataDictionary.ContainsKey(SurveysController.TemporarySurveyKey));
                mockTempDataDictionary.Setup(d => d[SurveysController.TemporarySurveyKey])
                                      .Returns((string key) => tempDataDictionary[SurveysController.TemporarySurveyKey]);
                mockTempDataDictionary.SetupSet(d => d[SurveysController.TemporarySurveyKey] = It.IsAny<string>())
                                      .Callback((string key, object value) => tempDataDictionary[key] = value);
                controller.TempData = mockTempDataDictionary.Object;

                var result = controller.NewQuestion(new SurveyModel()) as ViewResult;

                var model = result.ViewData.Model as MasterPageViewData;
                Assert.AreSame("New Question", model.Title);
            }
        }

        [TestMethod]
        public void NewQuestionReturnsNewQuestionInTheModel()
        {
            var tempDataDictionary = new Dictionary<string, object>();

            using (var controller = new SurveysController(new Mock<ISurveyManagementService>().Object, new Mock<ISurveyAnswerService>().Object, new Mock<ISurveyAnalysisService>().Object))
            {
                tempDataDictionary[SurveysController.TemporarySurveyKey] = JsonConvert.SerializeObject(new SurveyModel());

                var mockTempDataDictionary = new Mock<ITempDataDictionary>();
                mockTempDataDictionary.Setup(d => d.ContainsKey(SurveysController.TemporarySurveyKey))
                                      .Returns(tempDataDictionary.ContainsKey(SurveysController.TemporarySurveyKey));
                mockTempDataDictionary.Setup(d => d[SurveysController.TemporarySurveyKey])
                                      .Returns((string key) => tempDataDictionary[SurveysController.TemporarySurveyKey]);
                mockTempDataDictionary.SetupSet(d => d[SurveysController.TemporarySurveyKey] = It.IsAny<string>())
                                      .Callback((string key, object value) => tempDataDictionary[key] = value);
                controller.TempData = mockTempDataDictionary.Object;

                var result = controller.NewQuestion(new SurveyModel()) as ViewResult;

                var model = result.ViewData.Model as PageViewData<Question>;
                Assert.IsInstanceOfType(model.ContentModel, typeof(Question));
            }
        }

        [TestMethod]
        public void NewQuestionRedirectToTheNewActionWhenCachedSurveyIsNull()
        {
            using (var controller = new SurveysController(new Mock<ISurveyManagementService>().Object, new Mock<ISurveyAnswerService>().Object, new Mock<ISurveyAnalysisService>().Object))
            {
                var mockTempDataDictionary = new Mock<ITempDataDictionary>();
                mockTempDataDictionary.SetupGet(d => d[SurveysController.TemporarySurveyKey]).Returns(null);
                controller.TempData = mockTempDataDictionary.Object;

                var result = controller.NewQuestion(new SurveyModel()) as RedirectToActionResult;

                Assert.AreEqual("New", result.ActionName);
                Assert.AreEqual(null, result.ControllerName);
            }
        }

        [TestMethod]
        public void NewQuestionCopiesSurveyTitleToCachedSurveyThatIsReturnedInViewData()
        {
            var tempDataDictionary = new Dictionary<string, object>();

            var survey = new SurveyModel { Title = "title" };

            using (var controller = new SurveysController(new Mock<ISurveyManagementService>().Object, new Mock<ISurveyAnswerService>().Object, new Mock<ISurveyAnalysisService>().Object))
            {
                tempDataDictionary[SurveysController.TemporarySurveyKey] = JsonConvert.SerializeObject(survey);

                var mockTempDataDictionary = new Mock<ITempDataDictionary>();
                mockTempDataDictionary.Setup(d => d.ContainsKey(SurveysController.TemporarySurveyKey))
                                      .Returns(tempDataDictionary.ContainsKey(SurveysController.TemporarySurveyKey));
                mockTempDataDictionary.Setup(d => d[SurveysController.TemporarySurveyKey])
                                      .Returns((string key) => tempDataDictionary[SurveysController.TemporarySurveyKey]);
                mockTempDataDictionary.SetupSet(d => d[SurveysController.TemporarySurveyKey] = It.IsAny<string>())
                                      .Callback((string key, object value) => tempDataDictionary[key] = value);
                controller.TempData = mockTempDataDictionary.Object;

                var result = controller.NewQuestion(survey) as ViewResult;

                var inProgressSurvey = JsonConvert.DeserializeObject<SurveyModel>(result.TempData[SurveysController.TemporarySurveyKey].ToString());

                Assert.AreEqual(survey.Title, inProgressSurvey.Title);
            }
        }

        [TestMethod]
        public void AddQuestionReturnsRedirectToNewSurveyWhenModelIsValid()
        {
            var tempDataDictionary = new Dictionary<string, object>();

            using (var controller = new SurveysController(new Mock<ISurveyManagementService>().Object, new Mock<ISurveyAnswerService>().Object, new Mock<ISurveyAnalysisService>().Object))
            {
                tempDataDictionary[SurveysController.TemporarySurveyKey] = JsonConvert.SerializeObject(new SurveyModel());

                var mockTempDataDictionary = new Mock<ITempDataDictionary>();
                mockTempDataDictionary.Setup(d => d.ContainsKey(SurveysController.TemporarySurveyKey))
                                      .Returns(tempDataDictionary.ContainsKey(SurveysController.TemporarySurveyKey));
                mockTempDataDictionary.Setup(d => d[SurveysController.TemporarySurveyKey])
                                      .Returns((string key) => tempDataDictionary[SurveysController.TemporarySurveyKey]);
                mockTempDataDictionary.SetupSet(d => d[SurveysController.TemporarySurveyKey] = It.IsAny<string>())
                                      .Callback((string key, object value) => tempDataDictionary[key] = value);
                controller.TempData = mockTempDataDictionary.Object;

                var result = controller.AddQuestion(new Question()) as RedirectToActionResult;

                Assert.AreEqual("New", result.ActionName);
                Assert.AreEqual(null, result.ControllerName);
            }
        }

        [TestMethod]
        public void AddQuestionAddsTheNewQuestionToTheCachedSurveyWhenModelIsValid()
        {
            var tempDataDictionary = new Dictionary<string, object>();
            var inProgressSurvey = new SurveyModel();
            inProgressSurvey.Questions.Add(new Question());
            var question = new Question() { Text = "question2" };

            using (var controller = new SurveysController(new Mock<ISurveyManagementService>().Object, new Mock<ISurveyAnswerService>().Object, new Mock<ISurveyAnalysisService>().Object))
            {
                tempDataDictionary[SurveysController.TemporarySurveyKey] = JsonConvert.SerializeObject(inProgressSurvey);

                var mockTempDataDictionary = new Mock<ITempDataDictionary>();
                mockTempDataDictionary.Setup(d => d.ContainsKey(SurveysController.TemporarySurveyKey))
                                      .Returns(tempDataDictionary.ContainsKey(SurveysController.TemporarySurveyKey));
                mockTempDataDictionary.Setup(d => d[SurveysController.TemporarySurveyKey])
                                      .Returns((string key) => tempDataDictionary[SurveysController.TemporarySurveyKey]);
                mockTempDataDictionary.SetupSet(d => d[SurveysController.TemporarySurveyKey] = It.IsAny<string>())
                                      .Callback((string key, object value) => tempDataDictionary[key] = value);
                controller.TempData = mockTempDataDictionary.Object;

                controller.AddQuestion(question);

                var actualQuestions = (JsonConvert.DeserializeObject<SurveyModel>(controller.TempData[SurveysController.TemporarySurveyKey].ToString())).Questions;

                Assert.AreEqual(2, actualQuestions.Count);
                Assert.IsTrue(actualQuestions.Any(q => q.Text == "question2"));
            }
        }

        [TestMethod]
        public void AddQuestionReplacesCarriageReturnsInPossibleAnswersWhenModelIsValid()
        {
            var tempDataDictionary = new Dictionary<string, object>();

            using (var controller = new SurveysController(new Mock<ISurveyManagementService>().Object, new Mock<ISurveyAnswerService>().Object, new Mock<ISurveyAnalysisService>().Object))
            {
                tempDataDictionary[SurveysController.TemporarySurveyKey] = JsonConvert.SerializeObject(new SurveyModel());

                var question = new Question { PossibleAnswers = "possible answers\r\n" };

                var mockTempDataDictionary = new Mock<ITempDataDictionary>();
                mockTempDataDictionary.Setup(d => d.ContainsKey(SurveysController.TemporarySurveyKey))
                                     .Returns(tempDataDictionary.ContainsKey(SurveysController.TemporarySurveyKey));
                mockTempDataDictionary.Setup(d => d[SurveysController.TemporarySurveyKey])
                                      .Returns((string key) => tempDataDictionary[SurveysController.TemporarySurveyKey]);
                mockTempDataDictionary.SetupSet(d => d[SurveysController.TemporarySurveyKey] = It.IsAny<string>())
                                      .Callback((string key, object value) => tempDataDictionary[key] = value);
                controller.TempData = mockTempDataDictionary.Object;

                controller.AddQuestion(question);

                var cachedSurvey = JsonConvert.DeserializeObject<SurveyModel>(controller.TempData[SurveysController.TemporarySurveyKey].ToString());

                var actualQuestion = cachedSurvey.Questions.First();
                Assert.AreEqual("possible answers\n", actualQuestion.PossibleAnswers);
            }
        }

        [TestMethod]
        public void AddQuestionReturnsNewQuestionViewWhenModelIsNotValid()
        {
            using (var controller = new SurveysController(new Mock<ISurveyManagementService>().Object, new Mock<ISurveyAnswerService>().Object, new Mock<ISurveyAnalysisService>().Object))
            {
                var mockTempDataDictionary = new Mock<ITempDataDictionary>();
                mockTempDataDictionary.SetupGet(d => d[SurveysController.TemporarySurveyKey]).Returns(new SurveyModel());
                controller.TempData = mockTempDataDictionary.Object;

                controller.ModelState.AddModelError("error for test", "invalid model state");

                var result = controller.AddQuestion(null) as ViewResult;

                Assert.AreSame("NewQuestion", result.ViewName);
            }
        }

        [TestMethod]
        public void AddQuestionReturnsQuestionAsModelWhenModelIsNotValid()
        {
            using (var controller = new SurveysController(new Mock<ISurveyManagementService>().Object, new Mock<ISurveyAnswerService>().Object, new Mock<ISurveyAnalysisService>().Object))
            {
                var mockTempDataDictionary = new Mock<ITempDataDictionary>();
                mockTempDataDictionary.SetupGet(d => d[SurveysController.TemporarySurveyKey]).Returns(new SurveyModel());
                controller.TempData = mockTempDataDictionary.Object;

                controller.ModelState.AddModelError("error for test", "invalid model state");
                var question = new Question();

                var result = controller.AddQuestion(question) as ViewResult;

                var model = result.ViewData.Model as PageViewData<Question>;

                Assert.AreSame(question, model.ContentModel);
            }
        }

        [TestMethod]
        public void AddQuestionReturnsNewQuestionAsModelWhenModelIsNotValidAndQuestionIsNull()
        {
            using (var controller = new SurveysController(new Mock<ISurveyManagementService>().Object, new Mock<ISurveyAnswerService>().Object, new Mock<ISurveyAnalysisService>().Object))
            {
                var mockTempDataDictionary = new Mock<ITempDataDictionary>();
                mockTempDataDictionary.SetupGet(d => d[SurveysController.TemporarySurveyKey]).Returns(new SurveyModel());
                controller.TempData = mockTempDataDictionary.Object;

                controller.ModelState.AddModelError("error for test", "invalid model state");

                var result = controller.AddQuestion(null) as ViewResult;

                var model = result.ViewData.Model as PageViewData<Question>;

                Assert.IsInstanceOfType(model.ContentModel, typeof(Question));
            }
        }

        [TestMethod]
        public void AddQuestionCopiesCachedSurveyToTempDataWhenModelIsNotValid()
        {
            using (var controller = new SurveysController(new Mock<ISurveyManagementService>().Object, new Mock<ISurveyAnswerService>().Object, new Mock<ISurveyAnalysisService>().Object))
            {
                controller.ModelState.AddModelError("error for test", "invalid model state");
                var cachedSurvey = new SurveyModel();
                var mockTempDataDictionary = new Mock<ITempDataDictionary>();
                mockTempDataDictionary.SetupGet(d => d[SurveysController.TemporarySurveyKey]).Returns(cachedSurvey);
                controller.TempData = mockTempDataDictionary.Object;

                var result = controller.AddQuestion(null) as ViewResult;

                var cachedSurveyReturnedInTempData = result.TempData[SurveysController.TemporarySurveyKey] as SurveyModel;

                Assert.AreSame(cachedSurvey, cachedSurveyReturnedInTempData);
            }
        }

        [TestMethod]
        public void AddQuestionReturnsNewQuestionAsTitleInTheModelWhenModelIsNotValid()
        {
            using (var controller = new SurveysController(new Mock<ISurveyManagementService>().Object, new Mock<ISurveyAnswerService>().Object, new Mock<ISurveyAnalysisService>().Object))
            {
                var mockTempDataDictionary = new Mock<ITempDataDictionary>();
                mockTempDataDictionary.SetupGet(d => d[SurveysController.TemporarySurveyKey]).Returns(new SurveyModel());
                controller.TempData = mockTempDataDictionary.Object;

                controller.ModelState.AddModelError("error for test", "invalid model state");

                var result = controller.AddQuestion(null) as ViewResult;

                var model = result.ViewData.Model as MasterPageViewData;
                Assert.AreSame("New Question", model.Title);
            }
        }

        [TestMethod]
        public async Task BrowseResponsesReturnSlugNameAsTheTitle()
        {
            var mockSurveyAnswerService = new Mock<ISurveyAnswerService>();
            mockSurveyAnswerService.Setup(r => r.GetSurveyAnswerBrowsingContextAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new SurveyAnswerService.Client.Models.SurveyAnswerBrowsingContext { PreviousAnswerId = string.Empty, NextAnswerId = string.Empty });

            using (var controller = new SurveysController(new Mock<ISurveyManagementService>().Object, mockSurveyAnswerService.Object, new Mock<ISurveyAnalysisService>().Object))
            {
                var result = await controller.BrowseResponses("slug-name", string.Empty) as ViewResult;

                var model = result.ViewData.Model as MasterPageViewData;
                Assert.AreSame("slug-name", model.Title);
            }
        }

        //[TestMethod]
        //public async Task BrowseResponsesGetsTheAnswerFromTheStoreWhenAnswerIdIsNotEmpty()
        //{
        //    var mockSurveyAnswerStore = new Mock<ISurveyAnswerStore>();
        //    mockSurveyAnswerStore.Setup(r => r.GetSurveyAnswerBrowsingContextAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
        //                              .ReturnsAsync(new SurveyAnswerBrowsingContext { PreviousId = string.Empty, NextId = string.Empty });

        //    var mockTenantStore = new Mock<ITenantStore>();
        //    mockTenantStore.Setup(r => r.GetTenantAsync(It.IsAny<string>()))
        //        .ReturnsAsync(new Tenant());

        //    using (var controller = new SurveysController(null, mockSurveyAnswerStore.Object, null, mockTenantStore.Object, null))
        //    {
        //        await controller.BrowseResponses("survey-slug", "answer id");
        //    }

        //    mockSurveyAnswerStore.Verify(r => r.GetSurveyAnswerAsync("adatum", "survey-slug", "answer id"));
        //}

        //[TestMethod]
        //public async Task BrowseResponsesGetsTheFirstAnswerIdFromTheStoreWhenAnswerIdIsEmpty()
        //{
        //    var mockSurveyAnswerStore = new Mock<ISurveyAnswerStore>();
        //    mockSurveyAnswerStore.Setup(r => r.GetSurveyAnswerBrowsingContextAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
        //                              .ReturnsAsync(new SurveyAnswerBrowsingContext { PreviousId = string.Empty, NextId = string.Empty });

        //    var mockTenantStore = new Mock<ITenantStore>();
        //    mockTenantStore.Setup(r => r.GetTenantAsync(It.IsAny<string>()))
        //        .ReturnsAsync(new Tenant());

        //    using (var controller = new SurveysController(null, mockSurveyAnswerStore.Object, null, mockTenantStore.Object, null))
        //    {
        //        await controller.BrowseResponses("survey-slug", string.Empty);
        //    }

        //    mockSurveyAnswerStore.Verify(r => r.GetFirstSurveyAnswerIdAsync("adatum", "survey-slug"), Times.Once());
        //}

        //[TestMethod]
        //public async Task BrowseResponsesGetsSurveyAnswerWithTheIdReturnedFromTheStoreWhenAnswerIdIsEmpty()
        //{
        //    var mockSurveyAnswerStore = new Mock<ISurveyAnswerStore>();
        //    mockSurveyAnswerStore.Setup(r => r.GetSurveyAnswerBrowsingContextAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
        //                              .ReturnsAsync(new SurveyAnswerBrowsingContext { PreviousId = string.Empty, NextId = string.Empty });
        //    mockSurveyAnswerStore.Setup(r => r.GetFirstSurveyAnswerIdAsync("adatum", "survey-slug"))
        //                              .ReturnsAsync("id");

        //    var mockTenantStore = new Mock<ITenantStore>();
        //    mockTenantStore.Setup(r => r.GetTenantAsync(It.IsAny<string>()))
        //        .ReturnsAsync(new Tenant());

        //    using (var controller = new SurveysController(null, mockSurveyAnswerStore.Object, null, mockTenantStore.Object, null))
        //    {
        //        await controller.BrowseResponses("survey-slug", string.Empty);
        //    }

        //    mockSurveyAnswerStore.Verify(r => r.GetSurveyAnswerAsync("adatum", "survey-slug", "id"));
        //}

        //[TestMethod]
        //public async Task BrowseResponsesSetsTheAnswerFromTheStoreInTheModel()
        //{
        //    var mockSurveyAnswerStore = new Mock<ISurveyAnswerStore>();
        //    var surveyAnswer = new SurveyAnswer();
        //    mockSurveyAnswerStore.Setup(r => r.GetSurveyAnswerBrowsingContextAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
        //                              .ReturnsAsync(new SurveyAnswerBrowsingContext { PreviousId = string.Empty, NextId = string.Empty });
        //    mockSurveyAnswerStore.Setup(r => r.GetSurveyAnswerAsync("adatum", "survey-slug", "answer id"))
        //                              .ReturnsAsync(surveyAnswer);

        //    var mockTenantStore = new Mock<ITenantStore>();
        //    mockTenantStore.Setup(r => r.GetTenantAsync(It.IsAny<string>()))
        //        .ReturnsAsync(new Tenant());

        //    using (var controller = new SurveysController(null, mockSurveyAnswerStore.Object, null, mockTenantStore.Object, null))
        //    {
        //        var result = await controller.BrowseResponses("survey-slug", "answer id") as ViewResult;

        //        var model = result.ViewData.Model as PageViewData<BrowseResponseModel>;
        //        Assert.AreSame(surveyAnswer, model.ContentModel.SurveyAnswer);
        //    }
        //}

        //[TestMethod]
        //public async Task BrowseResponsesCallsGetSurveyAnswerBrowsingContextFromStore()
        //{
        //    var mockSurveyAnswerStore = new Mock<ISurveyAnswerStore>();
        //    mockSurveyAnswerStore.Setup(r => r.GetSurveyAnswerBrowsingContextAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
        //                              .ReturnsAsync(new SurveyAnswerBrowsingContext { PreviousId = string.Empty, NextId = string.Empty });

        //    var mockTenantStore = new Mock<ITenantStore>();
        //    mockTenantStore.Setup(r => r.GetTenantAsync(It.IsAny<string>()))
        //        .ReturnsAsync(new Tenant());

        //    using (var controller = new SurveysController(null, mockSurveyAnswerStore.Object, null, mockTenantStore.Object, null))
        //    {
        //        await controller.BrowseResponses("survey-slug", "answer id");
        //    }

        //    mockSurveyAnswerStore.Verify(
        //        r => r.GetSurveyAnswerBrowsingContextAsync("adatum", "survey-slug", "answer id"),
        //        Times.Once());
        //}

        //[TestMethod]
        //public async Task BrowseResponsesSetsPreviousAndNextIdsInModel()
        //{
        //    var mockSurveyAnswerStore = new Mock<ISurveyAnswerStore>();
        //    mockSurveyAnswerStore.Setup(r => r.GetSurveyAnswerBrowsingContextAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
        //                              .ReturnsAsync(new SurveyAnswerBrowsingContext { PreviousId = "PreviousId", NextId = "NextId" });

        //    var mockTenantStore = new Mock<ITenantStore>();
        //    mockTenantStore.Setup(r => r.GetTenantAsync(It.IsAny<string>()))
        //        .ReturnsAsync(new Tenant());

        //    using (var controller = new SurveysController(null, mockSurveyAnswerStore.Object, null, mockTenantStore.Object, null))
        //    {
        //        var result = await controller.BrowseResponses("survey-slug", "answer id") as ViewResult;

        //        var model = result.ViewData.Model as PageViewData<BrowseResponseModel>;
        //        Assert.AreEqual("PreviousId", model.ContentModel.PreviousAnswerId);
        //        Assert.AreEqual("NextId", model.ContentModel.NextAnswerId);
        //    }
        //}

        //[TestMethod]
        //public async Task AnalyzeReturnSlugNameAsTheTitle()
        //{
        //    using (var controller = new SurveysController(new Mock<ISurveyManagementService>().Object, new Mock<ISurveyAnswerService>().Object, new Mock<ISurveyAnalysisService>().Object))
        //    {
        //        var result = await controller.Analyze("slug-name") as ViewResult;

        //        var model = result.ViewData.Model as MasterPageViewData;
        //        Assert.AreSame("slug-name", model.Title);
        //    }
        //}

        //[TestMethod]
        //public async Task AnalyzeGetsTheSummaryFromTheStore()
        //{
        //    var mockSurveyAnswersSummaryStore = new Mock<ISurveyAnswersSummaryStore>();

        //    var mockTenantStore = new Mock<ITenantStore>();
        //    mockTenantStore.Setup(r => r.GetTenantAsync(It.IsAny<string>()))
        //        .ReturnsAsync(new Tenant());

        //    using (var controller = new SurveysController(null, null, mockSurveyAnswersSummaryStore.Object, mockTenantStore.Object, null))
        //    {
        //        await controller.Analyze("slug-name");
        //    }

        //    mockSurveyAnswersSummaryStore.Verify(r => r.GetSurveyAnswersSummaryAsync("adatum", "slug-name"), Times.Once());
        //}

        //[TestMethod]
        //public async Task AnalyzeReturnsTheSummaryGetFromTheStoreInTheModel()
        //{
        //    var mockSurveyAnswersSummaryStore = new Mock<ISurveyAnswersSummaryStore>();
        //    var surveyAnswersSummary = new SurveyAnswersSummary();
        //    mockSurveyAnswersSummaryStore.Setup(r => r.GetSurveyAnswersSummaryAsync(It.IsAny<string>(), It.IsAny<string>()))
        //                                      .ReturnsAsync(surveyAnswersSummary);

        //    var mockTenantStore = new Mock<ITenantStore>();
        //    mockTenantStore.Setup(r => r.GetTenantAsync(It.IsAny<string>()))
        //        .ReturnsAsync(new Tenant());

        //    using (var controller = new SurveysController(null, null, mockSurveyAnswersSummaryStore.Object, mockTenantStore.Object, null))
        //    {
        //        var result = await controller.Analyze(string.Empty) as ViewResult;

        //        var model = result.ViewData.Model as PageViewData<SurveyAnswersSummary>;
        //        Assert.AreSame(surveyAnswersSummary, model.ContentModel);
        //    }
        //}

        //[TestMethod]
        //public async Task ExportDeterminesIfThereAreResponsesToExport()
        //{
        //    var mockTenantStore = new Mock<ITenantStore>();
        //    var mockSurveyAnswerStore = new Mock<ISurveyAnswerStore>();
        //    var tenant = new Tenant();
        //    mockTenantStore.Setup(r => r.GetTenantAsync(It.IsAny<string>())).ReturnsAsync(tenant);
        //    mockSurveyAnswerStore.Setup(r => r.GetFirstSurveyAnswerIdAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(string.Empty);

        //    using (var controller = new SurveysController(null, mockSurveyAnswerStore.Object, null, mockTenantStore.Object, null))
        //    {
        //        var result = await controller.ExportResponses(string.Empty) as ViewResult;

        //        var model = result.ViewData.Model as PageViewData<ExportResponseModel>;
        //        Assert.AreEqual(false, model.ContentModel.HasResponses);
        //    }
        //}

        //[TestMethod]
        //public async Task ExportPostCallsTransferPostWithTenantAndSlugName()
        //{
        //    var mockSurveyTransferStore = new Mock<ISurveyTransferStore>();

        //    using (var controller = new SurveysController(null, null, null, null, mockSurveyTransferStore.Object))
        //    {
        //        await controller.PostExportResponses("slugName");
        //    }

        //    mockSurveyTransferStore.Verify(r => r.TransferAsync("adatum", "slugName"), Times.Once());
        //}

        //[TestMethod]
        //public async Task ExportPostRedirectsToBrowseAction()
        //{
        //    var mockSurveyTransferStore = new Mock<ISurveyTransferStore>();

        //    using (var controller = new SurveysController(null, null, null, null, mockSurveyTransferStore.Object))
        //    {
        //        var result = await controller.PostExportResponses("slugName") as RedirectToActionResult;

        //        Assert.AreEqual(result.ActionName, "BrowseResponses");
        //    }
        //}

    }
}