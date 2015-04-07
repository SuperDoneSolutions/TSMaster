using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TotalSquashNext.Startup))]
namespace TotalSquashNext
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
