using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ACRES.WebUI.Startup))]
namespace ACRES.WebUI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
