﻿using System.Threading.Tasks;

namespace MaziStore.Module.Core.Services
{
   public interface IEmailSender
   {
      Task SendEmailAsync(
         string email,
         string subject,
         string message,
         bool isHtml = false
      );
   }
}
