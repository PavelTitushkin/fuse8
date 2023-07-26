using System.Reflection;
using System.Text.Json.Serialization;
using Fuse8_ByteMinds.SummerSchool.PublicApi.Middleware;
using Serilog;

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
		services.AddSwaggerGen(options =>
		{
			var xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
			options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFileName));
		});
	}

	public void Configure(IApplicationBuilder app, IWebHostEnvironment env, WebApplicationBuilder builder)
	{
		if (env.IsDevelopment())
		{
			builder.Host.UseSerilog((context, loggerConfiguration) =>
			{
				loggerConfiguration.WriteTo.Console();
			});
			app.UseSwagger();
			app.UseSwaggerUI();
		}

        //Добавление логирования
        app.UseMiddleware<LoggingMiddleware>();

		app.UseRouting()
			.UseEndpoints(endpoints => endpoints.MapControllers());
	}
}