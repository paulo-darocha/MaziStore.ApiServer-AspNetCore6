﻿namespace MaziStore.Module.Infrastructure.Models
{
   public interface IEntityWithTypedId<TId>
   {
      TId Id { get; }
   }
}
