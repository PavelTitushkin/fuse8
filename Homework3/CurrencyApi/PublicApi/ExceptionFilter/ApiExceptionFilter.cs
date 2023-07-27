using System.Net;
using System.Web.Http.Filters;
using Fuse8_ByteMinds.SummerSchool.PublicApi.Exceptions;

namespace Fuse8_ByteMinds.SummerSchool.PublicApi.ExceptionFilter
{
    public class ApiExceptionFilter : Attribute, IExceptionFilter
    {
        //private readonly ILogger<ApiExceptionFilter> _logger;

        //public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger)
        //{
        //    _logger = logger;
        //}

        public bool AllowMultiple
        {
            get { return true; }
        }

        public Task ExecuteExceptionFilterAsync(HttpActionExecutedContext context, CancellationToken cancellationToken)
        {
            if (context.Exception != null && context.Exception is ApiRequestLimitException)
            {
                context.Response = context.Request.CreateErrorResponse(HttpStatusCode.TooManyRequests, "Превышен лимит запросов API");
                //_logger.LogError("Превышен лимит запросов API");
            }
            if(context.Exception != null && context.Exception is CurrencyNotFoundException)
            {
                context.Response = context.Request.CreateResponse(HttpStatusCode.NotFound, context.Exception.Message);
            }
            if (context.Exception != null && context.Exception is not CurrencyNotFoundException || context.Exception != null && context.Exception is not ApiRequestLimitException)
            {
                //_logger.LogError(context.Exception.Message);
                context.Response = context.Request.CreateResponse(HttpStatusCode.InternalServerError, context.Exception.Message);
            }

            return Task.FromResult<object>(null);
        }
    }
}
