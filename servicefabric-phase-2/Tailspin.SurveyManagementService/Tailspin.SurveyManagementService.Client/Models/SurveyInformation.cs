namespace Tailspin.SurveyManagementService.Client.Models
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public class SurveyInformation
    {
        public SurveyInformation()
        {
        }

        [DataMember(Order = 0)]
        public string SlugName { get; set; }

        [DataMember(Order = 1)]
        public string Title { get; set; }

        [DataMember(Order = 2)]
        public DateTime CreatedOn { get; set; }
    }
}
