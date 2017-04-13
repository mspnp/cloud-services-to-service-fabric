namespace Tailspin.Web.Models
{
    using Tailspin.Web.Shared.Models;

    public class SurveyModel : Survey
    {
        public SurveyModel() : base() { }

        public SurveyModel(string slugName) : base(slugName) { }

        public SurveyModel(Survey survey) : base(survey.SlugName)
        {
            this.CreatedOn = survey.CreatedOn;
            this.Questions = survey.Questions;
            this.TenantId = survey.TenantId;
            this.Title = survey.Title;
        }
    }
}