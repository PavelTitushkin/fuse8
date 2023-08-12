using Audit.Core;
using Audit.Http;
using Fuse8_ByteMinds.SummerSchool.InternalApi.Abstractions;
using Fuse8_ByteMinds.SummerSchool.InternalApi.Contracts;
using Fuse8_ByteMinds.SummerSchool.InternalApi.Filter;
using Fuse8_ByteMinds.SummerSchool.InternalApi.Middleware;
using Fuse8_ByteMinds.SummerSchool.InternalApi.Models.ModelsConfig;
using Fuse8_ByteMinds.SummerSchool.InternalApi.Services;
using Fuse8_ByteMinds.SummerSchool.PublicApi.Services;
using InternalApi.Contracts;
using InternalApi.Data;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.OpenApi.Models;
using Serilog;

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

            //���������� gRPC-�������
            builder.Services.AddGrpc();

            builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("Currency"));
            builder.Services.Configure<AppSettings>(opt =>
            {
                opt.APIKey = builder.Configuration.GetSection("Settings:APIKey").Value;
            });

            builder.Services.AddControllers();

            //Add Auto-mapper
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            builder.Services.AddScoped<ICurrencyRateService, CurrencyRateService>();
            builder.Services.AddScoped<ICachedCurrencyAPI, CachedCurrencyAPIService>();
            builder.Services.AddScoped<ICachedCurrencyRepository, CachedCurrencyRepository>();
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

            //���������� ������� ����������
            builder.Services.AddControllers(options =>
            {
                options.Filters.Add(typeof(ApiExceptionFilter));
            });
            //// ��������� ���������� ��������� ��� �������������� Json
            //.AddJsonOptions(
            //    options =>
            //    {
            //        // ��������� ��������� ��� �����
            //        // �� ��������� ���� ������������� � �������� ��������
            //        // ���� ����������� ������ ������� � ��������� ��������
            //        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            //    });
            //;
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                //��������� Swagegr
                c.SwaggerDoc(
                    "v1",
                    new OpenApiInfo()
                    {
                        Title = "����� �����",
                        Version = "v1",
                        Description = "API ��� ��������� ����� �����"
                    });
                //���������� ����������
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{typeof(Program).Assembly.GetName().Name}.xml"), true);
            });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //��������� gRPC
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

            //���������� �����������
            app.UseMiddleware<LoggingMiddleware>();

            app.Run();
        }
    }
}