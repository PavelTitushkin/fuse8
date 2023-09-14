using Fuse8_ByteMinds.SummerSchool.PublicApi;
using Serilog;


Host.CreateDefaultBuilder(args)
  .ConfigureWebHostDefaults(
    webBuilder =>
    {
        webBuilder.UseStartup<Startup>();
        webBuilder.UseSerilog((context, config) =>
                {
                    config.ReadFrom.Configuration(context.Configuration)
                    .MinimumLevel.Information()
                    .Enrich.WithMachineName()
                    .WriteTo.Console();
                });
    })
      .Build().Run();
