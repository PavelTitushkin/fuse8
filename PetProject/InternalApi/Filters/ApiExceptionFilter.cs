using InternalApi.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace InternalApi.Filter
{
    /// <summary>
    /// Класс фильтра исключения
    /// </summary>
    public class ApiExceptionFilter : Attribute, IExceptionFilter
    {
        private readonly ILogger<ApiExceptionFilter> _logger;

        public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            switch (context.Exception.GetType().Name)
            {
                case nameof(ApiRequestLimitException) :
                    _logger.LogError("Превышен лимит запросов API");
                    context.Result = new StatusCodeResult(429);
                    break;
                case nameof(CurrencyNotFoundException):
                    context.Result = new StatusCodeResult(404);
                    break;
                default:
                    _logger.LogError(context.Exception.GetType().Name, context.Exception.Message, context.HttpContext.Response.StatusCode);
                    context.Result = new StatusCodeResult(500);
                    break;
            }
        }
    }
}
