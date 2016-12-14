namespace Tailspin.Web.Controllers
{
    using Microsoft.Owin.Security;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Tailspin.Web.Models;
    using Tailspin.Web.Survey.Shared.Models;
    using Tailspin.Web.Survey.Shared.Stores;
    using Microsoft.Owin.Security.Cookies;
    using Microsoft.Owin.Security.OpenIdConnect;
    using System.Web;

    public class OnBoardingController : Controller
    {
        private readonly ITenantStore tenantStore;

        public OnBoardingController(ITenantStore tenantStore)
        {
            this.tenantStore = tenantStore;
        }

        public ITenantStore TenantStore
        {
            get { return this.tenantStore; }
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            IList<Tenant> tenants = new List<Tenant>();
            foreach (var tenantId in this.tenantStore.GetTenantIds())
            {
                tenants.Add(await this.tenantStore.GetTenantAsync(tenantId));
            }

            var model = new TenantPageViewData<IEnumerable<Tenant>>(tenants)
            {
                Title = "On boarding"
            };
            return this.View(model);
        }

        [HttpGet]
        public void Join()
        {
            var authenticationProperties = new AuthenticationProperties(new Dictionary<string, string> { { "signup", "true" } });
            authenticationProperties.RedirectUri = "/";
            HttpContext.GetOwinContext().Authentication.Challenge(authenticationProperties, OpenIdConnectAuthenticationDefaults.AuthenticationType);

            // TODO: Incorporate this view into Tenant Management page
            //var model = new TenantMasterPageViewData { Title = "Join Tailspin" };
            //return this.View(model);
        }
    }
}
