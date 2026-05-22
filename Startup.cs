using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WAPP_Assignment_Module.Startup))]
namespace WAPP_Assignment_Module
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
