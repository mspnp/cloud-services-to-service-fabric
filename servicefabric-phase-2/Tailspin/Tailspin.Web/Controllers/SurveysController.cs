namespace Tailspin.Web.Controllers
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using Tailspin.SurveyAnswerService.Client;
    using Tailspin.SurveyManagementService.Client;
    using Tailspin.SurveyAnalysisService.Client;
    using Tailspin.Web.Models;
    using Tailspin.Web.Shared.Helpers;
    using Tailspin.Web.Shared.Models;

    //[RequireHttps]
    public class SurveysController : Controller
    {
        public const string TemporarySurveyKey = "temporarySurveyKey";

        private readonly ISurveyManagementService surveyManagementService;
        private readonly ISurveyAnswerService surveyAnswerService;
        private readonly ISurveyAnalysisService surveyAnalysisService;

        public SurveysController(ISurveyManagementService surveyManagementService, ISurveyAnswerService surveyAnswerService, ISurveyAnalysisService surveyAnalysisService)
        {
            if (surveyManagementService == null)
            {
                throw new ArgumentNullException(nameof(surveyManagementService));
            }

            if (surveyAnswerService == null)
            {
                throw new ArgumentNullException(nameof(surveyAnswerService));
            }

            if (surveyAnalysisService == null)
            {
                throw new ArgumentNullException(nameof(surveyAnalysisService));
            }

            this.surveyManagementService = surveyManagementService;
            this.surveyAnswerService = surveyAnswerService;
            this.surveyAnalysisService = surveyAnalysisService;
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            ClearTemporarySurvey();

            var surveyInformations = await this.surveyManagementService.ListSurveysAsync();

            var model = this.CreatePageViewData(surveyInformations.Select(si => si.ToSurveyModel()));
            model.Title = "My Surveys";

            return this.View(model);
        }

        [HttpGet]
        public ActionResult New()
        {
            var temporarySurveyModel = GetTemporarySurveyModel();

            if (temporarySurveyModel == null)
            {
                temporarySurveyModel = new SurveyModel();  // First time to the page
            }

            var model = this.CreatePageViewData(temporarySurveyModel);
            model.Title = "New Survey";

            SaveTemporarySurveyModel(temporarySurveyModel);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> New(SurveyModel contentModel)
        {
            var temporarySurveyModel = GetTemporarySurveyModel();

            if (temporarySurveyModel == null)
            {
                return this.RedirectToAction("New");
            }

            if (temporarySurveyModel.Questions == null || temporarySurveyModel.Questions.Count <= 0)
            {
                this.ModelState.AddModelError("ContentModel.Questions", string.Format(CultureInfo.InvariantCulture, "Please add at least one question to the survey."));
            }

            contentModel.Questions = temporarySurveyModel.Questions;
            if (!this.ModelState.IsValid)
            {
                var model = this.CreatePageViewData(contentModel);
                model.Title = "New Survey";
                SaveTemporarySurveyModel(temporarySurveyModel);
                return this.View(model);
            }

            try
            {
                await this.surveyManagementService.PublishSurveyAsync(contentModel.ToSurvey());
            }
            catch (Exception ex)
            {
                TraceHelper.TraceError(ex.TraceInformation());
                throw;
            }

            ClearTemporarySurvey();
            return this.RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewQuestion(SurveyModel contentModel)
        {
            var temporarySurveyModel = GetTemporarySurveyModel();

            if (temporarySurveyModel == null)
            {
                return this.RedirectToAction("New");
            }

            temporarySurveyModel.Title = contentModel.Title;

            SaveTemporarySurveyModel(temporarySurveyModel);

            var model = this.CreatePageViewData(new Question());
            model.Title = "New Question";

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddQuestion(Question contentModel)
        {
            var temporarySurveyModel = GetTemporarySurveyModel();

            if (!this.ModelState.IsValid)
            {
                SaveTemporarySurveyModel(temporarySurveyModel);
                var model = this.CreatePageViewData(contentModel ?? new Question());
                model.Title = "New Question";
                return this.View("NewQuestion", model);
            }

            if (contentModel.PossibleAnswers != null)
            {
                contentModel.PossibleAnswers = contentModel.PossibleAnswers.Replace("\r\n", "\n");
            }

            temporarySurveyModel.Questions.Add(contentModel);
            SaveTemporarySurveyModel(temporarySurveyModel);
            return this.RedirectToAction("New");
        }

        [HttpGet]
        public async Task<ActionResult> Analyze(string surveySlug)
        {
            var surveyAnswersSummary = await this.surveyAnalysisService.GetSurveyAnswersSummaryAsync(surveySlug);
            if (surveyAnswersSummary == null) surveyAnswersSummary = new SurveyAnalysisService.Client.Models.SurveyAnswersSummary();
            var model = this.CreatePageViewData(surveyAnswersSummary.ToSurveyAnswersSummary());
            model.Title = surveySlug;
            return this.View(model);
        }

        [HttpGet]
        public async Task<ActionResult> BrowseResponses(string surveySlug, string answerId)
        {
            var browsingContext = await this.surveyAnswerService.GetSurveyAnswerBrowsingContextAsync(surveySlug, answerId);
            var browseResponsesModel = browsingContext.ToBrowseResponseModel();
            var model = this.CreatePageViewData(browseResponsesModel);
            model.Title = surveySlug;
            return this.View(model);
        }

        private PageViewData<T> CreatePageViewData<T>(T contentModel)
        {
            var pageViewData = new PageViewData<T>(contentModel);
            return pageViewData;
        }

        private void ClearTemporarySurvey()
        {
            this.TempData.Remove(TemporarySurveyKey);
        }

        private SurveyModel GetTemporarySurveyModel()
        {
            if (this.TempData.ContainsKey(TemporarySurveyKey) && this.TempData[TemporarySurveyKey] != null)
            {
                return JsonConvert.DeserializeObject<SurveyModel>(this.TempData[TemporarySurveyKey].ToString());
            }
            return null;
        }

        private void SaveTemporarySurveyModel(SurveyModel temporarySurveyModel)
        {
            this.TempData[TemporarySurveyKey] = JsonConvert.SerializeObject(temporarySurveyModel);
        }
    }
}