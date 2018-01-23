using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Active.Startup))]
namespace Active
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
