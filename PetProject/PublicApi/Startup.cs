﻿using Audit.Core;
using Audit.Http;
using DataPublicApi;
using Fuse8_ByteMinds.SummerSchool.PublicApi.Abstractions;
using Fuse8_ByteMinds.SummerSchool.PublicApi.Contracts.IRepositories;
using Fuse8_ByteMinds.SummerSchool.PublicApi.Data;
using Fuse8_ByteMinds.SummerSchool.PublicApi.Middleware;
using Fuse8_ByteMinds.SummerSchool.PublicApi.Models.ModelsConfig;
using Fuse8_ByteMinds.SummerSchool.PublicApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.OpenApi.Models;
using PublicClientApi;
using Serilog;
using System.Text.Json.Serialization;

namespace Fuse8_ByteMinds.SummerSchool.PublicApi;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.Configure<AppSettings>(_configuration.GetSection("Currency"));
        services.Configure<AppSettings>(opt =>
        {
            opt.APIKey = _configuration.GetSection("Settings:APIKey").Value;
        });

        //Добавление сервисов
        services.AddScoped<ICurrencyRepository, CurrencyRepository>();
        services.AddScoped<CurrencyRateGrpcClientService>();
        services.AddScoped<ICurrencyRateService, CurrencyRateService>();

        //Add Auto-mapper
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        //Подключение DbContext
        services.AddDbContext<PublicApiContext>(
            optionsBuilder =>
            {
                optionsBuilder.UseNpgsql(_configuration.GetConnectionString("CurrencyRateDb"),
                    sqlOptionsBuilder =>
                    {
                        sqlOptionsBuilder.EnableRetryOnFailure();
                        sqlOptionsBuilder.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "user");
                    })
                .UseSnakeCaseNamingConvention();
            });

        services.AddGrpcClient<CurrrncyGrpsService.CurrrncyGrpsServiceClient>(o =>
        {
            o.Address = new Uri(_configuration.GetValue<string>("gRPCServive"));
        })
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

        services.AddControllers()
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

        services.AddHealthChecks();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHealthChecks("/health");

        //Добавление логирования
        app.UseMiddleware<LoggingMiddleware>();

        app.UseRouting()
            .UseEndpoints(endpoints => endpoints.MapControllers());
    }
}