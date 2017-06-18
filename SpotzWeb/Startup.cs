using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SpotzWeb.Startup))]
namespace SpotzWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
