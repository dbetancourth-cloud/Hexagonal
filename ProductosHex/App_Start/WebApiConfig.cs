using System.Web.Http;
using ProductosHex.Domain.Ports;             // Importa los puertos (ILoggerService)
using ProductosHex.Infrastructure.Logging;   // Importa la implementación (FileLoggerService)
using ProductosHex.Filters;

namespace ProductosHex
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Declaración explícita con el tipo del Puerto de Dominio:
            ProductosHex.Domain.Ports.ILoggerService logger = new FileLoggerService();
            config.Filters.Add(new LoggingExceptionFilterAttribute(logger));
        }
    }
}