using MaziStore.ApiServer.Home.Extensions;
using MaziStore.Module.Core.Data;
using MaziStore.Module.Infrastructure;
using MaziStore.Module.Infrastructure.Data;
using MaziStore.Module.Infrastructure.Modules;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

// ////////////////////////////

ConfigureServices();

// ////////////////////////////

var app = builder.Build();

// ////////////////////////////

Configure();

// ////////////////////////////

app.MapGet("/", () => "Hello World!");

app.Run();

void ConfigureServices()
{
   builder.Services.AddControllers();

   GlobalVariables.WebRootPath = builder.Environment.WebRootPath;
   GlobalVariables.ContentRootPath = builder.Environment.ContentRootPath;

   builder.Services.AddModules();

   builder.Services.AddCustomizedDataStore(builder.Configuration);

   builder.Services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
   builder.Services.AddTransient(
      typeof(IRepositoryWithTypedId<,>),
      typeof(RepositoryWithTypedId<,>)
   );

   foreach (var module in GlobalVariables.Modules)
   {
      var moduleInitializerType = module.Assembly
         .GetTypes()
         .FirstOrDefault(x => typeof(IModuleInitializer).IsAssignableFrom(x));
      if (
         (moduleInitializerType != null)
         && (moduleInitializerType != typeof(IModuleInitializer))
      )
      {
         var moduleInitializer = (IModuleInitializer)
            Activator.CreateInstance(moduleInitializerType);
         builder.Services.AddSingleton(
            typeof(IModuleInitializer),
            moduleInitializer
         );
         moduleInitializer.ConfigureServices(builder.Services);
      }
   }

   builder.Services.AddScoped<ServiceFactory>(sp => sp.GetService);
   builder.Services.AddScoped<IMediator, Mediator>();
}

void Configure()
{
   app.UseHttpsRedirection();

   app.UseStaticFiles();

   app.MapControllerRoute("areas", "{area:exists}/{controller}/{action}/{id?}");
}
