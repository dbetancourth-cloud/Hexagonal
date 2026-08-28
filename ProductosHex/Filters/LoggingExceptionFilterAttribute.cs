using System.Web.Http.Filters;
using ProductosHex.Domain.Ports;

namespace ProductosHex.Filters
{
    public class LoggingExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly ILoggerService _logger;

        public LoggingExceptionFilterAttribute(ILoggerService logger)
        {
            _logger = logger;
        }

        public override void OnException(HttpActionExecutedContext context)
        {
            var accion = context.ActionContext.ActionDescriptor.ActionName;
            var controller = context.ActionContext.ControllerContext.ControllerDescriptor.ControllerName;

            _logger.LogError($"Excepción no controlada en {controller}.{accion}", context.Exception);

            base.OnException(context);
        }
    }
}