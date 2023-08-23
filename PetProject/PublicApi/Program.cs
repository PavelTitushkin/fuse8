using Fuse8_ByteMinds.SummerSchool.PublicApi;
using Microsoft.AspNetCore;
using Serilog;

//var webHost = WebHost
//    .CreateDefaultBuilder(args)
//    .UseStartup<Startup>()
//    .UseSerilog((context, config) =>
//    {
//        config.ReadFrom.Configuration(context.Configuration)
//        .MinimumLevel.Information()
//        .Enrich.WithMachineName()
//        .WriteTo.Console();
//    })
//    .Build();

//await webHost.RunAsync();

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
