﻿using MaziStore.Module.Catalog.Data;
using MaziStore.Module.Catalog.Services;
using MaziStore.Module.Infrastructure.Modules;
using Microsoft.Extensions.DependencyInjection;

namespace MaziStore.Module.Catalog
{
   public class ModuleInitializer : IModuleInitializer
   {
      public void ConfigureServices(IServiceCollection services)
      {
         services.AddTransient<IProductPricingService, ProductPricingService>();
         services.AddTransient<IProductService, ProductService>();
         services.AddTransient<
            IProductTemplateProductAttributeRepository,
            ProductTemplateProductAttributeRepository
         >();
         services.AddTransient<ICategoryService, CategoryService>();
         services.AddTransient<IBrandService, BrandService>();
      }
   }
}
