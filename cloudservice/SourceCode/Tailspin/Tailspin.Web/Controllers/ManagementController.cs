namespace Tailspin.Web.Controllers
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Tailspin.Web.Models;
    using Tailspin.Web.Survey.Shared.Models;
    using Tailspin.Web.Survey.Shared.Stores;
    using Security;
    using System.Threading.Tasks;

    [RequireHttps]
    [AuthenticateAndAuthorize(Roles = TailspinRoles.TenantAdministrator)]
    public class ManagementController : Controller
    {
        private readonly ITenantStore tenantStore;

        public ManagementController(ITenantStore tenantStore)
        {
            this.tenantStore = tenantStore;
        }

        public ActionResult Index()
        {
            var model = new TenantPageViewData<IEnumerable<string>>(this.tenantStore.GetTenantIds())
            {
                Title = "Subscribers"
            };
            return this.View(model);
        }

        public async Task<ActionResult> Detail(string tenantId)
        {
            var contentModel = await this.tenantStore.GetTenantAsync(tenantId);
            var model = new TenantPageViewData<Tenant>(contentModel)
            {
                Title = string.Format("{0} details", contentModel.TenantId)
            };
            return this.View(model);
        }

        public ActionResult New()
        {
            var model = new TenantPageViewData<Tenant>(new Tenant())
            {
                Title = "New Tenant"
            };
            return this.View(model);
        }

        [HttpPost]
        public async Task<ActionResult> New(Tenant tenant)
        {
            if (string.IsNullOrWhiteSpace(tenant.TenantId))
            {
                var model = new TenantPageViewData<Tenant>(tenant)
                {
                    Title = "New Tenant : Error!"
                };
                this.ViewData["error"] = "Organization's name cannot be empty";
                return this.View(model);
            }
            else if (tenant.TenantId.Equals("new", System.StringComparison.InvariantCultureIgnoreCase))
            {
                var model = new TenantPageViewData<Tenant>(tenant)
                {
                    Title = "New Tenant : Error!"
                };
                this.ViewData["error"] = "Organization's name cannot be 'new'";
                return this.View(model);
            }

            // TODO: check if tenant already exist
            await this.tenantStore.SaveTenantAsync(tenant);

            return this.RedirectToAction("Index");
        }
    }
}
