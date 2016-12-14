namespace Tailspin.AnswerAnalysisService.Commands
{
    using Tailspin.Web.Survey.Shared.Stores.AzureStorage;

    public interface ICommand<in T> where T : AzureQueueMessage
    {
        bool Run(T message);
    }
}