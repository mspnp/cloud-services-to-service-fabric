namespace Tailspin.Web.Models
{
    using Tailspin.Web.Survey.Shared.Models;

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

        public Survey ToSurvey()
        {
            return new Survey(this.SlugName)
            {
                TenantId = this.TenantId,
                Title = this.Title,                
                CreatedOn = this.CreatedOn,
                Questions = this.Questions
            };
        }
    }
}