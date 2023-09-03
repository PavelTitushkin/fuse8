using DataStore.InternalApiDb;
using Audit.Core;
using Audit.Http;
using Fuse8_ByteMinds.SummerSchool.InternalApi.Contracts.IRepositories;
using InternalApi.Contracts;
using InternalApi.Data;
using InternalApi.Filter;
using InternalApi.Middleware;
using InternalApi.Models.ModelsConfig;
using InternalApi.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.OpenApi.Models;
using Serilog;
using Fuse8_ByteMinds.SummerSchool.InternalApi.Contracts.IQueues;
using Fuse8_ByteMinds.SummerSchool.InternalApi.Queues;
using Fuse8_ByteMinds.SummerSchool.InternalApi.Services;
using Fuse8_ByteMinds.SummerSchool.InternalApi.Contracts;

namespace InternalApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.WebHost.UseKestrel(
                (builderContext, options) =>
                {
                    var grpcPort = builder.Configuration.GetValue<int>("gRPCPort");
                    options.ConfigureEndpointDefaults(
                        p =>
                        {
                            p.Protocols = p.IPEndPoint!.Port == grpcPort ? HttpProtocols.Http2 : HttpProtocols.Http1;
                        });
                });

            //Подключение DbContext
            builder.Services.AddDbContext<InternalApiContext>(
                optionsBuilder =>
                {
                    optionsBuilder.UseNpgsql(builder.Configuration.GetConnectionString("CurrencyRateDb"),
                        sqlOptionsBuilder =>
                        {
                            sqlOptionsBuilder.EnableRetryOnFailure();
                            sqlOptionsBuilder.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "cur");
                        })
                    .UseSnakeCaseNamingConvention();
                });

            //Добавление gRPC-сервиса
            builder.Services.AddGrpc();

            builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("Currency"));
            builder.Services.Configure<AppSettings>(opt =>
            {
                opt.APIKey = builder.Configuration.GetSection("Settings:APIKey").Value;
            });

            builder.Services.AddControllers();

            //Add Auto-mapper
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            //Добавление сервисов
            builder.Services.AddScoped<ICurrencyAPI, CurrencyRateService>();
            builder.Services.AddScoped<ICurrencyRateService, CurrencyRateService>();
            builder.Services.AddScoped<ICachedCurrencyRepository, CachedCurrencyRepository>();
            builder.Services.AddScoped<ICachedCurrencyAPI, CachedCurrencyAPI>();
            builder.Services.AddScoped<IRecalculateCacheService, RecalculateCacheService>();

            builder.Services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            builder.Services.AddHostedService<QueuedHostedService>();

            builder.Services.AddHttpClient<ICurrencyRepository, HttpCurrencyRepository>()
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
            });
            //// Добавляем глобальные настройки для преобразования Json
            //.AddJsonOptions(
            //    options =>
            //    {
            //        // Добавляем конвертер для енама
            //        // По умолчанию енам преобразуется в цифровое значение
            //        // Этим конвертером задаем перевод в строковое занчение
            //        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            //    });
            //;
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

            builder.Services.AddHealthChecks();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.MapHealthChecks("/health");

            //Настройка gRPC
            app.UseWhen(
                predicate: context => context.Connection.LocalPort == builder.Configuration.GetValue<int>("gRPCPort"),
                configuration: grpcBuilder =>
                {
                    grpcBuilder.UseRouting();
                    grpcBuilder.UseEndpoints(endpoints => endpoints.MapGrpcService<CurrencyRateGrpcService>());
                });

            app.UseRouting()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });

            //Добавление логирования
            app.UseMiddleware<LoggingMiddleware>();

            app.Run();
        }
    }
}