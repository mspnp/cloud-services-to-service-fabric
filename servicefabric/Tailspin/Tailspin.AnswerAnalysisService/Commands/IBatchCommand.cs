namespace Tailspin.AnswerAnalysisService.Commands
{
    using Tailspin.Web.Survey.Shared.Stores.AzureStorage;

    public interface IBatchCommand<in T> : ICommand<T> where T : AzureQueueMessage
    {
        void PreRun();
        void PostRun();
    }
}