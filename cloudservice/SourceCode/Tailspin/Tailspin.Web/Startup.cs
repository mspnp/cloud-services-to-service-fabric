using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Tailspin.Web.Startup))]

namespace Tailspin.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}