namespace Tailspin.Web.Shared.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    [Serializable]
    public class Survey
    {
        private readonly string slugName;

        public Survey()
            : this(string.Empty)
        {
        }

        public Survey(string slugName)
        {
            this.slugName = slugName;
            this.Questions = new List<Question>();
        }

        public string SlugName
        {
            get
            {
                return this.slugName;
            }
        }

        public string TenantId { get; set; }

        [Required(ErrorMessage = "* You must provide a Title for the survey.")]
        public string Title { get; set; }

        public DateTime CreatedOn { get; set; }
        
        public List<Question> Questions { get; set; }
    }
}