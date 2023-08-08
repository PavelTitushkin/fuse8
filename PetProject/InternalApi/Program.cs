using Audit.Core;
using Audit.Http;
using Fuse8_ByteMinds.SummerSchool.InternalApi.Abstractions;
using Fuse8_ByteMinds.SummerSchool.InternalApi.Filter;
using Fuse8_ByteMinds.SummerSchool.InternalApi.Middleware;
using Fuse8_ByteMinds.SummerSchool.InternalApi.Models.ModelsConfig;
using Fuse8_ByteMinds.SummerSchool.PublicApi.Services;
using InternalApi.Contracts;
using InternalApi.Data;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text.Json.Serialization;

namespace InternalApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("Currency"));
            builder.Services.Configure<AppSettings>(opt =>
            {
                opt.APIKey = builder.Configuration.GetSection("Settings:APIKey").Value;
            });


            builder.Services.AddControllers();

            builder.Services.AddScoped<ICurrencyRateService, CurrencyRateService>();
            //builder.Services.AddScoped<ICachedCurrencyAPI, CachedCurrencyRepository>();

            builder.Services.AddHttpClient<IHttpCurrencyRepository, HttpCurrencyRepository>()
                .AddAuditHandler(
                audit => audit
                    .IncludeRequestBody()
                    .IncludeRequestHeaders()
                    .IncludeResponseBody()
                    .IncludeResponseHeaders()
                    .IncludeContentHeaders());

            Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
            Configuration.Setup()
                .UseSerilog(
                config => config.Message(
                    auditEvent =>
                    {
                        if (auditEvent is AuditEventHttpClient httpClientEvent)
                        {
                            var contentBody = httpClientEvent.Action?.Response?.Content?.Body;
                            if (contentBody is string { Length: > 1000 } stringBody)
                            {
                                httpClientEvent.Action.Response.Content.Body = stringBody[..1000] + "<...>";
                            }
                        }
                        return auditEvent.ToJson();
                    }));

            //Добавление фильтра исключения
            builder.Services.AddControllers(options =>
            {
                options.Filters.Add(typeof(ApiExceptionFilter));
            })
            // Добавляем глобальные настройки для преобразования Json
            .AddJsonOptions(
                options =>
                {
                    // Добавляем конвертер для енама
                    // По умолчанию енам преобразуется в цифровое значение
                    // Этим конвертером задаем перевод в строковое занчение
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });
            ;
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                //Настройка Swagegr
                c.SwaggerDoc(
                    "v1",
                    new OpenApiInfo()
                    {
                        Title = "Курсы валют",
                        Version = "v1",
                        Description = "API для получения курса валют"
                    });
                //Добавление коментарии
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{typeof(Program).Assembly.GetName().Name}.xml"), true);
            });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //Добавление логирования
            app.UseMiddleware<LoggingMiddleware>();

            app.UseRouting()
                .UseEndpoints(endpoints => endpoints.MapControllers());

            app.MapControllers();
            app.Run();
        }
    }
}