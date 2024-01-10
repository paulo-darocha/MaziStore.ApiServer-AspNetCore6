namespace MaziStore.Module.Infrastructure
{
   public class MaziResult<TValue> : MaziResult
   {
      public TValue Value { get; set; }

      protected internal MaziResult(TValue value, bool success, string error)
         : base(success, error)
      {
         Value = value;
      }
   }
}
