namespace Tailspin.Web.Controllers
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Tailspin.Web.Survey.Shared.Stores;
    using Microsoft.AspNetCore.Http;

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
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UploadLogo(string tenant, IFormFile newLogo)
        {
            // TODO: Validate that the file received is an image
            if (newLogo != null && newLogo.Length > 0)
            {
                await this.TenantStore.UploadLogoAsync(tenant, new BinaryReader(newLogo.OpenReadStream()).ReadBytes(Convert.ToInt32(newLogo.Length)));
            }

            return this.RedirectToAction("Index");
        }
    }
}