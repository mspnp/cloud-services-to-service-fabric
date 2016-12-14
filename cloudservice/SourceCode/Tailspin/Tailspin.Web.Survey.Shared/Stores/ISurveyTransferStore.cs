using System.Threading.Tasks;

namespace Tailspin.Web.Survey.Shared.Stores
{
    public interface ISurveyTransferStore
    {
        Task InitializeAsync();
        Task TransferAsync(string tenant, string slugName);
    }
}