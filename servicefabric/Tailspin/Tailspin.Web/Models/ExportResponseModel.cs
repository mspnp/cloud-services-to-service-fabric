namespace Tailspin.Web.Models
{
    using Web.Survey.Shared.Models;

    public class ExportResponseModel
    {
        public bool HasResponses { get; set; }

        public Tenant Tenant { get; set; }
    }
}