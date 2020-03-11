using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PaymentTempalte.Startup))]
namespace PaymentTempalte
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
