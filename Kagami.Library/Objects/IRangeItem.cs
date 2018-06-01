namespace Kagami.Library.Objects
{
   public interface IRangeItem : IObjectCompare
   {
      IRangeItem Successor { get; }

      IRangeItem Predecessor { get; }

      Range Range();
   }
}