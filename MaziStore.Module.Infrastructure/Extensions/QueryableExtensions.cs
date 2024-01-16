using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MaziStore.Module.Infrastructure.Extensions
{
   public static class QueryableExtensions
   {
      public static IQueryable<T> OrderByName<T>(
         this IQueryable<T> source,
         string propertyName,
         bool isDescending
      )
      {
         if (source == null)
         {
            throw new ArgumentNullException(nameof(source));
         }

         if (string.IsNullOrWhiteSpace(propertyName))
         {
            throw new ArgumentException(
               "Order By property should not be empty",
               nameof(propertyName)
            );
         }

         Type type = typeof(T);
         ParameterExpression arg = Expression.Parameter(type, "x");
         PropertyInfo propertyInfo = type.GetProperty(propertyName);
         Expression expression = Expression.Property(arg, propertyName);
         type = propertyInfo.PropertyType;

         Type delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
         LambdaExpression lambda = Expression.Lambda(delegateType, expression, arg);

         var methodName = isDescending ? "OrderByDescending" : "OrderBy";

         object result = typeof(Queryable)
            .GetMethods()
            .Single(
               x =>
                  x.Name == methodName
                  && x.IsGenericMethodDefinition
                  && x.GetGenericArguments().Length == 2
                  && x.GetParameters().Length == 2
            )
            .MakeGenericMethod(typeof(T), type)
            .Invoke(null, new object[] { source, lambda });

         return (IQueryable<T>)result;
      }
   }
}
