using Fuse8_ByteMinds.SummerSchool.PublicApi.Abstractions;
using Fuse8_ByteMinds.SummerSchool.PublicApi.ExceptionFilter;
using Fuse8_ByteMinds.SummerSchool.PublicApi.Middleware;
using Fuse8_ByteMinds.SummerSchool.PublicApi.Services;
using Microsoft.OpenApi.Models;
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
        //Добавление сервисов
        services.AddScoped<ILimitCheck, LimitCheck>();
        services.AddScoped<ISendMessage, SendMessage>();

        //Ещё ругается на логер при запуске, пока ещё не выяснил почему
        services.AddControllers()
        //    (options =>
        //{
        //    options.Filters.Add(typeof(ApiExceptionFilter));
        //})

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
            c.SwaggerDoc(
                "v1",
                new OpenApiInfo()
                {
                    Title = "Курсы валют",
                    Version = "v1",
                    Description = "API для получения курса валют"
                });
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

        //Добавление логирования
        app.UseMiddleware<LoggingMiddleware>();

        app.UseRouting()
            .UseEndpoints(endpoints => endpoints.MapControllers());
    }
}