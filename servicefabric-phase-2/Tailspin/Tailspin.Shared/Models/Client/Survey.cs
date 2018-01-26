namespace Tailspin.Shared.Models.Client
{
    using System;
    using System.Collections.Generic;

    public class Survey
    {
        public string SlugName { get; set; }

        public string Title { get; set; }

        public DateTime CreatedOn { get; set; }

        public IList<Question> Questions { get; set; } = new List<Question>();
    }
}
