using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Owin;

namespace OwinSelfhostSample
{
    public class Startup
    {
        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            //return values
            config.Routes.MapHttpRoute(
                name: "ReturnType",
                routeTemplate:"api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional}
                );

            config.Routes.MapHttpRoute(
                name : "RouteTemplate",
                routeTemplate:"api/{controller}/{action}/{category}/{id}",
                defaults: new { category = "all",id=RouteParameter.Optional },
                constraints: new { id = @"\d+" }
                );

            appBuilder.UseWebApi(config);
        }
    }
}
