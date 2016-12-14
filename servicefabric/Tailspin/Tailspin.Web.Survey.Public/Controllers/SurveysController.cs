namespace Tailspin.Web.Survey.Public.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Tailspin.Web.Survey.Public.Models;
    using Tailspin.Web.Survey.Shared.Models;
    using Tailspin.Web.Survey.Shared.Stores;

    public class SurveysController : Controller
    {
        private readonly ISurveyStore surveyStore;
        private readonly ISurveyAnswerStore surveyAnswerStore;

        public SurveysController(ISurveyStore surveyStore, ISurveyAnswerStore surveyAnswerStore)
        {
            this.surveyStore = surveyStore;
            this.surveyAnswerStore = surveyAnswerStore;
        }

        public string TenantId { get; private set; }

        public async Task<ActionResult> Index()
        {
            var model = new TenantPageViewData<IEnumerable<Survey>>(await this.surveyStore.GetRecentSurveysAsync());
            model.Title = "Existing surveys";
            return this.View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Display(string tenantId, string surveySlug)
        {
            var surveyAnswer = await this.CallGetSurveyAndCreateSurveyAnswerAsync(tenantId, surveySlug);

            var model = new TenantPageViewData<SurveyAnswer>(surveyAnswer);
            model.Title = surveyAnswer.Title;
            return this.View(model);
        }
        
        [HttpPost]
        public async Task<ActionResult> Display(string tenantId, string surveySlug, SurveyAnswer contentModel)
        {
            var surveyAnswer = await this.CallGetSurveyAndCreateSurveyAnswerAsync(tenantId, surveySlug);

            if (surveyAnswer.QuestionAnswers.Count != contentModel.QuestionAnswers.Count)
            {
                throw new ArgumentException("The survey answers received have different amount of questions than the survey to be filled.");
            }

            for (int i = 0; i < surveyAnswer.QuestionAnswers.Count; i++)
            {
                surveyAnswer.QuestionAnswers[i].Answer = contentModel.QuestionAnswers[i].Answer;
            }

            if (!this.ModelState.IsValid)
            {
                var model = new TenantPageViewData<SurveyAnswer>(surveyAnswer);
                model.Title = surveyAnswer.Title;
                return this.View(model);
            }

            await this.surveyAnswerStore.SaveSurveyAnswerAsync(surveyAnswer);

            return this.RedirectToAction("ThankYou");
        }
        
        public ActionResult ThankYou()
        {
            var model = new TenantMasterPageViewData { Title = "Thank you for filling the survey" };
            return this.View(model);
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.RouteData.Values["tenantId"] != null)
            {
                this.TenantId = (string)filterContext.RouteData.Values["tenantId"];
                this.ViewData["tenantId"] = this.TenantId;
            }

            base.OnActionExecuting(filterContext);
        }

        private async Task<SurveyAnswer> CallGetSurveyAndCreateSurveyAnswerAsync(string tenantId, string surveySlug)
        {
            var survey = await this.surveyStore.GetSurveyByTenantAndSlugNameAsync(tenantId, surveySlug, true);

            var surveyAnswer = new SurveyAnswer
            {
                Title = survey.Title,
                SlugName = surveySlug,
                TenantId = tenantId
            };

            foreach (var question in survey.Questions)
            {
                surveyAnswer.QuestionAnswers.Add(new QuestionAnswer
                {
                    QuestionText = question.Text,
                    QuestionType = question.Type,
                    PossibleAnswers = question.PossibleAnswers
                });
            }

            return surveyAnswer;
        }
    }
}
