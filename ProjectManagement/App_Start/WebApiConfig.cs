using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Microsoft.AspNetCore.Cors;
using System.Web.Http.Cors;  // ✅ Correct for Web API 2


namespace ProjectManagement
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var cors = new System.Web.Http.Cors.EnableCorsAttribute("*", "*", "*");  // origin, headers, methods
            config.EnableCors(cors);
            
            config.MapHttpAttributeRoutes();  // 🔥 Required!

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }

    }
}
