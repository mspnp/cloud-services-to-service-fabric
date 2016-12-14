namespace Tailspin.Web.Survey.Shared.Stores.Azure
{
    public interface IAzureObjectWithRetryPolicyFactory
    {
        IRetryPolicyFactory RetryPolicyFactory { get; set; }

        IRetryPolicyFactory GetRetryPolicyFactoryInstance();
    }
}
