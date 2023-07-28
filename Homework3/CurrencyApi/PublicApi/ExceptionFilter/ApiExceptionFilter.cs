using Fuse8_ByteMinds.SummerSchool.PublicApi.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Fuse8_ByteMinds.SummerSchool.PublicApi.ExceptionFilter
{
    public class ApiExceptionFilter : Attribute, IExceptionFilter
    {
        private readonly ILogger<ApiExceptionFilter> _logger;

        public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            if (context.Exception != null && context.Exception is ApiRequestLimitException)
            {
                _logger.LogError("Превышен лимит запросов API");
                context.Result = new StatusCodeResult(429);
            }
            if (context.Exception != null && context.Exception is CurrencyNotFoundException)
            {
                context.Result = new StatusCodeResult(404);
            }
            else
            {
                _logger.LogError(context.Exception.Message);
                context.Result = new StatusCodeResult(500);
            }
        }
    }
}
