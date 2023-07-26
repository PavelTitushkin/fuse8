namespace Fuse8_ByteMinds.SummerSchool.PublicApi.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                _logger.LogInformation($"Поступил запрос: {context.Request.Method} {context.Request.Path}");
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка!");
                throw;
            }
        }
    }
}
