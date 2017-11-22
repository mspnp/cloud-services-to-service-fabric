namespace Tailspin.SurveyManagementService.Models
{
    using System;
    using Microsoft.WindowsAzure.Storage.Table;

    public class SurveyInformationRow : TableEntity
    {
        public SurveyInformationRow()
            : base()
        {
        }

        public string SlugName { get; set; }

        public string Title { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
