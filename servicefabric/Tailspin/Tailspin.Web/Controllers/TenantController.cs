namespace Tailspin.Web.Controllers
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Tailspin.Web.Models;
    using Tailspin.Web.Survey.Shared.Models;
    using Tailspin.Web.Survey.Shared.Stores;

    public abstract class TenantController : Controller
    {
        private readonly ITenantStore tenantStore;
        private string tenantId;

        protected TenantController(ITenantStore tenantStore)
        {
            this.tenantStore = tenantStore;
        }

        public ITenantStore TenantStore
        {
            get { return this.tenantStore; }
        }

        public string TenantId
        {
            get
            {
                return this.tenantId;
            }

            set
            {
                this.tenantId = value;
                this.ViewData["tenantId"] = value;
            }
        }

        private Tenant Tenant { get; set; }

        [NonAction]
        public async Task<Tenant> GetTenantAsync()
        {
            if (Tenant == null)
            {
                Tenant = await this.TenantStore.GetTenantAsync(this.tenantId);
                if (Tenant == null)
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentUICulture, "'{0}' is not a valid tenant.", this.tenantId));
                }
            }
            return Tenant;
        }

        protected async Task<TenantPageViewData<T>> CreateTenantPageViewDataAsync<T>(T contentModel)
        {
            var tenant = await GetTenantAsync();
            var tenantPageViewData = new TenantPageViewData<T>(contentModel)
            {
                LogoUrl = tenant?.Logo ?? string.Empty
            };
            return tenantPageViewData;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.RouteData.Values["tenantId"] != null)
            {
                this.TenantId = (string)filterContext.RouteData.Values["tenantId"];
            }
            base.OnActionExecuting(filterContext);
        }
    }
}
