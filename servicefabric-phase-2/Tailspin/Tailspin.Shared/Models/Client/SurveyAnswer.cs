namespace Tailspin.Shared.Models.Client
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class SurveyAnswer
    {
        public string Id { get; set; }

        public string SlugName { get; set; }

        public string Title { get; set; }

        public DateTime CreatedOn { get; set; }

        public IList<QuestionAnswer> QuestionAnswers { get; set; } = new List<QuestionAnswer>();
    }
}