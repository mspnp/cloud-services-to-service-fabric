namespace Tailspin.Web.Areas.Survey.Controllers
{
    using System.Collections.Generic;
    using System.Data.Services.Client;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Web.Hosting;
    using System.Web.Mvc;
    using Models;
    using Tailspin.Web.Controllers;
    using Tailspin.Web.Survey.Shared.DataExtensibility;
    using Tailspin.Web.Survey.Shared.Helpers;
    using Tailspin.Web.Survey.Shared.Models;
    using Tailspin.Web.Survey.Shared.Stores;
    using System.Threading.Tasks;
    using Security;

    [RequireHttps]
    [AuthenticateAndAuthorizeTenant]
    public class SurveysController : TenantController
    {
        public const string CachedSurvey = "cachedSurvey";
        private const string ExtensionsPath = "ExtensionsPath";

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
            this.TempData[CachedSurvey] = null;

            IEnumerable<SurveyModel> surveysForTenant = (await this.surveyStore
                .GetSurveysByTenantAsync(this.TenantId))
                .Select(s => new SurveyModel(s))
                .ToList();

            if (surveysForTenant.Any())
            {
                string assemblyFile, typeNamespace;
                if (this.HasModelExtensionAssembly(await GetTenantAsync(), out assemblyFile, out typeNamespace))
                {
                    var extensionType = ExtensibilityTypeResolver.GetTypeFrom(assemblyFile, typeNamespace, typeof(Survey));
                    var modelExtensions = await this.surveyStore.GetSurveyExtensionsByTenantAsync(this.TenantId, extensionType);
                    foreach (var modelExtension in modelExtensions)
                    {
                        var survey = surveysForTenant.FirstOrDefault(s => modelExtension.IsChildOf(this.surveyStore.GetStorageKeyFor(s)));
                        if (survey != null)
                        {
                            survey.Extensions = ExtensibilityTypeMapper.GetModelExtensionProperties(modelExtension).ToList();
                        }
                    }
                }
            }

            var model = await this.CreateTenantPageViewDataAsync(surveysForTenant);
            model.Title = "My Surveys";

            return this.View(model);
        }

        [HttpGet]
        public async Task<ActionResult> New()
        {
            var cachedSurvey = (SurveyModel)this.TempData[CachedSurvey];

            if (cachedSurvey == null)
            {
                cachedSurvey = new SurveyModel();  // First time to the page
                string assemblyFile, typeNamespace;
                if (this.HasModelExtensionAssembly(await GetTenantAsync(), out assemblyFile, out typeNamespace))
                {
                    var modelExtension = ExtensibilityTypeResolver.GetInstanceFrom(assemblyFile, typeNamespace, typeof(Survey));
                    cachedSurvey.Extensions = ExtensibilityTypeMapper.GetModelExtensionProperties(modelExtension).ToList();
                }
            }

            var model = await this.CreateTenantPageViewDataAsync(cachedSurvey);
            model.Title = "New Survey";

            this.TempData[CachedSurvey] = cachedSurvey;

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> New(SurveyModel contentModel)
        {
            var cachedSurvey = (SurveyModel)this.TempData[CachedSurvey];

            if (cachedSurvey == null)
            {
                return this.RedirectToAction("New");
            }

            if (cachedSurvey.Questions == null || cachedSurvey.Questions.Count <= 0)
            {
                this.ModelState.AddModelError("ContentModel.Questions", string.Format(CultureInfo.InvariantCulture, "Please add at least one question to the survey."));
            }

            if ((cachedSurvey.Extensions != null) &&
                (cachedSurvey.Extensions.Count != contentModel.Extensions.Count))
            {
                this.ModelState.AddModelError("ContentModel.Extensions", string.Format(CultureInfo.InvariantCulture, "The number of custom properties don't match with the custom model."));
            }

            if (cachedSurvey.Extensions != null)
            {
                for (int i = 0; i < cachedSurvey.Extensions.Count; i++)
                {
                    contentModel.Extensions[i].PropertyName = cachedSurvey.Extensions[i].PropertyName;
                    if (string.IsNullOrWhiteSpace(contentModel.Extensions[i].PropertyValue))
                    {
                        this.ModelState.AddModelError("ContentModel.Extensions", string.Format(CultureInfo.InvariantCulture, "You need to provide a value for extended property '{0}'.", contentModel.Extensions[i].PropertyName.SplitWords()));
                    }
                }
            }

            contentModel.Questions = cachedSurvey.Questions;
            if (!this.ModelState.IsValid)
            {
                var model = await this.CreateTenantPageViewDataAsync(contentModel);
                model.Title = "New Survey";
                this.TempData[CachedSurvey] = cachedSurvey;
                return this.View(model);
            }

            contentModel.TenantId = this.TenantId;
            try
            {
                string assemblyFile, typeNamespace;
                if (this.HasModelExtensionAssembly(await GetTenantAsync(), out assemblyFile, out typeNamespace))
                {
                    var modelExtension = ExtensibilityTypeResolver.GetInstanceFrom(assemblyFile, typeNamespace, typeof(Survey));
                    ExtensibilityTypeMapper.SetModelExtensionProperties(modelExtension, contentModel.Extensions);
                    await this.surveyStore.SaveSurveyExtensionAsync(contentModel, modelExtension);
                }

                await this.surveyStore.SaveSurveyAsync(contentModel.ToSurvey());
            }
            catch (DataServiceRequestException ex)
            {
                var dataServiceClientException = ex.InnerException as DataServiceClientException;
                if (dataServiceClientException != null)
                {
                    if (dataServiceClientException.StatusCode == 409)
                    {
                        TraceHelper.TraceWarning(ex.TraceInformation());

                        var model = await this.CreateTenantPageViewDataAsync(contentModel);
                        model.Title = "New Survey";
                        this.ModelState.AddModelError("ContentModel.Title", string.Format(CultureInfo.InvariantCulture, "The name '{0}' is already in use. Please choose another name.", model.ContentModel.Title));
                        return this.View(model);
                    }
                }

                TraceHelper.TraceError(ex.TraceInformation());

                throw;
            }

            this.TempData.Remove(CachedSurvey);
            return this.RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> NewQuestion(SurveyModel contentModel)
        {
            var cachedSurvey = (SurveyModel)this.TempData[CachedSurvey];

            if (cachedSurvey == null)
            {
                return this.RedirectToAction("New");
            }

            cachedSurvey.Title = contentModel.Title;

            if ((cachedSurvey.Extensions != null) &&
                (cachedSurvey.Extensions.Count != contentModel.Extensions.Count))
            {
                this.ModelState.AddModelError("ContentModel.Extensions", string.Format(CultureInfo.InvariantCulture, "The number of custom properties don't match with the custom model."));
            }

            if (cachedSurvey.Extensions != null)
            {
                for (int i = 0; i < cachedSurvey.Extensions.Count; i++)
                {
                    cachedSurvey.Extensions[i].PropertyValue = contentModel.Extensions[i].PropertyValue;
                }
            }

            this.TempData[CachedSurvey] = cachedSurvey;

            var model = await this.CreateTenantPageViewDataAsync(new Question());
            model.Title = "New Question";

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddQuestion(Question contentModel)
        {
            var cachedSurvey = (SurveyModel)this.TempData[CachedSurvey];

            if (!this.ModelState.IsValid)
            {
                this.TempData[CachedSurvey] = cachedSurvey;
                var model = await this.CreateTenantPageViewDataAsync(contentModel ?? new Question());
                model.Title = "New Question";
                return this.View("NewQuestion", model);
            }

            if (contentModel.PossibleAnswers != null)
            {
                contentModel.PossibleAnswers = contentModel.PossibleAnswers.Replace("\r\n", "\n");
            }

            cachedSurvey.Questions.Add(contentModel);
            this.TempData[CachedSurvey] = cachedSurvey;
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

        private bool HasModelExtensionAssembly(Tenant tenant, out string assemblyFile, out string @namespace)
        {
            var result = false;
            assemblyFile = null;
            @namespace = null;

            if (SubscriptionKind.Premium.Equals(tenant.SubscriptionKind) &&
                !string.IsNullOrWhiteSpace(tenant.ModelExtensionAssembly) &&
                !string.IsNullOrWhiteSpace(tenant.ModelExtensionNamespace))
            {
                var extensionsPath = CloudConfiguration.GetConfigurationSetting(ExtensionsPath);
                if (!Path.IsPathRooted(extensionsPath))
                {
                    extensionsPath = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, extensionsPath);
                }

                assemblyFile = Path.Combine(extensionsPath, string.Format("{0}.dll", tenant.ModelExtensionAssembly));
                @namespace = tenant.ModelExtensionNamespace;

                result = true;
            }

            return result;
        }
    }
}