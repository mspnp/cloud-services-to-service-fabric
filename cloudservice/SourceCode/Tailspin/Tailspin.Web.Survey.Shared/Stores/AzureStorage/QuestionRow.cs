namespace Tailspin.Web.Survey.Shared.Stores.AzureStorage
{
    using Microsoft.WindowsAzure.Storage.Table;

    public class QuestionRow : TableEntity
    {
        public string Text { get; set; }

        public string Type { get; set; }

        public string PossibleAnswers { get; set; }
    }
}