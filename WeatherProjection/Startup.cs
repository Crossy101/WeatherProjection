using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WeatherProjection.Startup))]
namespace WeatherProjection
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
