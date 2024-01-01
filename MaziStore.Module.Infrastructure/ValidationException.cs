using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MaziStore.Module.Infrastructure
{
   public class ValidationException : Exception
   {
      public ValidationException(
         IList<ValidationResult> validationResults,
         Type targetType
      )
      {
         ValidationResults = validationResults;
         TargetType = targetType;
      }

      public IList<ValidationResult> ValidationResults { get; }

      public Type TargetType { get; }

      public override string Message
      {
         get
         {
            return string.Concat(
               TargetType.ToString(),
               ": ",
               string.Join(';', ValidationResults.Select(x => $"{x.ErrorMessage}"))
            );
         }
      }
   }
}
