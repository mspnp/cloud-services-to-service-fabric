namespace Tailspin.Web.Controllers
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.WindowsAzure.Storage;
    using Newtonsoft.Json;
    using Tailspin.Web.Models;
    using Tailspin.Web.Survey.Shared.Helpers;
    using Tailspin.Web.Survey.Shared.Models;
    using Tailspin.Web.Survey.Shared.Stores;

    //[RequireHttps]
    //[AuthenticateAndAuthorizeTenant]
    public class SurveysController : TenantController
    {
        public const string TemporarySurveyKey = "temporarySurveyKey";

        private readonly ISurveyStore surveyStore;
        private readonly ISurveyAnswerStore surveyAnswerStore;
        private readonly ISurveyAnswersSummaryStore surveyAnswersSummaryStore;
        private readonly ISurveyTransferStore surveyTransferStore;

        public SurveysController(
            ISurveyStore surveyStore,
            ISurveyAnswerStore surveyAnswerStore,
            ISurveyAnswersSummaryStore surveyAnswersSummaryStore,
            ITenantStore tenantStore,
            ISurveyTransferStore surveyTransferStore)
            : base(tenantStore)
        {
            this.surveyStore = surveyStore;
            this.surveyAnswerStore = surveyAnswerStore;
            this.surveyAnswersSummaryStore = surveyAnswersSummaryStore;
            this.surveyTransferStore = surveyTransferStore;
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            ClearTemporarySurvey();

            IEnumerable<SurveyModel> surveysForTenant = (await this.surveyStore
                .GetSurveysByTenantAsync(this.TenantId))
                .Select(s => new SurveyModel(s))
                .ToList();

            var model = await this.CreateTenantPageViewDataAsync(surveysForTenant);
            model.Title = "My Surveys";

            return this.View(model);
        }

        [HttpGet]
        public async Task<ActionResult> New()
        {
            var temporarySurveyModel = GetTemporarySurveyModel();

            if (temporarySurveyModel == null)
            {
                temporarySurveyModel = new SurveyModel();  // First time to the page
            }

            var model = await this.CreateTenantPageViewDataAsync(temporarySurveyModel);
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
                var model = await this.CreateTenantPageViewDataAsync(contentModel);
                model.Title = "New Survey";
                SaveTemporarySurveyModel(temporarySurveyModel);
                return this.View(model);
            }

            contentModel.TenantId = this.TenantId;
            try
            {
                await this.surveyStore.SaveSurveyAsync(contentModel.ToSurvey());
            }
            catch (StorageException ex)
            {
                TraceHelper.TraceError(ex.TraceInformation());
                throw;
            }

            ClearTemporarySurvey();
            return this.RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> NewQuestion(SurveyModel contentModel)
        {
            var temporarySurveyModel = GetTemporarySurveyModel();

            if (temporarySurveyModel == null)
            {
                return this.RedirectToAction("New");
            }

            temporarySurveyModel.Title = contentModel.Title;

            SaveTemporarySurveyModel(temporarySurveyModel);

            var model = await this.CreateTenantPageViewDataAsync(new Question());
            model.Title = "New Question";

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddQuestion(Question contentModel)
        {
            var temporarySurveyModel = GetTemporarySurveyModel();

            if (!this.ModelState.IsValid)
            {
                SaveTemporarySurveyModel(temporarySurveyModel);
                var model = await this.CreateTenantPageViewDataAsync(contentModel ?? new Question());
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string tenantId, string surveySlug)
        {
            await this.surveyStore.DeleteSurveyByTenantAndSlugNameAsync(tenantId, surveySlug);
            await this.surveyAnswerStore.DeleteSurveyAnswersAsync(tenantId, surveySlug);
            await this.surveyAnswersSummaryStore.DeleteSurveyAnswersSummaryAsync(tenantId, surveySlug);

            return this.RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Analyze(string tenantId, string surveySlug)
        {
            var surveyAnswersSummary = await this.surveyAnswersSummaryStore.GetSurveyAnswersSummaryAsync(tenantId, surveySlug);

            var model = await this.CreateTenantPageViewDataAsync(surveyAnswersSummary);
            model.Title = surveySlug;
            return this.View(model);
        }

        [HttpGet]
        public async Task<ActionResult> BrowseResponses(string tenantId, string surveySlug, string answerId)
        {
            SurveyAnswer surveyAnswer = null;
            if (string.IsNullOrEmpty(answerId))
            {
                answerId = await this.surveyAnswerStore.GetFirstSurveyAnswerIdAsync(tenantId, surveySlug);
            }

            if (!string.IsNullOrEmpty(answerId))
            {
                surveyAnswer = await this.surveyAnswerStore.GetSurveyAnswerAsync(tenantId, surveySlug, answerId);
            }

            var surveyAnswerBrowsingContext = await this.surveyAnswerStore.GetSurveyAnswerBrowsingContextAsync(tenantId, surveySlug, answerId);

            var browseResponsesModel = new BrowseResponseModel
                                           {
                                               SurveyAnswer = surveyAnswer,
                                               PreviousAnswerId = surveyAnswerBrowsingContext.PreviousId,
                                               NextAnswerId = surveyAnswerBrowsingContext.NextId
                                           };

            var model = await this.CreateTenantPageViewDataAsync(browseResponsesModel);
            model.Title = surveySlug;
            return this.View(model);
        }

        [HttpGet]
        public async Task<ActionResult> ExportResponses(string surveySlug)
        {

            var exportResponseModel = new ExportResponseModel { Tenant = await GetTenantAsync() };
            string answerId = await this.surveyAnswerStore.GetFirstSurveyAnswerIdAsync(this.TenantId, surveySlug);
            exportResponseModel.HasResponses = !string.IsNullOrEmpty(answerId);

            var model = await this.CreateTenantPageViewDataAsync(exportResponseModel);
            model.Title = surveySlug;
            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExportResponses(string tenantId, string surveySlug)
        {
            await this.surveyTransferStore.TransferAsync(tenantId, surveySlug);
            return this.RedirectToAction("BrowseResponses");
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