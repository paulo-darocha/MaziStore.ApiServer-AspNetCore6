using MaziStore.ApiServer.Home.Extensions;
using MaziStore.Module.Core.Data;
using MaziStore.Module.Infrastructure;
using MaziStore.Module.Infrastructure.Data;
using MaziStore.Module.Infrastructure.Modules;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace MaziStore.ApiServer.Home
{
   public class Program
   {
      public static void Main(string[] args)
      {
         var builder = WebApplication.CreateBuilder(args);
         //builder.WebHost.UseUrls("https://*:7000");

         // ////////////////////////////

         ConfigureServices(builder);

         // ////////////////////////////

         var app = builder.Build();

         // ////////////////////////////

         Configure(app);

         // ////////////////////////////

         app.MapGet("/", () => "Hello World!");

         app.Run();
      }

      public static void ConfigureServices(WebApplicationBuilder builder)
      {
         builder.Services
            .AddControllers()
            .AddJsonOptions(
               x =>
                  x.JsonSerializerOptions.ReferenceHandler =
                     ReferenceHandler.IgnoreCycles
            );

         GlobalVariables.WebRootPath = builder.Environment.WebRootPath;
         GlobalVariables.ContentRootPath = builder.Environment.ContentRootPath;

         builder.Services.AddMaziModules();
         builder.Services.AddMaziDataStore(builder.Configuration);
         builder.Services.AddMaziCors();
         builder.Services.AddMaziIdentity(builder.Configuration);

         //builder.Services.AddHttpContextAccessor();

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

      public static void Configure(WebApplication app)
      {
         if (app.Environment.IsDevelopment())
         {
            app.UseDeveloperExceptionPage();
         }
         //app.UseHttpsRedirection();

         app.Use(
            (httpContext, nextDelegate) =>
            {
               httpContext.Response.Headers["Access-Control-Allow-Origin"] =
                  "http://store.paulodarocha.eu;http://localhost:8000";
               return nextDelegate.Invoke();
            }
         );

         app.UseCors("CorsPolicy");

         app.UseCookiePolicy();

         app.UseAuthentication();

         app.UseAuthorization();

         app.UseStaticFiles();

         app.MapControllerRoute(
            "areas",
            "{area:exists}/{controller}/{action}/{id?}"
         );

         app.SetupMaziDatabase();
      }
   }
}
