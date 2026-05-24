using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(OlyMath.Startup))]
namespace OlyMath
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}