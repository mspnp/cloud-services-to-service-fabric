namespace Tailspin.Web.Survey.Shared.Stores.AzureStorage
{
    using Microsoft.WindowsAzure.Storage.Blob;

    public interface IListBlobItemWithName : IListBlobItem
    {
        string Name { get; }
    }
}
