using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Internals.Fibers;

namespace DotsBot
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            RegisterBotDependencies();

            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        private void RegisterBotDependencies()
        {
            Conversation.UpdateContainer(builder =>
            {
                builder.RegisterModule(new ReflectionSurrogateModule());
                builder.RegisterModule<DotsBotModule>();
                builder.RegisterControllers(typeof(WebApiApplication).Assembly);
            });

            DependencyResolver.SetResolver(new AutofacDependencyResolver(Conversation.Container));
        }
    }
}