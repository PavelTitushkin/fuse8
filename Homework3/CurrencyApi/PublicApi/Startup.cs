﻿using Audit.Core;
using Audit.Http;
using Fuse8_ByteMinds.SummerSchool.PublicApi.Abstractions;
using Fuse8_ByteMinds.SummerSchool.PublicApi.Filter;
using Fuse8_ByteMinds.SummerSchool.PublicApi.Middleware;
using Fuse8_ByteMinds.SummerSchool.PublicApi.Models.ModelsConfig;
using Fuse8_ByteMinds.SummerSchool.PublicApi.Services;
using Microsoft.Extensions.Primitives;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.AspNetCore;
using System.Text.Json.Serialization;


namespace Fuse8_ByteMinds.SummerSchool.PublicApi;

public class Startup
{
    private readonly IConfiguration _configuration;
    public IServiceCollection Services { get; set; }

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        Services = services;

        //Считаем секцию AppSettings из конфигурации
        var appSettings = _configuration.GetSection("Currency").Get<AppSettings>();
        appSettings.APIKey = _configuration.GetSection("Settings:APIKey").Value;
        appSettings.ClientConfigBuild();

        //Создадим Singleton конфигурации, и добавим его в коллекцию сервисов
        SingletonAppSettings singletonAppSettings = SingletonAppSettings.Instance;
        singletonAppSettings.appSettings = appSettings;
        services.AddSingleton(singletonAppSettings);
        services.AddScoped(sp => sp.GetService<SingletonAppSettings>().appSettings);

        //Добавление сервисов
        services.AddScoped<ICurrencyRateService, CurrencyRateService>();
        services.AddHttpClient<ICurrencyRateService, CurrencyRateService>()
            .AddAuditHandler(
            audit =>audit
                .IncludeRequestBody()
                .IncludeRequestHeaders()
                .IncludeResponseBody()
                .IncludeResponseHeaders()
                .IncludeContentHeaders());

        Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
        Configuration.Setup()
            .UseSerilog(
            config=>config.Message(
                auditEvent =>
                {
                    if(auditEvent is AuditEventHttpClient httpClientEvent)
                    {
                        var contentBody = httpClientEvent.Action?.Response?.Content?.Body;
                        if(contentBody is string { Length: > 1000} stringBody)
                        {
                            httpClientEvent.Action.Response.Content.Body = stringBody[..1000] + "<...>";
                        }
                    }
                    return auditEvent.ToJson();
                }));

        //Добавление фильтра исключения
        services.AddControllers(options =>
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

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
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
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        ChangeToken.OnChange(() => _configuration.GetReloadToken(), onChange);

        //Добавление логирования
        app.UseMiddleware<LoggingMiddleware>();

        app.UseRouting()
            .UseEndpoints(endpoints => endpoints.MapControllers());
    }

    private void onChange()
    {
        var newAppSettings = _configuration.GetSection("AppSettings").Get<AppSettings>();
        newAppSettings.ClientConfigBuild();
        var serviceAppSettings = Services.BuildServiceProvider().GetService<SingletonAppSettings>();
        serviceAppSettings.appSettings = newAppSettings;
    }
}