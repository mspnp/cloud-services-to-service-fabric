namespace Tailspin.Web.Controllers
{
    using System;
    using System.IO;
    using System.Web;
    using System.Web.Mvc;
    using Tailspin.Web.Survey.Shared.Stores;
    using System.Threading.Tasks;

    [RequireHttps]
    [Authorize]
    public class AccountController : TenantController 
    {
        public AccountController(ITenantStore tenantStore) : base(tenantStore)
        {
        }

        public async Task<ActionResult> Index()
        {
            var model = await this.CreateTenantPageViewDataAsync(await GetTenantAsync());
            model.Title = "My Account";
            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UploadLogo(string tenant, HttpPostedFileBase newLogo)
        {
            // TODO: Validate that the file received is an image
            if (newLogo != null && newLogo.ContentLength > 0)
            {
                await this.TenantStore.UploadLogoAsync(tenant, new BinaryReader(newLogo.InputStream).ReadBytes(Convert.ToInt32(newLogo.InputStream.Length)));
            }

            return this.RedirectToAction("Index");
        }
    }
}