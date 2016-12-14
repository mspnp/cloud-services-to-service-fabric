namespace Tailspin.Web.Areas.Survey.Models
{
    using Web.Survey.Shared.Models;

    public class BrowseResponseModel
    {
        public SurveyAnswer SurveyAnswer { get; set; }

        public string NextAnswerId { get; set; }

        public string PreviousAnswerId { get; set; }

        public bool CanMoveForward
        {
            get
            {
                return !string.IsNullOrEmpty(this.NextAnswerId);
            }
        }

        public bool CanMoveBackward
        {
            get
            {
                return !string.IsNullOrEmpty(this.PreviousAnswerId);
            }
        }
    }
}