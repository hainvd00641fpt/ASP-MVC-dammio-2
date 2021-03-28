using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(OneCMVC1.Startup))]
namespace OneCMVC1
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
