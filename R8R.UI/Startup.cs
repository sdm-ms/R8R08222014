using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(R8R.UI.Startup))]
namespace R8R.UI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
