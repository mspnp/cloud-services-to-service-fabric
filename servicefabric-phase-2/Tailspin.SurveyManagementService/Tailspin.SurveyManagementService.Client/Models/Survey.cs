namespace Tailspin.SurveyManagementService.Client.Models
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract]
    public class Survey
    {
        public Survey()
        {
        }

        [DataMember(Order = 0)]
        public string SlugName { get; set; }

        [DataMember(Order = 1)]
        public string Title { get; set; }

        [DataMember(Order = 2)]
        public DateTime CreatedOn { get; set; }

        [DataMember(Order = 3)]
        public IList<Question> Questions { get; set; } = new List<Question>();
    }
}
