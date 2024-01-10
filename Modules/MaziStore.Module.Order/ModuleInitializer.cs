using MaziStore.Module.Infrastructure.Modules;
using MaziStore.Module.Orders.Events;
using MaziStore.Module.Orders.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace MaziStore.Module.Orders
{
   public class ModuleInitializer : IModuleInitializer
   {
      public void ConfigureServices(IServiceCollection services)
      {
         services.AddTransient<IOrderService, OrderService>();
         services.AddTransient<
            INotificationHandler<OrderCreated>,
            OrderCreatedCreateOrderHistoryHandler
         >();
         //services.AddTransient<
         //   INotificationHandler<AfterOrderCreated>,
         //   AfterOrderCreatedSendEmailHandler
         //>();
         services.AddTransient<IOrderEmailService, OrderEmailService>();
      }
   }
}
