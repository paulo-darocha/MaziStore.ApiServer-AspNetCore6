﻿using MaziStore.Module.Core.Models;
using MaziStore.Module.Infrastructure.Data;
using System;
using System.Linq;

namespace MaziStore.Module.Core.Services
{
   public class WidgetInstanceService : IWidgetInstanceService
   {
      private readonly IRepository<WidgetInstance> _widgetInstanceRepository;

      public WidgetInstanceService(
         IRepository<WidgetInstance> widgetInstanceRepository
      )
      {
         _widgetInstanceRepository = widgetInstanceRepository;
      }

      public IQueryable<WidgetInstance> GetPublished()
      {
         var now = DateTimeOffset.Now;
         return _widgetInstanceRepository
            .QueryRp()
            .Where(
               x =>
                  x.PublishStart.HasValue
                  && x.PublishStart < now
                  && (!x.PublishEnd.HasValue || x.PublishEnd > now)
            );
      }
   }
}
