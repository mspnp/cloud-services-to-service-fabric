namespace Tailspin.Web.Survey.Shared.Stores.AzureStorage
{
    using System;
    using Microsoft.WindowsAzure.Storage.Table;

    public class SurveyRow : TableEntity
    {
        public string SlugName { get; set; }

        public string Title { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}