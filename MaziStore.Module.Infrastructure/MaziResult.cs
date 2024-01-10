namespace MaziStore.Module.Infrastructure
{
   public class MaziResult
   {
      public bool Success { get; set; }

      public string Error { get; set; }

      public MaziResult(bool success, string error)
      {
         Success = success;
         Error = error;
      }

      public static MaziResult Fail(string error)
      {
         return new MaziResult(false, error);
      }

      public static MaziResult Ok()
      {
         return new MaziResult(true, null);
      }

      public static MaziResult<TValue> Ok<TValue>(TValue value)
      {
         return new MaziResult<TValue>(value, true, null);
      }

      public static MaziResult<TValue> Fail<TValue>(string error)
      {
         return new MaziResult<TValue>(default(TValue), false, null);
      }
   }
}
