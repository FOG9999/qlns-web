using System.Net.Http.Formatting;
using System.Web.Http;

namespace VIETTEL
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // suport json
            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter());
            //config.Formatters.Add(new XmlMediaTypeFormatter());
        }
    }
}
